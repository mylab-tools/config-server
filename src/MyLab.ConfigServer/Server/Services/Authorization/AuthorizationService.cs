using System.Linq;
using System.Threading.Tasks;

namespace MyLab.ConfigServer.Server.Services.Authorization
{
    public interface IAuthorizationService
    {
        Task<bool> AuthorizeAsync(string login, string pass);
    }

    class AuthorizationService : IAuthorizationService
    {
        private readonly IClientsProvider _clientsProvider;

        public AuthorizationService(IClientsProvider clientsProvider)
        {
            _clientsProvider = clientsProvider;
        }

        public async Task<bool> AuthorizeAsync(string login, string pass)
        {
            var itms = await _clientsProvider.ProvideAsync();
            return itms.Any(itm => itm.Login == login && itm.Secret == pass);
        }
    }
}
