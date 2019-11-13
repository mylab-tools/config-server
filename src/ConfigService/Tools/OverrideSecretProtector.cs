using System.Linq;
using Newtonsoft.Json;

namespace ConfigService.Tools
{
    static class OverrideSecretProtector
    {
        public static string Protect(string overrideJson, string originConfig)
        {
            var originXml = JsonConvert.DeserializeXNode(originConfig, "root");
            var overrideXml = JsonConvert.DeserializeXNode(overrideJson, "root");

            var originElements = originXml
                .Descendants()
                .Where(e => e != originXml.Root)
                .ToDictionary(XElementPathProvider.Provide, e => e);

            var overridingElements = overrideXml
                .Descendants()
                .Where(e => e != originXml.Root)
                .Select(n => new XElementDesc
                {
                    Element = n,
                    Path = XElementPathProvider.Provide(n)
                })
                .ToArray();

            foreach (var e in overridingElements)
            {
                if (originElements.TryGetValue(e.Path, out var originElement))
                {
                    if (originElement.Value == ConfigConstants.SecretMarkerValue)
                    {
                        e.Element.Value = ConfigConstants.SecretHideText;
                    }
                }
            }

            return JsonConvert.SerializeXNode(overrideXml, Formatting.Indented, true);
        }
    }
}