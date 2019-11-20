using System.Collections.Generic;
using ConfigService.Services;
using Xunit;

namespace ConfigService.Tests
{
    public class AuthorizationServiceBehavior
    {

        [Fact]
        public void ShouldFailIfLoginNotFound()
        {
            //Arrange
            var service = new AuthorizationService(new AuthorizationOptions
            {
                LoginToSecretHashMap = new Dictionary<string, string>(),
                Salt = "salt"
            });

            //Act
            var isAuth = service.Authorize("not-found", "pass");

            //Assert
            Assert.False(isAuth);
        }

        [Theory]
        [InlineData("right-pass", true)]
        [InlineData("wrong-pass", false)]
        public void ShouldAuthorize(string pass, bool expectedResult)
        {
            //Arrange
            var service = new AuthorizationService(new AuthorizationOptions
            {
                LoginToSecretHashMap = new Dictionary<string, string>
                {
                    {"foo", "U6mViwX1GlCe967L1FEehw==" }
                },
                Salt = "salt"
            });

            //Act
            var isAuth = service.Authorize("foo", pass);

            //Assert
            Assert.Equal(expectedResult, isAuth);
        }
    }
}
