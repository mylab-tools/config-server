using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MyLab.ConfigServer.Server.Tools;
using MyLab.ConfigServer.Shared;
using Newtonsoft.Json;

namespace MyLab.ConfigServer.Server.Services
{
    public interface IConfigProvider
    {
        IEnumerable<StoredConfig> GetConfigList();
        IEnumerable<StoredConfig> GetOverrideList();
        IEnumerable<StoredConfig> GetIncludeList();

        Task<ConfigInfo> LoadConfig(string id, bool prettyJson);
        Task<ConfigInfo> LoadConfigWithoutSecrets(string id, bool prettyJson);
        Task<ConfigInfo> LoadConfigBase(string id);
        Task<ConfigInfo> LoadOverride(string id);
        Task<ConfigInfo> LoadInclude(string id);
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

        public IEnumerable<StoredConfig> GetConfigList()
        {
            if (!Directory.Exists(ConfigsPath))
                return Enumerable.Empty<StoredConfig>();

            return Directory
                .EnumerateFiles(ConfigsPath, "*.json")
                .Select(f => new FileInfo(f))
                .Select(StoredConfig.FromFile);
        }

        public IEnumerable<StoredConfig> GetOverrideList()
        {
            if (!Directory.Exists(OverridesPath))
                return Enumerable.Empty<StoredConfig>();

            return Directory
                .EnumerateFiles(OverridesPath, "*.json")
                .Select(f => new FileInfo(f))
                .Select(StoredConfig.FromFile);
        }

        public IEnumerable<StoredConfig> GetIncludeList()
        {
            if (!Directory.Exists(IncludePath))
                return Enumerable.Empty<StoredConfig>();

            return Directory
                .EnumerateFiles(IncludePath, "*.json")
                .Select(f => new FileInfo(f))
                .Select(StoredConfig.FromFile);
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
                Content = JsonPrettify(str)
            };
        }

        public async Task<ConfigInfo> LoadOverride(string id)
        {
            var str = await File.ReadAllTextAsync(Path.Combine(OverridesPath, id + ".json"));
            var config = ConfigDocument.Load(str);

            return new ConfigInfo
            {
                Secrets = _secretsAnalyzer.GetSecrets(config).ToArray(),
                Content = JsonPrettify(str)
            };
        }

        public async Task<ConfigInfo> LoadInclude(string id)
        {
            var str = await File.ReadAllTextAsync(Path.Combine(IncludePath, id + ".json"));
            var config = ConfigDocument.Load(str);

            return new ConfigInfo
            {
                Secrets = _secretsAnalyzer.GetSecrets(config).ToArray(),
                Content = JsonPrettify(str)
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

        public static string JsonPrettify(string json)
        {
            string pretty;
            using (var stringReader = new StringReader(json))
            using (var stringWriter = new StringWriter())
            {
                var jsonReader = new JsonTextReader(stringReader);
                var jsonWriter = new JsonTextWriter(stringWriter) { Formatting = Formatting.Indented };
                jsonWriter.WriteToken(jsonReader);
                pretty = stringWriter.ToString();
            }

            pretty = Regex.Replace(pretty, "\\/\\*.*?\\*\\/", m => Environment.NewLine + "  " + m.Value);

            return pretty;
        }
    }
}
