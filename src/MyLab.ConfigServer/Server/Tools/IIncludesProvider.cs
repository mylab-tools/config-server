using System.IO;
using System.Threading.Tasks;

namespace MyLab.ConfigServer.Server.Tools
{
    interface IIncludesProvider
    {
        Task<ConfigDocument> GetInclude(string id);
    }

    class DefaultIncludeProvider : IIncludesProvider
    {
        public string BasePath { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="DefaultIncludeProvider"/>
        /// </summary>
        public DefaultIncludeProvider(string basePath)
        {
            BasePath = basePath;
        }
        public async Task<ConfigDocument> GetInclude(string id)
        {
            var filePath = Path.Combine(BasePath, id + ".json");
            var content = File.Exists(filePath) ? 
                await File.ReadAllTextAsync(filePath) :
                await Task.FromResult<string>(null);

            return ConfigDocument.Load(content);
        }
    }
}