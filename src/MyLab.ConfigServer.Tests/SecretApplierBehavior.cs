using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MyLab.ConfigServer.Tools;
using Xunit;

namespace MyLab.ConfigServer.Tests
{
    public class SecretApplierBehavior
    {
        [Fact]
        public void ShouldApplySecrets()
        {
            //Arrange
            var applier = SecretApplier.FromJson("[{ \"key\": \"some-secret\", \"value\": \"some-val\" }]");
            var config = "{ \"secret\": \"[secret:some-secret]\" }";

            //Act
            var configWithSecret = applier.ApplySecrets(config);

            //Assert
            Assert.Equal("{ \"secret\": \"some-val\" }", configWithSecret);
        }


        [Fact]
        public void ShouldProvideUnresolvedSecrets()
        {
            //Arrange
            var config = "{ \"secret\": \"[secret:some-secret2]\" }";

            //Act
            var unresolvedSecrets = SecretApplier.GetUnresolvedSecrets(config)
                .ToArray();

            //Assert
            Assert.NotNull(unresolvedSecrets);
            Assert.Single(unresolvedSecrets);
            Assert.Equal("secret", unresolvedSecrets[0].FieldPath);
            Assert.Equal("some-secret2", unresolvedSecrets[0].SecretKey);
        }
    }
}
