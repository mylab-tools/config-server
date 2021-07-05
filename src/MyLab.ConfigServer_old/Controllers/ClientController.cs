using System.Linq;
using System.Threading.Tasks;
using MyLab.ConfigServer.Models;
using Microsoft.AspNetCore.Mvc;
using MyLab.ConfigServer.Services.Authorization;

namespace MyLab.ConfigServer.Controllers
{
    [Route("clients")]
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

            return View(new ClientsStorageViewModel
            {
                Clients = authItems
                    .Select(ii => ii.Login)
                    .ToArray()
            });
        }
    }
}