using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyLab.ConfigServer.Server.Models;
using MyLab.ConfigServer.Server.Services;
using MyLab.ConfigServer.Shared;

namespace MyLab.ConfigServer.Server.Controllers
{
    [Route("api/v2/base-configs")]
    public class BaseConfigsController : Controller
    {
        public IConfigProvider ConfigProvider { get; }

            /// <summary>
            /// Initializes a new instance of <see cref="BaseConfigsController"/>
            /// </summary>
            public BaseConfigsController(IConfigProvider configProvider)
        {
            ConfigProvider = configProvider;
        }

        public IActionResult Index()
        {
            var configClients = ConfigProvider
                .GetConfigList()
                .OrderBy(n => n.Name)
                .ToArray();
            return Ok(new ConfigStorageViewModel
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
                ConfigInfo = await ConfigProvider.LoadConfigBase(id)
            };
            return Ok(model);
        }
    }
}