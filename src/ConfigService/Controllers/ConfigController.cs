using System.Linq;
using System.Threading.Tasks;
using ConfigService.Models;
using ConfigService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConfigService.Controllers
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
                Content = await ConfigProvider.LoadConfigWithoutSecrets(id, true)
            };
            return View(model);
        }
    }
}