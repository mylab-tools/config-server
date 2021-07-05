using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyLab.ConfigServer.Models;
using MyLab.ConfigServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace MyLab.ConfigServer.Controllers
{
    [Route("overrides")]
    public class OverrideController : Controller
    {
        public IConfigProvider ConfigProvider { get; }

            /// <summary>
            /// Initializes a new instance of <see cref="OverrideController"/>
            /// </summary>
            public OverrideController(IConfigProvider configProvider)
        {
            ConfigProvider = configProvider;
        }

            public IActionResult Index()
            {
                var configClients = ConfigProvider
                    .GetOverrideList()
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
                    ConfigInfo = await ConfigProvider.LoadOverride(id)
                };
                return View(model);
            }
    }
}