using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyLab.ConfigServer.Server.Services;
using MyLab.ConfigServer.Server.Services.Authorization;

namespace MyLab.ConfigServer.Server.Controllers
{
    [Route("api/config")]
    public class ResultConfigController : Controller
    {
        public IConfigProvider ConfigProvider { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="ResultConfigController"/>
        /// </summary>
        public ResultConfigController(IConfigProvider configProvider)
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