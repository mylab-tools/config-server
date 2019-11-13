using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace ConfigService.Tools
{
    class ConfigMerger
    {
        public bool HideSecrets { get; set; }

        public bool PrettyJson { get; set; }

        public string Merge(string originStr, string overridingStr)
        {
            var originXml = JsonConvert.DeserializeXNode(originStr, "root");
            var overrideXml = JsonConvert.DeserializeXNode(overridingStr, "root");

            var originElements = originXml
                .Descendants()
                .Where(e => e != originXml.Root)
                .Select(n => new XElementDesc
                {
                    Element = n,
                    Path = XElementPathProvider.Provide(n)
                })
                .ToArray();

            var overridingElements = overrideXml
                .Descendants()
                .Where(e => e != originXml.Root)
                .ToDictionary(XElementPathProvider.Provide, e => e);

            foreach (var e in originElements)
            {
                if (e.Element.Value == ConfigConstants.SecretMarkerValue && HideSecrets)
                {
                    e.Element.Value = ConfigConstants.SecretHideText;
                    continue;
                }

                if (overridingElements.TryGetValue(e.Path, out var overridingElement))
                {
                    e.Element.Value = overridingElement.Value;
                }
            }

            return JsonConvert.SerializeXNode(originXml, 
                PrettyJson 
                    ? Formatting.Indented
                    : Formatting.None, 
                true);
        }
    }
}
