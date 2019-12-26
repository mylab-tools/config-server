using System.Linq;
using System.Xml.Linq;

namespace ConfigService.Tools
{
    class OverrideProcessor
    {
        public void Override(XDocument originDoc, XDocument overrideDoc)
        {
            var originElements = originDoc
                .Descendants()
                .Where(e => e != originDoc.Root && !e.HasElements)
                .Select(n => new XElementDesc
                {
                    Element = n,
                    Path = XElementPathProvider.Provide(n)
                })
                .ToArray();
            var overridingElements = overrideDoc
                .Descendants()
                .Where(e => e != overrideDoc.Root)
                .ToDictionary(XElementPathProvider.Provide, e => e);

            foreach (var e in originElements)
            {
                if (overridingElements.TryGetValue(e.Path, out var overridingElement))
                    e.Element.Value = overridingElement.Value;
            }
        }
    }
}