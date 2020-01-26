using System;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using MyLab.ConfigServer.Models;
using MyLab.ConfigServer.Services;
using MyLab.ConfigServer.Services.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IAuthorizationService = MyLab.ConfigServer.Services.Authorization.IAuthorizationService;

namespace MyLab.ConfigServer.Controllers
{
    [Route("api/config")]
    public class ApiController : Controller
    {
        public IConfigProvider ConfigProvider { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="ApiController"/>
        /// </summary>
        public ApiController(IConfigProvider configProvider)
        {
            ConfigProvider = configProvider;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = BasicAuthSchemaName.Name)]
        public async Task<IActionResult> Get()
        {
            var config = await ConfigProvider.LoadConfig(Request.HttpContext.User.Identity.Name, true);
            return Ok(config.Content);
        }
    }
}