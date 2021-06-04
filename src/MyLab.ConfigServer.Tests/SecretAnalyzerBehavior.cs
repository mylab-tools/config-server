using System.Collections.Generic;
using System.Linq;
using Moq;
using MyLab.ConfigServer.Tools;
using Xunit;

namespace MyLab.ConfigServer.Tests
{
    public class SecretAnalyzerBehavior
    {
        static ConfigDocument Config = ConfigDocument.Load("{ \"ResolvedSecret\": \"[secret:some-secret1]\", \"UnresolvedSecret\": \"[secret:some-secret2]\" }");

        private readonly ISecretsProvider _secretsProvider;
        static readonly string[] ResolvedSecrets= new []{ "some-secret1" };

        public SecretAnalyzerBehavior()
        {
            var mock = new Mock<ISecretsProvider>();
            mock
                .Setup(sp => sp.Provide())
                .Returns(() => new Dictionary<string, string> { { "some-secret1", "some-val1" } });
            _secretsProvider = mock.Object;
        }

        [Fact]
        public void ShouldDetectUnresolvedSecrets()
        {
            //Arrange
            var analyzer = new SecretsAnalyzer(_secretsProvider);

            //Act
            var secrets = analyzer.GetSecrets(Config).ToArray();
            var unresolvedSecret = secrets.FirstOrDefault(s => s.FieldPath == "/UnresolvedSecret");

            //Assert
            Assert.NotNull(unresolvedSecret);
            Assert.Equal("some-secret2", unresolvedSecret.SecretKey);
            Assert.False(unresolvedSecret.Resolved);
        }

        [Fact]
        public void ShouldDetectResolvedSecrets()
        {
            //Arrange
            var analyzer = new SecretsAnalyzer(_secretsProvider);

            //Act
            var secrets = analyzer.GetSecrets(Config).ToArray();
            var resolvedSecret = secrets.FirstOrDefault(s => s.FieldPath == "/ResolvedSecret");

            //Assert
            Assert.NotNull(resolvedSecret);
            Assert.Equal("some-secret1", resolvedSecret.SecretKey);
            Assert.True(resolvedSecret.Resolved);
        }
    }
}