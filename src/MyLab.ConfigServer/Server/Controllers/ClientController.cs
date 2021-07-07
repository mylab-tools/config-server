using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyLab.ConfigServer.Server.Models;
using MyLab.ConfigServer.Server.Services.Authorization;
using MyLab.ConfigServer.Shared;

namespace MyLab.ConfigServer.Server.Controllers
{
    [Route("api/v2/clients")]
    public class ClientController : Controller
    {
        public IClientsProvider ClientsProvider { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="OverrideController"/>
        /// </summary>
        public ClientController(IClientsProvider clientsProvider)
        {
            ClientsProvider = clientsProvider;
        }

        public async Task<IActionResult> Index()
        {
            var authItems = await ClientsProvider.ProvideAsync();

            return Ok(new ClientsStorageViewModel
            {
                Clients = authItems
                    .Select(ii => ii.Login)
                    .ToArray()
            });
        }
    }
}