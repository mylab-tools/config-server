using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MyLab.ConfigServer.Server.Services.Authorization
{
    public interface IClientsProvider
    {
        Task<IEnumerable<AuthorizationItem>> ProvideAsync();
    }
    public class AuthorizationItem
    {
        public string Login { get; set; }
        public string Secret { get; set; }
    }

    class FileBasedClientsProvider : IClientsProvider
    {
        private readonly string _filename;

        public FileBasedClientsProvider(string filename)
        {
            _filename = filename;
        }

        public async Task<IEnumerable<AuthorizationItem>> ProvideAsync()
        {
            var file = new FileInfo(_filename);

            if (file.Exists)
            {
                using (var strm = file.OpenRead())
                using (var rdr = new StreamReader(strm))
                {
                    var json = await rdr.ReadToEndAsync();

                    return JsonConvert.DeserializeObject<AuthorizationItem[]>(json);
                }
            }

            return Enumerable.Empty<AuthorizationItem>();
        }
    }
}