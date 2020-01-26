using System.Linq;
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

        public IActionResult Index()
        {
            return View(new ClientsStorageViewModel
            {
                Clients = ClientsProvider
                    .Provide()
                    .Select(ii => ii.Login)
                    .ToArray()
            });
        }
    }
}