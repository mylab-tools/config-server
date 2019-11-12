using ConfigService.Models;
using ConfigService.Services;
using Microsoft.AspNetCore.Identity.UI.V3.Pages.Internal.Account;
using Microsoft.AspNetCore.Mvc;

namespace ConfigService.Controllers
{
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

        public IActionResult Index(string id)
        {
            var model = new ConfigViewModel
            {
                Id = id,
                Content = ConfigProvider.LoadConfig(id)
            };
            return View(model);
        }
    }
}