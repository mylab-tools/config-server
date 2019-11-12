using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigService.Services
{
    public interface IConfigProvider
    {
        IEnumerable<string> GetConfigList();

        string LoadConfig(string id);
    }

    class DefaultConfigProvider : IConfigProvider
    {
        string BasePath { get; }

        private string ConfigPath { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="DefaultConfigProvider"/>
        /// </summary>
        public DefaultConfigProvider(string basePath)
        {
            BasePath = basePath;
            ConfigPath = Path.Combine(basePath, "ConfigFiles");
        }

        public IEnumerable<string> GetConfigList()
        {
            return Directory.EnumerateFiles(ConfigPath, "*.json");
        }

        public string LoadConfig(string id)
        {
            return File.ReadAllText(Path.Combine(ConfigPath, id + ".json"));
        }
    }
}
