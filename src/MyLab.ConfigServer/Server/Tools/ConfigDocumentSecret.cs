using System.Xml.Linq;

namespace MyLab.ConfigServer.Server.Tools
{
    class ConfigDocumentSecret
    {
        private readonly XElement _element;

        public string Key { get; }
        public string Path { get; }

        public ConfigDocumentSecret(XElement element, string key, string path)
        {
            Key = key;
            Path = path;
            _element = element;
            
        }

        public void Resolve(string secretValue)
        {
            _element.Value = secretValue;
        }
    }
}