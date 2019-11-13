using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConfigService.Models;
using ConfigService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConfigService.Controllers
{
    [Route("includes")]
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
                Content = await ConfigProvider.LoadInclude(id)
            };
            return View(model);
        }
    }
}