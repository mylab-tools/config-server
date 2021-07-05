using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using MyLab.ConfigServer.Services;
using MyLab.ConfigServer.Services.Authorization;
using MyLab.ConfigServer.Tools;
using MyLab.HttpMetrics;
using MyLab.StatusProvider;
using MyLab.Syslog;
using MyLab.WebErrors;
using Newtonsoft.Json;
using Prometheus;

namespace MyLab.ConfigServer
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            Configuration = configuration;
            CurrentEnvironment = env;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment CurrentEnvironment { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
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
            services.AddSingleton<MyLab.ConfigServer.Services.Authorization.IAuthorizationService>(new AuthorizationService(clientsProvider));


            services.AddAuthentication()
                .AddScheme<AuthenticationSchemeOptions, DefaultBasicIdentityService>(
                    BasicAuthSchemaName.Name, null);
            services.Configure<SyslogLoggerOptions>(Configuration.GetSection("Logging:Syslog"));
            services.AddLogging(b => b
                .AddSyslog()
                .AddConsole());

            services.AddUrlBasedHttpMetrics();

            services.AddControllersWithViews(options =>
            {
                options.AddExceptionProcessing();
            }).AddNewtonsoftJson();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseHttpMetrics();
            app.UseUrlBasedHttpMetrics();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapMetrics();
            });

            app.UseStatusApi(serializerSettings: new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                TypeNameHandling = TypeNameHandling.All
            });
        }
    }
}
