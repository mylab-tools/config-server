using System;
using System.Threading.Tasks;
using ConfigService.Tools;
using Newtonsoft.Json;
using Xunit;

namespace ConfigService.Tests
{
    public class IncludeProcessorBehavior
    {
        [Fact]
        public async Task ShouldResolveIncludes()
        {
            //Arrange
            var doc = TestTools.LoadXDoc(TestFiles.Origin);
            var incP = new IncludeProcessor(new TestIncludeProvider());

            //Act
            await incP.ResolveIncludes(doc);

            var res = TestTools.XDocToFoo(doc);

            //Assert
            Assert.Equal("from-include-val", res.Included);
        }

        [Fact]
        public async Task ShouldUseBaseValueWhenConflict()
        {
            //Arrange
            var doc = TestTools.LoadXDoc(TestFiles.Origin);
            var incP = new IncludeProcessor(new TestIncludeProvider());

            //Act
            await incP.ResolveIncludes(doc);

            var res = TestTools.XDocToFoo(doc);

            //Assert
            Assert.Equal("from-base-val", res.IncludeConflict);
        }

        class TestIncludeProvider : IIncludesProvider
        {
            public Task<string> GetInclude(string id)
            {
                switch (id)
                {
                    case "include-1": return Task.FromResult(TestFiles.Include1);
                    case "include-2": return Task.FromResult(TestFiles.Include2);
                    default: throw new InvalidOperationException($"Unexpected include id: '{id}'");
                }
            }
        }
    }
}
