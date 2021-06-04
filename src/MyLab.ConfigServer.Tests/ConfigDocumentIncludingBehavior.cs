using System;
using System.Threading.Tasks;
using MyLab.ConfigServer.Tools;
using Newtonsoft.Json;
using Xunit;

namespace MyLab.ConfigServer.Tests
{
    public class ConfigDocumentIncludingBehavior
    {
        private const string SrcConfig =
            "{" +
                "/*include: include-1*/" +
                "\"IncludeConflict\":\"from-base-val\"" +
            "}";

        [Fact]
        public async Task ShouldResolveIncludes()
        {
            //Arrange
            var doc = ConfigDocument.Load(SrcConfig);

            //Act
            await doc.ApplyIncludes(new TestIncludeProvider());

            var res = TestModel.Create(doc);

            //Assert
            Assert.Equal("from-include-val", res.Included);

        }

        [Fact]
        public async Task ShouldUseBaseValueWhenConflict()
        {
            //Arrange
            var doc = ConfigDocument.Load(SrcConfig);

            //Act
            await doc.ApplyIncludes(new TestIncludeProvider());

            var res = TestModel.Create(doc);

            //Assert
            Assert.Equal("from-base-val", res.IncludeConflict);
        }

        class TestModel
        {
            public string Included { get; set; }
            public string IncludeConflict { get; set; }

            public static TestModel Create(ConfigDocument confDoc)
            {
                return JsonConvert.DeserializeObject<TestModel>(confDoc.Serialize(false));
            }
        }

        class TestIncludeProvider : IIncludesProvider
        {
            public Task<ConfigDocument> GetInclude(string id)
            {
                string config = "{" +
                                    "\"Included\":\"from-include-val\"," +
                                    "\"IncludeConflict\":\"from-include-val\"" +
                                "}";
                return Task.FromResult(ConfigDocument.Load(config));
            }
        }
    }
}
