using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MyLab.ConfigServer.Services;
using MyLab.ConfigServer.Services.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyLab.ConfigServer.Tools;
using MyLab.Syslog;

namespace MyLab.ConfigServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        ///public IHostingEnvironment CurrentEnvironment { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            var contentRoot = Path.Combine(
                Configuration.GetValue<string>(WebHostDefaults.ContentRootKey),
#if DEV
                    "DevResources"
#else
                    "Resources"
#endif
                );

            var secretsFilePath = Path.Combine(contentRoot, "secrets.json");
            var secretsProvider = DefaultSecretsProvider.LoadFromFile(secretsFilePath);
            var secretsApplier = new SecretApplier(secretsProvider);
            var secretAnalyzer = new SecretsAnalyzer(secretsProvider);

            services.AddSingleton<IConfigProvider>(new DefaultConfigProvider(contentRoot, secretsApplier, secretAnalyzer));

            var clientsFile = Path.Combine(contentRoot, "clients.json");
            var clientsProvider = new FileBasedClientsProvider(clientsFile);

            services.AddSingleton<IClientsProvider>(clientsProvider);
            services.AddSingleton<IAuthorizationService>(new AuthorizationService(clientsProvider));


            services.AddAuthentication()
                .AddScheme<AuthenticationSchemeOptions, DefaultBasicIdentityService>(
                    BasicAuthSchemaName.Name, null);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);


            services.Configure<SyslogLoggerOptions>(Configuration.GetSection("Logging:Syslog"));
            services.AddLogging(b => b
                .AddSyslog()
                .AddConsole());

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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
            app.UseCookiePolicy();

            app.Use((ctx, next) =>
            {
                ctx.Request.PathBase = Configuration["BaseAddress"];
                return next();
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "api",
                    template: "api/{controller}");
            });
        }
    }
}
