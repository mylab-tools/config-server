using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
            var secretProvider = new DefaultSecretsProvider("[{ \"key\": \"some-secret\", \"value\": \"some-val\" }]");
            var applier = new SecretApplier(secretProvider);
            var config = "{ \"secret\": \"[secret:some-secret]\" }";

            //Act
            var configWithSecret = applier.ApplySecrets(config);

            //Assert
            Assert.Equal("{ \"secret\": \"some-val\" }", configWithSecret);
        }
    }
}
