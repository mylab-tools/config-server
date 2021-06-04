using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MyLab.ConfigServer.Tools
{
    class ConfigDocumentInclude
    {
        private readonly XComment _includeComment;

        public string Link { get; }

        public ConfigDocumentInclude(XComment includeComment)
        {
            _includeComment = includeComment;
            Link = includeComment.Value.Substring(8).Trim();
        }

        public void Resolve(ConfigDocument includeDoc)
        {
            if (includeDoc == null) throw new ArgumentNullException(nameof(includeDoc));
            if (includeDoc.GetXDocument()?.Root == null) return;

            var payloadElements = includeDoc.GetXDocument().Root
                .Descendants()
                .Where(e => !e.HasElements);

            foreach (var pe in payloadElements)
            {
                var elementPath = new List<XElement>();
                GetElementPath(pe, elementPath);

                var newElement = BuildPathElements(_includeComment.Parent, elementPath);

                if (string.IsNullOrEmpty(newElement.Value))
                    newElement.Value = pe.Value;
            }

            _includeComment.Remove();
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
            if (endElement.Parent != null && endElement.Document?.Root != endElement.Parent)
                GetElementPath(endElement.Parent, elements);
        }
    }
}