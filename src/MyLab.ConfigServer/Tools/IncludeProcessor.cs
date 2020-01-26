using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace MyLab.ConfigServer.Tools
{
    class IncludeProcessor
    {
        public IIncludesProvider IncludesProvider { get; }

        public int MaxDeep { get; set; } = 2;

        /// <summary>
        /// Initializes a new instance of <see cref="IncludeProcessor"/>
        /// </summary>
        public IncludeProcessor(IIncludesProvider includesProvider)
        {
            IncludesProvider = includesProvider;
        }

        public async Task ResolveIncludes(XDocument doc)
        {
            await ResolveIncludes(doc, 0);
        }
        async Task ResolveIncludes(XDocument doc, int deepCount)
        {
            if (deepCount >= MaxDeep) return;

            var originElementsNames = doc.Root == null
              ? Array.Empty<string>()
              : doc.Root
                .Nodes()
                .OfType<XElement>()
                .Select(e => e.Name.LocalName)
                .ToArray();

            var containerElement = doc.Root != null ? (XContainer)doc.Root : doc;

            var includeNodes = containerElement
                .Nodes()
                .OfType<XComment>()
                .ToArray();
            var includeIds = includeNodes
                .Select(n => new 
                    {
                        n.Value, 
                        SeparatorIndex = n.Value.IndexOf(":")
                    }
                )
                .Where(c => c.SeparatorIndex > 0 && c.SeparatorIndex < c.Value.Length-1)
                .Select(c => c.Value.Substring(c.SeparatorIndex + 1).Trim());

            var elementToInsert = new List<XElement>();

            foreach (var id in includeIds)
            {
                var includeContent = await IncludesProvider.GetInclude(id);
                var xDoc = JsonConvert.DeserializeXNode(includeContent, "root");

                await ResolveIncludes(xDoc, deepCount + 1);

                if (xDoc.Root != null)
                {
                    foreach (var node in xDoc.Root.Nodes().OfType<XElement>())
                    {
                        if (!originElementsNames.Contains(node.Name.LocalName))
                        {
                            elementToInsert.Add(node);
                        }
                    }
                }
            }

            foreach (var includeNode in includeNodes)
                includeNode.Remove();
            foreach (var elToInsert in elementToInsert)
                containerElement.Add(elToInsert);
        }
    }
}
