using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace MyLab.ConfigServer.Tools
{
    class ConfigDocument 
    {
        private readonly XDocument _xDoc;

        public ConfigDocument(XDocument xDoc)
        {
            _xDoc = xDoc;
        }

        public static ConfigDocument Load(string json)
        {
            return new ConfigDocument(JsonConvert.DeserializeXNode(json, "root"));
        }

        public string Serialize(bool pretty)
        {
            return JsonConvert.SerializeXNode(_xDoc,
                pretty
                    ? Formatting.Indented
                    : Formatting.None,
                true);
        }

        public XDocument GetXDocument()
        {
            return _xDoc;
        }

        public IEnumerable<ConfigDocumentInclude> GetIncludes()
        {
            var containerElement = _xDoc.Root != null ? (XContainer)_xDoc.Root : _xDoc;

            return containerElement
                .Nodes()
                .OfType<XComment>()
                .Where(c => c.Value.StartsWith("include:"))
                .Select(c => new ConfigDocumentInclude(c));
        }


        public IEnumerable<ConfigDocumentOverride> CreateOverrides()
        {
            return _xDoc
                .Descendants()
                .Where(e => e != _xDoc.Root && !e.HasElements)
                .Select(n => new ConfigDocumentOverride(XElementPathProvider.Provide(n), n.Value));
        }

        public void ApplyOverrides(IEnumerable<ConfigDocumentOverride> overrides)
        {
            var forOverride = _xDoc
                .Descendants()
                .Where(e => e != _xDoc.Root)
                .ToDictionary(XElementPathProvider.Provide, e => e);

            foreach (var ovrd in overrides)
            {
                if (forOverride.TryGetValue(ovrd.Path, out var forOverrideElement))
                {
                    forOverrideElement.Value = ovrd.Value;
                }
            }
        }

        public IEnumerable<ConfigDocumentSecret> GetSecrets()
        {
            foreach (var descendant in _xDoc.Descendants().Where(d => !d.HasElements))
            {
                var match = Regex.Match(descendant.Value, "\\[secret:(?<skey>[\\w\\-\\d]+)\\]");
                if (!match.Success)
                    continue;

                var secretKey = match.Groups["skey"].Value;

                yield return new ConfigDocumentSecret(descendant, secretKey, XElementPathProvider.Provide(descendant));
            }
        }

        static class XElementPathProvider
        {
            public static string Provide(XElement xElement)
            {
                var sb = new StringBuilder(xElement.Name.LocalName);
                ExtractXElementBasePath(xElement, sb);
                return sb.ToString();
            }

            static void ExtractXElementBasePath(XElement xElement, StringBuilder sb)
            {
                var parent = xElement.Parent;

                if (parent?.Parent == null)
                {
                    sb.Insert(0, "/");
                    return;
                }

                sb.Insert(0, parent.Name.LocalName + "/");
                ExtractXElementBasePath(parent, sb);
            }
        }
    }
}