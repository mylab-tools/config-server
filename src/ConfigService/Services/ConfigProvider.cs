using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ConfigService.Tools;

namespace ConfigService.Services
{
    public interface IConfigProvider
    {
        IEnumerable<string> GetConfigList();

        Task<string> LoadConfig(string id, bool hideSecrets, bool prettyJson);
    }

    class DefaultConfigProvider : IConfigProvider
    {
        string BasePath { get; }

        private string ConfigsPath { get; }
        private string OverridesPath { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="DefaultConfigProvider"/>
        /// </summary>
        public DefaultConfigProvider(string basePath)
        {
            BasePath = basePath;
            ConfigsPath = Path.Combine(basePath, "Configs");
            OverridesPath = Path.Combine(basePath, "Overrides");
        }

        public IEnumerable<string> GetConfigList()
        {
            return Directory
                .EnumerateFiles(ConfigsPath, "*.json")
                .Select(Path.GetFileNameWithoutExtension);
        }

        public async Task<string> LoadConfig(string id, bool hideSecrets, bool prettyJson)
        {
            var originStr = await File.ReadAllTextAsync(Path.Combine(ConfigsPath, id + ".json"));

            var overridePath = Path.Combine(OverridesPath, id + ".json");
            if (!File.Exists(overridePath))
            {
                var pretty = JsonPrettyFormatter.JsonPrettify(originStr);
                return pretty;
            }

            var overridingStr = await File.ReadAllTextAsync(overridePath);
            var merger = new ConfigMerger
            {
                HideSecrets = hideSecrets,
                PrettyJson = prettyJson
            };

            return merger.Merge(originStr, overridingStr);
        }
    }
}
