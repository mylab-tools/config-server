using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MyLab.ApiClient;

namespace MyLab.ConfigServer.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddApiClients(registrar => registrar.RegisterContract<IConfigServiceV2>(), new ApiClientsOptions
            {
                List = new Dictionary<string, ApiConnectionOptions>
                {
                    {"api", new ApiConnectionOptions { Url = builder.HostEnvironment.BaseAddress }}
                }
            });

            await builder.Build().RunAsync();
        }
    }
}
