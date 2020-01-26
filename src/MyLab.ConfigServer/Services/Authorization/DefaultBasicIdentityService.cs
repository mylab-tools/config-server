using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MyLab.ConfigServer.Services.Authorization
{
    class DefaultBasicIdentityService : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IAuthorizationService _authorizationService;

        public DefaultBasicIdentityService(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IAuthorizationService authorizationService)
            : base(options, logger, encoder, clock)
        {
            _authorizationService = authorizationService;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return Task.FromResult(AuthenticateResult.Fail("Missing Authorization Header"));

            string user;
            string pass;
            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(new[] { ':' }, 2);
                user = credentials[0];
                pass = credentials[1];
            }
            catch
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
            }

            if (!_authorizationService.Authorize(user,pass))
                return Task.FromResult(AuthenticateResult.Fail("User-Password pair not found"));

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, user),
                new Claim(ClaimTypes.Name, user)
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}