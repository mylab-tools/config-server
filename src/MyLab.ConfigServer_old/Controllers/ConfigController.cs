using System.Linq;
using System.Threading.Tasks;
using MyLab.ConfigServer.Models;
using MyLab.ConfigServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace MyLab.ConfigServer.Controllers
{
    [Route("configs")]
    public class ConfigController : Controller
    {
        public IConfigProvider ConfigProvider { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="ConfigController"/>
        /// </summary>
        public ConfigController(IConfigProvider configProvider)
        {
            ConfigProvider = configProvider;
        }

        public IActionResult Index()
        {
            var configClients = ConfigProvider
                .GetConfigList()
                .OrderBy(n => n)
                .ToArray();
            return View(new ConfigStorageViewModel
            {
                Configs = configClients
            });
        }

        [Route("{id}")]
        public async Task<IActionResult> Details(string id)
        {
            var model = new ConfigViewModel
            {
                Id = id,
                ConfigInfo = await ConfigProvider.LoadConfigWithoutSecrets(id, true)
            };
            return View(model);
        }
    }
}