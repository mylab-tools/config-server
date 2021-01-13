using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace MyLab.ConfigServer.Tools
{
    class SecretApplier
    {
        private const string RegExpr = "\"\\[secret:(?<skey>[\\w\\-\\d]+)\\]\"";

        private readonly IDictionary<string, string> _secretMap;
        
        public SecretApplier(ISecretsProvider secretsProvider)
        {
            _secretMap = secretsProvider.Provide();
        }

        public string ApplySecrets(string strData)
        {
            if (strData == null) throw new ArgumentNullException(nameof(strData));

            var matchEvaluator = new MatchEvaluator(match =>
            {
                var secretKey = match.Groups["skey"].Value;
                return _secretMap.TryGetValue(secretKey, out var secretVal)
                    ? "\""+secretVal+"\""
                    : match.Value;
            });

            return Regex.Replace(strData, RegExpr, matchEvaluator);
        }
    }
}
