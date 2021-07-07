using System.Collections.Generic;
using System.Linq;
using MyLab.ConfigServer.Shared;

namespace MyLab.ConfigServer.Server.Tools
{
    class SecretsAnalyzer
    {
        private readonly string[] _resolvedKeys;

        public SecretsAnalyzer(ISecretsProvider secretsProvider)
        {
            _resolvedKeys = secretsProvider.Provide().Keys.ToArray();
        }

        public IEnumerable<ConfigSecret> GetSecrets(ConfigDocument confDoc)
        {
            foreach (var secret in confDoc.GetSecrets())
            {
                yield return new ConfigSecret
                {
                    FieldPath = secret.Path,
                    SecretKey = secret.Key,
                    Resolved = _resolvedKeys.Contains(secret.Key)
                };
            }
        }
    }
}