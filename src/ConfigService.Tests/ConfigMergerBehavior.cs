using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ConfigService.Tests
{
    public class ConfigMergerBehavior
    {
        [Fact]
        public void ShouldOverrideConfig()
        {
            //Assert
            Assert.Equal("NewVal", TestTools.Foo.ParamForOverride);
        }

        [Fact]
        public void ShouldHideSecret()
        {
            //Assert
            Assert.Equal(ConfigConstants.SecretHideText, TestTools.FooWithoutSecret.SecretParam);
        }

        [Fact]
        public void ShouldProvideSecret()
        {
            //Assert
            Assert.Equal("this is a secret", TestTools.Foo.SecretParam);
        }
    }
}
