using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MyLab.ConfigServer.Tools
{
    public interface ISecretsProvider
    {
        IDictionary<string, string> Provide();
    }

    class DefaultSecretsProvider : ISecretsProvider
    {
        private readonly string _json;

        public DefaultSecretsProvider(string json)
        {
            _json = json;
        }

        public static DefaultSecretsProvider LoadFromFile(string filePath)
        {
            return new DefaultSecretsProvider(
                File.Exists(filePath) 
                    ? File.ReadAllText(filePath)
                    : null);
        }

        public IDictionary<string, string> Provide()
        {
            if(_json == null)
                return new Dictionary<string, string>();

            var items = JsonConvert.DeserializeObject<ConfigSecretItem[]>(_json);
            return items.ToDictionary(itm => itm.Key, itm => itm.Value);
        }

        private class ConfigSecretItem
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }
    }
}