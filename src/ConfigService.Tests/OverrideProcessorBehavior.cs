using ConfigService.Tools;
using Xunit;

namespace ConfigService.Tests
{
    public class OverrideProcessorBehavior
    {
        [Fact]
        public void ShouldOverride()
        {
            //Arrange
            var originDoc = TestTools.LoadXDoc(TestFiles.Origin);
            var overrideDoc = TestTools.LoadXDoc(TestFiles.Override);
            var p = new OverrideProcessor(false);

            //Act
            p.Override(originDoc, overrideDoc);

            var res = TestTools.XDocToFoo(originDoc);

            //Assert
            Assert.Equal("NewVal", res.ParamForOverride);
        }

        [Fact]
        public void ShouldOverrideSubelements()
        {
            //Arrange
            var originDoc = TestTools.LoadXDoc(TestFiles.Origin);
            var overrideDoc = TestTools.LoadXDoc(TestFiles.Override);
            var p = new OverrideProcessor(false);

            //Act
            p.Override(originDoc, overrideDoc);

            var res = TestTools.XDocToFoo(originDoc);

            //Assert
            Assert.Equal("NewBarVal", res.InnerObject.Bar);
        }

        [Theory]
        [InlineData(true, ConfigConstants.SpecifiedSecretHidingText)]
        [InlineData(false, "this is a secret")]
        public void ShouldSupportSecretHidingValue(bool hideSecrets, string expectedMessage)
        {
            //Arrange
            var originDoc = TestTools.LoadXDoc(TestFiles.Origin);
            var overrideDoc = TestTools.LoadXDoc(TestFiles.Override);
            var p = new OverrideProcessor(hideSecrets);

            //Act
            p.Override(originDoc, overrideDoc);

            var res = TestTools.XDocToFoo(originDoc);

            //Assert
            Assert.Equal(expectedMessage, res.SecretParam);
        }
    }
}
