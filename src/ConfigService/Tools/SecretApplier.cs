using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ConfigService.Tools
{
    class SecretApplier
    {
        private const string RegExpr = "\"\\[secret:(?<skey>[\\w\\-\\d]+)\\]\""; 
        private const string ValueRegExpr = "\\[secret:(?<skey>[\\w\\-\\d]+)\\]"; 

        private readonly IDictionary<string, string> _secretMap;

        public SecretApplier(IDictionary<string, string> secretMap)
        {
            _secretMap = secretMap ?? throw new ArgumentNullException(nameof(secretMap));
        }
        public static async Task<SecretApplier> FromFileAsync(string filename)
        {
            var strData = await File.ReadAllTextAsync(filename);
            return FromJson(strData);
        }

        public static SecretApplier FromJson(string json)
        {
            var items = JsonConvert.DeserializeObject<ConfigSecretItem[]>(json);
            return new SecretApplier(items.ToDictionary(itm => itm.Key, itm => itm.Value));
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

        public static IEnumerable<UnresolvedSecret> GetUnresolvedSecrets(string strData)
        {
            var xmlJson = JsonConvert.DeserializeXNode(strData);

            foreach (var descendant in xmlJson.Descendants())
            {
                var match = Regex.Match(descendant.Value, ValueRegExpr);
                if (!match.Success)
                    continue;

                yield return new UnresolvedSecret
                {
                    FieldPath = XElementPathProvider.Provide(descendant).TrimStart('/'),
                    SecretKey = match.Groups["skey"].Value
                };
            }
        }
    }

    class UnresolvedSecret
    {
        public string FieldPath { get; set; }
        public string SecretKey { get; set; }
    }

    class ConfigSecretItem
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
