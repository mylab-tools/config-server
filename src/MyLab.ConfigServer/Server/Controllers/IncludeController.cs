using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyLab.ConfigServer.Server.Models;
using MyLab.ConfigServer.Server.Services;
using MyLab.ConfigServer.Shared;

namespace MyLab.ConfigServer.Server.Controllers
{
    [Route("api/v2/includes")]
    public class IncludeController : Controller
    {
        public IConfigProvider ConfigProvider { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="IncludeController"/>
        /// </summary>
        public IncludeController(IConfigProvider configProvider)
        {
            ConfigProvider = configProvider;
        }

        public IActionResult Index()
        {
            var includes = ConfigProvider
                .GetIncludeList()
                .OrderBy(n => n)
                .ToArray();
            return View(new ConfigStorageViewModel
            {
                Configs = includes
            });
        }

        [Route("{id}")]
        public async Task<IActionResult> Details(string id)
        {
            var model = new ConfigViewModel
            {
                Id = id,
                ConfigInfo = await ConfigProvider.LoadInclude(id)
            };
            return View(model);
        }
    }
}