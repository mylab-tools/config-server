using System;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ConfigService;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Xunit.Abstractions;

namespace MyLab.ConfigServer.FuncTests
{
    public class ApiControllerBehavior : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly ITestOutputHelper _output;

        public ApiControllerBehavior(WebApplicationFactory<Startup> factory, ITestOutputHelper output)
        {
            _factory = factory;
            _output = output;
        }

        [Fact]
        public async Task ShouldFailedIfNotAuthorized()
        {
            //Arrange
            var client = _factory.CreateClient();

            //Act
            var resp = await client.GetAsync("api/config");

            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task ShouldFailedWhenWrongAuthHeaderScheme()
        {
            //Arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "blablabla");

            //Act
            var resp = await client.GetAsync("api/config");

            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Theory]
        [InlineData(HttpStatusCode.OK, "right-pass")]
        [InlineData(HttpStatusCode.Unauthorized, "wrong-pass")]
        public async Task ShouldCheckBasicAuth(HttpStatusCode expectedStatusCode, string password)
        {
            //Arrange
            var client = _factory.CreateClient();
            var hValue = Convert.ToBase64String(Encoding.UTF8.GetBytes("foo" + ":" + password));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", hValue);

            //Act
            var resp = await client.GetAsync("api/config");

            //Assert
            Assert.Equal(expectedStatusCode, resp.StatusCode);

            _output.WriteLine(await resp.Content.ReadAsStringAsync());
        }
    }
}
