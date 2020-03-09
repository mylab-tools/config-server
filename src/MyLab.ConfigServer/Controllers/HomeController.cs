using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyLab.ConfigServer.Models;
using MyLab.ConfigServer.Services;

namespace MyLab.ConfigServer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public IConfigProvider ConfigProvider { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="HomeController"/>
        /// </summary>
        public HomeController(IConfigProvider configProvider, ILogger<HomeController> logger)
        {
            _logger = logger;
            ConfigProvider = configProvider;
        }

        public IActionResult Index()
        {
            return RedirectToAction("Index", "Config");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            LogError();

            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private void LogError()
        {
            var exceptionHandlerPathFeature =
                HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            string msg = exceptionHandlerPathFeature?.Error is Exception ex
                ? ex.Message
                : "Request handle error";

            _logger.LogError($"{msg}. Path:'{exceptionHandlerPathFeature?.Path}'");
        }
    }
}
