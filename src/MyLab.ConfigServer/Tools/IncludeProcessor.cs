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
            
            var containerElement = doc.Root != null ? (XContainer)doc.Root : doc;

            var includeNodes = containerElement
                .Nodes()
                .OfType<XComment>()
                .Where(c => c.Value.StartsWith("include:"))
                .ToArray();
            var includeIds = includeNodes
                .Select(c => c.Value.Substring(8).Trim());
            
            foreach (var id in includeIds)
            {
                var includeContent = await IncludesProvider.GetInclude(id);
                if(includeContent == null)
                    continue;

                var xDoc = JsonConvert.DeserializeXNode(includeContent, "root");

                await ResolveIncludes(xDoc, deepCount + 1);

                if (xDoc.Root != null)
                {
                    foreach (var d in xDoc.Root
                        .Descendants()
                        .Where(e => !e.HasElements && e != xDoc.Root))
                    {
                        var elementPath = new List<XElement>();
                        GetElementPath(d, elementPath);

                        var newElement = BuildPathElements(doc, elementPath);
                        
                        if(string.IsNullOrEmpty(newElement.Value))
                            newElement.Value = d.Value;
                    }
                }
            }

            foreach (var includeNode in includeNodes)
                includeNode.Remove();
        }

        XElement BuildPathElements(XContainer root, IEnumerable<XElement> path)
        {
            XContainer parentContainer = root;
            XElement pathItemElement = null;
            foreach (var pe in path)
            {
                pathItemElement = parentContainer.Elements().FirstOrDefault(e => e.Name.LocalName == pe.Name.LocalName);
                if (pathItemElement == null)
                {
                    pathItemElement = new XElement(pe.Name);
                    parentContainer.Add(pathItemElement);
                }

                parentContainer = pathItemElement;
            }

            return pathItemElement;
        }

        void GetElementPath(XElement endElement, List<XElement> elements)
        {
            elements.Insert(0, endElement);
            if(endElement.Parent != null)
                GetElementPath(endElement.Parent, elements);
        }
    }
}
