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

        private const string IncludeSimple = 
            "{" +
                "\"Included\":\"from-include-val\"," +
                "\"IncludeConflict\":\"from-include-val\"" +
            "}";

        [Fact]
        public async Task ShouldResolveIncludes()
        {
            //Arrange
            var doc = ConfigDocument.Load(SrcConfig);
            var includeProvider = new TestIncludeProvider(IncludeSimple);

            //Act
            await doc.ApplyIncludes(includeProvider);

            var res = TestModel.Create(doc);

            //Assert
            Assert.Equal("from-include-val", res.Included);

        }

        [Fact]
        public async Task ShouldUseBaseValueWhenConflict()
        {
            //Arrange
            var doc = ConfigDocument.Load(SrcConfig);
            var includeProvider = new TestIncludeProvider(IncludeSimple);

            //Act
            await doc.ApplyIncludes(includeProvider);

            var res = TestModel.Create(doc);

            //Assert
            Assert.Equal("from-base-val", res.IncludeConflict);
        }

        [Fact]
        public async Task ShouldIncludeArray()
        {
            //Arrange
            const string includeArray =
                "{" +
                    "\"IncludedArray\":[\"foo\",\"bar\"]," +
                    "\"IncludeConflict\":\"from-include-val\"" +
                "}";
            var doc = ConfigDocument.Load(SrcConfig);
            var includeProvider = new TestIncludeProvider(includeArray);

            //Act
            await doc.ApplyIncludes(includeProvider);

            var res = TestModel.Create(doc);

            //Assert
            Assert.NotNull(res.IncludedArray);
            Assert.Equal(2, res.IncludedArray.Length);
            Assert.Equal("foo", res.IncludedArray[0]);
            Assert.Equal("bar", res.IncludedArray[1]);
        }

        [Fact]
        public async Task ShouldIncludeOneItemArray()
        {
            //Arrange
            const string includeArray =
                "{" +
                    "\"IncludedArray\":[\"foo\"]," +
                    "\"IncludeConflict\":\"from-include-val\"" +
                "}";
            var doc = ConfigDocument.Load(SrcConfig);
            var includeProvider = new TestIncludeProvider(includeArray);

            //Act
            await doc.ApplyIncludes(includeProvider);

            var res = TestModel.Create(doc);

            //Assert
            Assert.NotNull(res.IncludedArray);
            Assert.Single(res.IncludedArray);
            Assert.Equal("foo", res.IncludedArray[0]);
        }

        class TestModel
        {
            public string Included { get; set; }
            public string IncludeConflict { get; set; }

            public string[] IncludedArray { get; set; }

            public static TestModel Create(ConfigDocument confDoc)
            {
                return JsonConvert.DeserializeObject<TestModel>(confDoc.Serialize(false));
            }
        }

        class TestIncludeProvider : IIncludesProvider
        {
            private readonly string _cfg;

            public TestIncludeProvider(string cfg)
            {
                _cfg = cfg;
            }

            public Task<ConfigDocument> GetInclude(string id)
            {
                return Task.FromResult(ConfigDocument.Load(_cfg));
            }
        }
    }
}
