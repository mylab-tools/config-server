using System.Collections.Generic;
using System.Linq;
using MyLab.ConfigServer.Server.Services;
using MyLab.ConfigServer.Server.Tools;
using Newtonsoft.Json;
using Xunit;

namespace UnitTests
{
    public class ConfigDocumentBehavior
    {
        private const string SrcConfig =
            "{" +
                "\"ParamForOverride\":\"OldVal\"," +
                "\"InnerObject\":" +
                "{" +
                    "\"Bar\":\"BarVal\"" +
                "}" +
            "}";

        private const string OverrideConfig = 
            "{" +
                "\"ParamForOverride\":\"NewVal\"," +
                "\"InnerObject\":" +
                "{" +
                    "\"Bar\":\"NewBarVal\"" +
                "}" +
            "}";

        [Fact]
        public void ShouldOverride()
        {
            //Arrange
            var originDoc = ConfigDocument.Load(SrcConfig);
            var overrideDoc = ConfigDocument.Load(OverrideConfig);
            var overrides = overrideDoc.CreateOverrides();

            //Act
            originDoc.ApplyOverrides(overrides);

            var res = TestModel.Create(originDoc);

            //Assert
            Assert.Equal("NewVal", res.ParamForOverride);
        }

        [Fact]
        public void ShouldOverrideSubElements()
        {
            //Arrange
            var originDoc = ConfigDocument.Load(SrcConfig);
            var overrideDoc = ConfigDocument.Load(OverrideConfig);
            var overrides = overrideDoc.CreateOverrides();

            //Act
            originDoc.ApplyOverrides(overrides);

            var res = TestModel.Create(originDoc);

            //Assert
            Assert.Equal("NewBarVal", res.InnerObject.Bar);
        }

        [Fact]
        public void ShouldApplySecrets()
        {
            //Arrange
            var secretProvider = new TestSecretsProvider("[{ \"key\": \"some-secret\", \"value\": \"some-val\" }]");
            var config = ConfigDocument.Load("{\"secret\":\"[secret:some-secret]\"}");

            //Act
            config.ApplySecrets(secretProvider);

            //Assert
            Assert.Equal("{\"secret\":\"some-val\"}", config.Serialize(false));
        }

        class TestModel
        {
            public string ParamForOverride { get; set; }

            public TestInnerModel InnerObject { get; set; }

            public static TestModel Create(ConfigDocument confDoc)
            {
                return JsonConvert.DeserializeObject<TestModel>(confDoc.Serialize(false));
            }
        }

        class TestSecretsProvider: ISecretsProvider
        {
            private readonly string _secretsJson;

            public TestSecretsProvider(string secretsJson)
            {
                _secretsJson = secretsJson;
            }
            public IDictionary<string, string> Provide()
            {
                var items = JsonConvert.DeserializeObject<DefaultSecretsProvider.ConfigSecretItem[]>(_secretsJson);
                return items.ToDictionary(itm => itm.Key, itm => itm.Value);
            }
        }

        public class TestInnerModel
        {
            public string Bar { get; set; }
        }
    }
}