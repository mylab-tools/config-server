using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Options;

namespace ConfigService.Services
{
    interface IAuthorizationService
    {
        bool Authorize(string login, string pass);
    }

    public class AuthorizationOptions
    {
        public IReadOnlyDictionary<string, string> LoginToSecretHashMap { get; set; }
        public string Salt { get; set; }
    }

    class AuthorizationService : IAuthorizationService
    {
        private readonly AuthorizationOptions _options;

        public AuthorizationService(AuthorizationOptions options)
        {
            _options = options;
        }

        public AuthorizationService(IOptions<AuthorizationOptions> options)
            : this(options.Value)
        {
        }

        public bool Authorize(string login, string pass)
        {
            if (!_options.LoginToSecretHashMap.TryGetValue(login, out var secretHash))
                return false;

            var curSecret = _options.Salt + pass;
            var curSecretBin = Encoding.UTF8.GetBytes(curSecret);
            var curSecretHash = MD5.Create().ComputeHash(curSecretBin);
            var curSecretHashBase64 = Convert.ToBase64String(curSecretHash);

            return curSecretHashBase64 == secretHash;
        }
    }
}
