using System.IO;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace ConfigService.Tools
{
    class ConfigMerger
    {
        public bool HideSecrets { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="ConfigMerger"/>
        /// </summary>
        public ConfigMerger(bool hideSecrets)
        {
            HideSecrets = hideSecrets;
        }

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

            return JsonConvert.SerializeXNode(originXml, Formatting.None, true);
        }

        class XElementDesc
        {
            public XElement Element { get; set; }
            public string Path { get; set; }
        }
    }
}
