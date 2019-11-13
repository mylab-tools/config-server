using System.Xml.Linq;
using Newtonsoft.Json;

namespace ConfigService.Tests
{
    static class TestTools
    {
        public static XDocument LoadXDoc(string jsonSrc)
        {
            return JsonConvert.DeserializeXNode(jsonSrc, "root");
        }

        public static FooModel XDocToFoo(XDocument xDoc)
        {
            return JsonConvert.DeserializeObject<FooModel>(JsonConvert.SerializeXNode(xDoc, Formatting.None, true));
        }
    }
}