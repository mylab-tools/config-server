using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ConfigService.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;

namespace ConfigService.Tests
{
    static class TestTools
    {
        private static readonly Lazy<FooModel> _foo;
        private static readonly Lazy<FooModel> _fooWithoutSecret;
        public static FooModel Foo => _foo.Value;
        public static FooModel FooWithoutSecret => _fooWithoutSecret.Value;

        static TestTools()
        {
            _foo = new Lazy<FooModel>( ()=> Load(false));
            _fooWithoutSecret = new Lazy<FooModel>( ()=> Load(true));
        }

        static FooModel Load(bool hideSecrets)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "ConfigFiles");
            var p = new DefaultConfigProvider(basePath);
            var str = p.LoadConfig("foo", hideSecrets, false).Result;

            return JsonConvert.DeserializeObject<FooModel>(str);
        }
    }
}
