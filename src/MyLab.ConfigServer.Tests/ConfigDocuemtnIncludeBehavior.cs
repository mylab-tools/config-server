using System.Linq;
using MyLab.ConfigServer.Tools;
using Xunit;
using Xunit.Abstractions;

namespace MyLab.ConfigServer.Tests
{
    public class ConfigDocumentIncludeBehavior
    {
        private readonly ITestOutputHelper _output;

        public ConfigDocumentIncludeBehavior(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]

        public void ShouldInclude()
        {
            //Arrange
            var srcDoc = ConfigDocument.Load("{/*include:123*/\"foo\": \"bar\"}");
            var includeDoc = ConfigDocument.Load("{\"baz\":\"quux\"}");

            _output.WriteLine("Source:");
            _output.WriteLine(srcDoc.Serialize(true));
            _output.WriteLine("");
            _output.WriteLine("Include:");
            _output.WriteLine(includeDoc.Serialize(true));
            _output.WriteLine("");

            var includes = srcDoc.GetIncludes().ToArray();

            //Act
            includes.First().Resolve(includeDoc);

            _output.WriteLine("Result:");
            _output.WriteLine(srcDoc.Serialize(true));

            //Assert
            Assert.Single(includes);
            Assert.Equal("{\"foo\":\"bar\",\"baz\":\"quux\"}", srcDoc.Serialize(false));
        }
    }
}
