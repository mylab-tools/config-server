using System;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using MyLab.ApiClient;
using MyLab.ApiClient.Test;
using MyLab.ConfigServer.Server;
using Xunit;
using Xunit.Abstractions;

namespace FuncTests
{
    public class ApiControllerBehavior : IDisposable
    {
        private readonly TestApi<Startup, IConfigService> _api;

        public ApiControllerBehavior(ITestOutputHelper output)
        {
            _api = new TestApi<Startup, IConfigService>
            {
                Output = output,
                HttpClientTuner = client => client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "blablabla")
            };
        }

        [Fact]
        public async Task ShouldFailedIfNotAuthorized()
        {
            //Arrange
            var client = _api.Start();

            //Act
            var resp = await client.Call(s => s.GetConfig());

            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Fact]
        public async Task ShouldFailedWhenWrongAuthHeaderScheme()
        {
            //Arrange
            var authHeader = new AuthenticationHeaderValue("Bearer", "blablabla");
            var client = _api.Start(httpClientTuner: c => c.DefaultRequestHeaders.Authorization = authHeader);

            //Act
            var resp = await client.Call(s => s.GetConfig());

            //Assert
            Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
        }

        [Theory]
        [InlineData(HttpStatusCode.OK, "right-pass")]
        [InlineData(HttpStatusCode.Unauthorized, "wrong-pass")]
        public async Task ShouldCheckBasicAuth(HttpStatusCode expectedStatusCode, string password)
        {
            //Arrange
            var hValue = Convert.ToBase64String(Encoding.UTF8.GetBytes("foo" + ":" + password));
            var authHeader = new AuthenticationHeaderValue("Basic", hValue);
            var client = _api.Start(httpClientTuner: c => c.DefaultRequestHeaders.Authorization = authHeader);

            //Act
            var resp = await client.Call(s => s.GetConfig());

            //Assert
            Assert.Equal(expectedStatusCode, resp.StatusCode);
        }

        public void Dispose()
        {
            _api.Dispose();
        }
    }

    [Api]
    interface IConfigService
    {
        [Get("api/config")]
        Task<string> GetConfig();
    }
}
