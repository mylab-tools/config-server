using System;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ConfigService.Models;
using ConfigService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConfigService.Controllers
{
    [Route("api/config")]
    public class ApiController : Controller
    {
        public IConfigProvider ConfigProvider { get; }
        public IAuthorizationService AuthorizationService { get; }

        /// <summary>
        /// Initializes a new instance of <see cref="ApiController"/>
        /// </summary>
        public ApiController(IConfigProvider configProvider, IAuthorizationService authorizationService)
        {
            ConfigProvider = configProvider;
            AuthorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            if (!Request.Headers.TryGetValue("Authorization", out var h))
                return Unauthorized();

            var authHeader = AuthenticationHeaderValue.Parse(h);

            if (authHeader.Scheme != "Basic")
                return Forbid("Basic");

            var basic = ExtractBasic(authHeader);

            if (!AuthorizationService.Authorize(basic.Login, basic.Pass))
                return Forbid();

            return Ok(await ConfigProvider.LoadConfig(basic.Login, true, true));
        }

        (string Login, string Pass) ExtractBasic(AuthenticationHeaderValue hValue)
        {
            var credentialBytes = Convert.FromBase64String(hValue.Parameter);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' },2);

            return (credentials[0], credentials[1]);
        }
    }
}