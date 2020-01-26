using System.Collections.Generic;
using System.Linq;

namespace MyLab.ConfigServer.Services.Authorization
{
    public interface IAuthorizationService
    {
        bool Authorize(string login, string pass);
    }

    class AuthorizationService : IAuthorizationService
    {
        private readonly Dictionary<string, string> _clients;

        public AuthorizationService(IClientsProvider clientsProvider)
        {
            _clients = clientsProvider.Provide().ToDictionary(ii => ii.Login, ii => ii.Secret);
        }

        public bool Authorize(string login, string pass)
        {
            return _clients.TryGetValue(login, out var secret) && secret == pass;
        }
    }
}
