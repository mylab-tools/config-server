using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MyLab.ConfigServer.Tools
{
    class ConfigBuilder
    {
        public IIncludesProvider IncludesProvider { get; }

        public bool PrettyJson { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="ConfigBuilder"/>
        /// </summary>
        public ConfigBuilder(IIncludesProvider includesProvider)
        {
            IncludesProvider = includesProvider;
        }

        public async Task<string> Build(string originStr, string overridingStr)
        {
            var originXml = JsonConvert.DeserializeXNode(originStr, "root");

            var includeProc = new IncludeProcessor(IncludesProvider);
            await includeProc.ResolveIncludes(originXml);

            if (overridingStr != null)
            {
                var op = new OverrideProcessor();

                var overrideXml = JsonConvert.DeserializeXNode(overridingStr, "root");
                op.Override(originXml, overrideXml);
            }

            return JsonConvert.SerializeXNode(originXml, 
                PrettyJson 
                    ? Formatting.Indented
                    : Formatting.None, 
                true);
        }
    }
}
