using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using MyLab.ConfigServer.Server.Services;
using MyLab.ConfigServer.Server.Services.Authorization;
using MyLab.ConfigServer.Server.Tools;
using MyLab.HttpMetrics;
using MyLab.WebErrors;

namespace MyLab.ConfigServer.Server
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            Configuration = configuration;
            CurrentEnvironment = env;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment CurrentEnvironment { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
        {
            var contentRoot = Path.Combine(
                Configuration.GetValue<string>(WebHostDefaults.ContentRootKey),
                CurrentEnvironment.IsDevelopment()
                    ? "DevResources"
                    : "Resources"
            );

            var secretsFilePath = Path.Combine(contentRoot, "secrets.json");
            var secretsProvider = DefaultSecretsProvider.LoadFromFile(secretsFilePath);

            services.AddSingleton<IConfigProvider>(new DefaultConfigProvider(contentRoot, secretsProvider));

            var clientsFile = Path.Combine(contentRoot, "clients.json");
            var clientsProvider = new FileBasedClientsProvider(clientsFile);

            services.AddSingleton<IClientsProvider>(clientsProvider);
            services.AddSingleton<IAuthorizationService>(new AuthorizationService(clientsProvider));


            services.AddAuthentication()
                .AddScheme<AuthenticationSchemeOptions, DefaultBasicIdentityService>(
                    BasicAuthSchemaName.Name, null);

            services.AddUrlBasedHttpMetrics();

            services.AddControllers(opt => opt.AddExceptionProcessing());
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
