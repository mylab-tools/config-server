using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace ConfigService.Services.Authorization
{
    public interface IAuthorizationService
    {
        bool Authorize(string login, string pass);
    }

    class AuthorizationService : IAuthorizationService
    {
        private readonly Lazy<Dictionary<string, string>> _clients;

        public AuthorizationService(string contentRoot)
        {
            _clients = new Lazy<Dictionary<string, string>>(() =>
            {
                var strData = File.ReadAllText(Path.Combine(contentRoot, "client-list.json"));
                var objModel = JsonConvert.DeserializeObject<AuthorizationItem[]>(strData);

                return objModel.ToDictionary(itm => itm.Login, itm => itm.Secret);
            });
        }

        public bool Authorize(string login, string pass)
        {
            return _clients.Value.TryGetValue(login, out var secret) && secret == pass;
        }
    }

    class AuthorizationItem
    {
        public string Login { get; set; }
        public string Secret { get; set; }
    }
}
