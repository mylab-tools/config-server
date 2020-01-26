using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace MyLab.ConfigServer.Services.Authorization
{
    public interface IClientsProvider
    {
        IEnumerable<AuthorizationItem> Provide();
    }
    public class AuthorizationItem
    {
        public string Login { get; set; }
        public string Secret { get; set; }
    }

    class DefaultClientsProvider : IClientsProvider
    {
        private readonly string _json;

        public DefaultClientsProvider(string json)
        {
            _json = json;
        }

        public static DefaultClientsProvider LoadFromFile(string filePath)
        {
            return new DefaultClientsProvider(
                File.Exists(filePath)
                    ? File.ReadAllText(filePath)
                    : null
            );
        }

        public IEnumerable<AuthorizationItem> Provide()
        {
            return _json != null
                ? JsonConvert.DeserializeObject<AuthorizationItem[]>(_json)
                : Enumerable.Empty<AuthorizationItem>();
        }
    }
}