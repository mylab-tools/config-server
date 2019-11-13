﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConfigService.Models;
using ConfigService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConfigService.Controllers
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
                    Content = await ConfigProvider.LoadOverride(id)
                };
                return View(model);
            }
    }
}