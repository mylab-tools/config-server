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
        private readonly SecretApplier _secretApplier;
        private readonly SecretsAnalyzer _secretsAnalyzer;
        private string ConfigsPath { get; }
        private string OverridesPath { get; }
        private string IncludePath { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="DefaultConfigProvider"/>
        /// </summary>
        public DefaultConfigProvider(
            string basePath, 
            SecretApplier secretApplier,
            SecretsAnalyzer secretsAnalyzer)
        {
            _secretApplier = secretApplier;
            _secretsAnalyzer = secretsAnalyzer;
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
            var confWithUnresolvedSecrets = await LoadConfigCore(id, prettyJson);

            var config = _secretApplier.ApplySecrets(confWithUnresolvedSecrets);

            return new ConfigInfo
            {
                Secrets = _secretsAnalyzer.GetSecrets(config).ToArray(),
                Content = config
            };
        }

        public async Task<ConfigInfo> LoadConfigWithoutSecrets(string id, bool prettyJson)
        {
            var config = await LoadConfigCore(id, prettyJson);

            return new ConfigInfo
            {
                Secrets = _secretsAnalyzer.GetSecrets(config).ToArray(),
                Content = config
            };
        }

        public async Task<ConfigInfo> LoadConfigBase(string id)
        {
            var str = await File.ReadAllTextAsync(Path.Combine(ConfigsPath, id + ".json"));
            var config = JsonPrettyFormatter.JsonPrettify(str);

            return new ConfigInfo
            {
                Secrets = _secretsAnalyzer.GetSecrets(config).ToArray(),
                Content = config
            };
        }

        public async Task<ConfigInfo> LoadOverride(string id)
        {
            var str = await File.ReadAllTextAsync(Path.Combine(OverridesPath, id + ".json"));
            var config = JsonPrettyFormatter.JsonPrettify(str);

            return new ConfigInfo
            {
                Secrets = _secretsAnalyzer.GetSecrets(config).ToArray(),
                Content = config
            };
        }

        public async Task<ConfigInfo> LoadInclude(string id)
        {
            var str = await File.ReadAllTextAsync(Path.Combine(IncludePath, id + ".json"));
            var config = JsonPrettyFormatter.JsonPrettify(str);

            return new ConfigInfo
            {
                Secrets = _secretsAnalyzer.GetSecrets(config).ToArray(),
                Content = config
            };
        }

        public async Task<string> LoadConfigCore(string id, bool prettyJson)
        {
            var originStr = await File.ReadAllTextAsync(Path.Combine(ConfigsPath, id + ".json"));

            var overridePath = Path.Combine(OverridesPath, id + ".json");
            var overridingStr = File.Exists(overridePath) ? await File.ReadAllTextAsync(overridePath) : null;

            var configBuilder = new ConfigBuilder(new DefaultIncludeProvider(IncludePath))
            {
                PrettyJson = prettyJson
            };

            return await configBuilder.Build(originStr, overridingStr);
        }
    }
}
