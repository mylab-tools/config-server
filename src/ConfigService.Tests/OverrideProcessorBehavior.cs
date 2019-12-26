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
            var p = new OverrideProcessor();

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
            var p = new OverrideProcessor();

            //Act
            p.Override(originDoc, overrideDoc);

            var res = TestTools.XDocToFoo(originDoc);

            //Assert
            Assert.Equal("NewBarVal", res.InnerObject.Bar);
        }

    }
}
