using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MyLab.ConfigServer.Tools;

namespace MyLab.ConfigServer.Services
{
    public interface IConfigProvider
    {
        IEnumerable<string> GetConfigList();
        IEnumerable<string> GetOverrideList();
        IEnumerable<string> GetIncludeList();

        Task<ConfigInfo> LoadConfig(string id, bool prettyJson);
        Task<ConfigInfo> LoadConfigWithoutSecrets(string id, bool prettyJson);
        Task<ConfigInfo> LoadConfigBase(string id);
        Task<ConfigInfo> LoadOverride(string id);
        Task<ConfigInfo> LoadInclude(string id);
    }

    public class ConfigInfo
    {
        public string Content { get; set; }
        public ConfigSecret[] Secrets { get; set; }
    }

    class DefaultConfigProvider : IConfigProvider
    {
        private readonly ISecretsProvider _secretsProvider;
        private readonly SecretsAnalyzer _secretsAnalyzer;
        private string ConfigsPath { get; }
        private string OverridesPath { get; }
        private string IncludePath { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="DefaultConfigProvider"/>
        /// </summary>
        public DefaultConfigProvider(
            string basePath,
            ISecretsProvider secretsProvider)
        {
            _secretsProvider = secretsProvider;
            _secretsAnalyzer = new SecretsAnalyzer(secretsProvider);
            ConfigsPath = Path.Combine(basePath, "Configs");
            OverridesPath = Path.Combine(basePath, "Overrides");
            IncludePath = Path.Combine(basePath, "Includes");
        }

        public IEnumerable<string> GetConfigList()
        {
            if (!Directory.Exists(ConfigsPath))
                return Enumerable.Empty<string>();

            return Directory
                .EnumerateFiles(ConfigsPath, "*.json")
                .Select(Path.GetFileNameWithoutExtension);
        }

        public IEnumerable<string> GetOverrideList()
        {
            if (!Directory.Exists(OverridesPath))
                return Enumerable.Empty<string>();

            return Directory
                .EnumerateFiles(OverridesPath, "*.json")
                .Select(Path.GetFileNameWithoutExtension);
        }

        public IEnumerable<string> GetIncludeList()
        {
            if (!Directory.Exists(IncludePath))
                return Enumerable.Empty<string>();

            return Directory
                .EnumerateFiles(IncludePath, "*.json")
                .Select(Path.GetFileNameWithoutExtension);
        }

        public async Task<ConfigInfo> LoadConfig(string id, bool prettyJson)
        {
            var confDoc = await LoadConfigCore(id);
            confDoc.ApplySecrets(_secretsProvider);

            return new ConfigInfo
            {
                Secrets = _secretsAnalyzer.GetSecrets(confDoc).ToArray(),
                Content = confDoc.Serialize(prettyJson)
            };
        }

        public async Task<ConfigInfo> LoadConfigWithoutSecrets(string id, bool prettyJson)
        {
            var config = await LoadConfigCore(id);

            return new ConfigInfo
            {
                Secrets = _secretsAnalyzer.GetSecrets(config).ToArray(),
                Content = config.Serialize(prettyJson)
            };
        }

        public async Task<ConfigInfo> LoadConfigBase(string id)
        {
            var str = await File.ReadAllTextAsync(Path.Combine(ConfigsPath, id + ".json"));
            var config = ConfigDocument.Load(str);

            return new ConfigInfo
            {
                Secrets = _secretsAnalyzer.GetSecrets(config).ToArray(),
                Content = config.Serialize(true)
            };
        }

        public async Task<ConfigInfo> LoadOverride(string id)
        {
            var str = await File.ReadAllTextAsync(Path.Combine(OverridesPath, id + ".json"));
            var config = ConfigDocument.Load(str);

            return new ConfigInfo
            {
                Secrets = _secretsAnalyzer.GetSecrets(config).ToArray(),
                Content = config.Serialize(true)
            };
        }

        public async Task<ConfigInfo> LoadInclude(string id)
        {
            var str = await File.ReadAllTextAsync(Path.Combine(IncludePath, id + ".json"));
            var config = ConfigDocument.Load(str);

            return new ConfigInfo
            {
                Secrets = _secretsAnalyzer.GetSecrets(config).ToArray(),
                Content = config.Serialize(true)
            };
        }

        public async Task<ConfigDocument> LoadConfigCore(string id)
        {
            var originStr = await File.ReadAllTextAsync(Path.Combine(ConfigsPath, id + ".json"));
            var originDoc = ConfigDocument.Load(originStr);

            await originDoc.ApplyIncludes(new DefaultIncludeProvider(IncludePath));

            var overridePath = Path.Combine(OverridesPath, id + ".json");
            var overridingStr = File.Exists(overridePath) ? await File.ReadAllTextAsync(overridePath) : null;

            if (!string.IsNullOrWhiteSpace(overridingStr))
            {
                var overrideDoc = ConfigDocument.Load(overridingStr);
                originDoc.ApplyOverride(overrideDoc);
            }

            return originDoc;
        }
    }
}
