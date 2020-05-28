using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MyLab.ConfigServer.Services.Authorization
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
        private AuthorizationItem[] _data;
        private DateTime _lastUpdate;

        public FileBasedClientsProvider(string filename)
        {
            _filename = filename;
        }

        public async Task<IEnumerable<AuthorizationItem>> ProvideAsync()
        {
            var file = new FileInfo(_filename);

            if (file.Exists)
            {
                if (file.LastWriteTime > _lastUpdate)
                {
                    using (var strm = file.OpenRead())
                    using (var rdr = new StreamReader(strm))
                    {
                        var json = await rdr.ReadToEndAsync();
                     
                        _data = JsonConvert.DeserializeObject<AuthorizationItem[]>(json);
                        _lastUpdate = DateTime.Now;
                    }
                }
            }
            
            return _data;
        }
    }
}