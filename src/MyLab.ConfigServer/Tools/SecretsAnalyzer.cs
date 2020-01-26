using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MyLab.ConfigServer.Tools
{
    class SecretsAnalyzer
    {
        private const string ValueRegExpr = "\\[secret:(?<skey>[\\w\\-\\d]+)\\]";

        private readonly string[] _resolvedKeys;

        public SecretsAnalyzer(ISecretsProvider secretsProvider)
        {
            _resolvedKeys = secretsProvider.Provide().Keys.ToArray();
        }

        public IEnumerable<ConfigSecret> GetSecrets(string configStrData)
        {
            var xmlJson = JsonConvert.DeserializeXNode(configStrData, "root");

            foreach (var descendant in xmlJson.Descendants().Where(d => !d.HasElements))
            {
                var match = Regex.Match(descendant.Value, ValueRegExpr);
                if (!match.Success)
                    continue;

                var secretKey = match.Groups["skey"].Value;

                yield return new ConfigSecret
                {
                    FieldPath = XElementPathProvider.Provide(descendant),
                    SecretKey = secretKey,
                    Resolved = _resolvedKeys.Contains(secretKey)
                };
            }
        }
    }

    public class ConfigSecret
    {
        public string FieldPath { get; set; }
        public string SecretKey { get; set; }
        public bool Resolved { get; set; }
    }
}