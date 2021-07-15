using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using MyLab.ConfigServer.Server.Services;
using MyLab.LogDsl;
using Newtonsoft.Json;

namespace MyLab.ConfigServer.Server.Tools
{
    public interface ISecretsProvider
    {
        IDictionary<string, string> Provide();
    }

    class DefaultSecretsProvider : ISecretsProvider
    {
        private readonly string _path;
        private readonly DslLogger _log;

        public DefaultSecretsProvider(IResourcePathProvider resourcePathProvider, ILogger<DefaultSecretsProvider> logger = null)
        {
            var dirPath  = resourcePathProvider.Provide();
            _path = Path.Combine(dirPath, "secrets.json");
            _log = logger?.Dsl();
        }

        public IDictionary<string, string> Provide()
        {
            if (!File.Exists(_path))
            {
                _log?.Warning("Secrets file not found")
                    .AndFactIs("Path", _path)
                    .Write();

                return new Dictionary<string, string>();
            }

            var json = File.ReadAllText(_path);
            var items = JsonConvert.DeserializeObject<ConfigSecretItem[]>(json);
            return items.ToDictionary(itm => itm.Key, itm => itm.Value);
        }

        public class ConfigSecretItem
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }
    }
}