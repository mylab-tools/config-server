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

            var addElements = new List<XElement>();
            foreach (var pe in payloadElements)
            {
                var pathItems = new List<XElement>();
                GetElementPath(pe, pathItems);

                var newElement = BuildPathElements(_includeComment.Parent, pathItems.ToArray(), pe.Name.LocalName, addElements);

                if (string.IsNullOrEmpty(newElement.Value))
                    newElement.Value = pe.Value;
            }

            _includeComment.Remove();
        }

        XElement BuildPathElements(XContainer root, XElement[] pathItems, string endElementName, List<XElement> addElements)
        {
            XContainer parentContainer = root;
            XElement pathItemElement = null;

            for (int i = 0; i < pathItems.Length; i++)
            {
                var pathItem = pathItems[i];

                pathItemElement = parentContainer.Elements().FirstOrDefault(e => e.Name.LocalName == pathItem.Name.LocalName);

                bool isLastPathItem = i == pathItems.Length - 1;
                if (pathItemElement == null ||
                    (isLastPathItem && pathItem.Name.LocalName == endElementName && addElements.Contains(pathItemElement)))
                {
                    pathItemElement = new XElement(XName.Get(pathItem.Name.LocalName));

                    var isArrayAttrName = XName.Get("Array", "http://james.newtonking.com/projects/json");
                    var isArrAttr = pathItem.Attribute(isArrayAttrName);
                    if (isArrAttr != null)
                    {
                        pathItemElement.Add(new XAttribute(isArrayAttrName, isArrAttr.Value));
                    }
                    
                    addElements.Add(pathItemElement);

                    parentContainer.Add(pathItemElement);
                }

                parentContainer = pathItemElement;
            }

            return pathItemElement;
        }

        void GetElementPath(XElement endElement, List<XElement> pathItems)
        {
            pathItems.Insert(0, endElement);
            if (endElement.Parent != null && endElement.Document?.Root != endElement.Parent)
                GetElementPath(endElement.Parent, pathItems);
        }
    }
}