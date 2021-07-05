using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MyLab.ConfigServer.Tools
{
    static class ConfigDocumentExtensions
    {
        public static void ApplySecrets(this ConfigDocument doc, ISecretsProvider secretsProvider)
        {
            if (doc == null) throw new ArgumentNullException(nameof(doc));
            if (secretsProvider == null) throw new ArgumentNullException(nameof(secretsProvider));

            var secretMap = secretsProvider.Provide();

            foreach (var secret in doc.GetSecrets())
            {
                if (secretMap.TryGetValue(secret.Key, out var secretVal))
                {
                    secret.Resolve(secretVal);
                }
            }
        }

        public static Task ApplyIncludes(this ConfigDocument doc, IIncludesProvider includeProvider)
        {
            if (doc == null) throw new ArgumentNullException(nameof(doc));
            if (includeProvider == null) throw new ArgumentNullException(nameof(includeProvider));

            return ResolveIncludes(doc, 0, includeProvider);
        }

        static async Task ResolveIncludes(ConfigDocument confDoc, int deepCount, IIncludesProvider includeProvider)
        {
            if (deepCount >= 2) return;

            foreach (var include in confDoc.GetIncludes())
            {
                var includeContent = await includeProvider.GetInclude(include.Link);

                await ResolveIncludes(includeContent, deepCount + 1, includeProvider);

                include.Resolve(includeContent);
            }
        }

        public static void ApplyOverride(this ConfigDocument doc, ConfigDocument ovrd)
        {
            if (doc == null) throw new ArgumentNullException(nameof(doc));
            if (ovrd == null) throw new ArgumentNullException(nameof(ovrd));

            var overrides = ovrd.CreateOverrides();

            doc.ApplyOverrides(overrides);
        }
    }
}
