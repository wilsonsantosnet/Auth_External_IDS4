using Common.API;
using Common.Domain.Base;
using Common.Domain.Model;
using IdentityServer4;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Seed.CrossCuting;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Sso.Server.Api
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }
        private readonly IHostingEnvironment _env;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                 .SetBasePath(env.ContentRootPath)
                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                 .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                 .AddEnvironmentVariables();

            Configuration = builder.Build();
            _env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
        
            var cns =
             Configuration
                .GetSection("ConfigConnectionString:Default").Value;


            services.AddIdentityServer(optionsConfig())
                .AddSigningCredential(GetRSAParameters())
                .AddCustomTokenRequestValidator<ClientCredentialRequestValidator>()
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryClients(Config.GetClients(Configuration.GetSection("ConfigSettings").Get<ConfigSettingsBase>()));
                

            //Configurations
            services.Configure<ConfigSettingsBase>(Configuration.GetSection("ConfigSettings"));
            services.Configure<ConfigConnectionStringBase>(Configuration.GetSection("ConfigConnectionString"));
            //Container DI
            services.AddScoped<CurrentUser>();
            services.AddScoped<IUserCredentialServices, UserCredentialServices>();
            services.AddScoped<IResourceOwnerPasswordValidator, ResourceOwnerPasswordValidator>();
            services.AddScoped<ICustomTokenRequestValidator, ClientCredentialRequestValidator>();

            services.AddScoped<ExternalProviderAuth>();

            services.AddSingleton<IConfiguration>(Configuration);
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConfiguration(Configuration.GetSection("Logging"));
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
            });


            services.AddAuthentication()
           .AddMicrosoftAccount(options => {
               options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
               options.ClientId = "073c55fb-622f-4934-902e-b3687a6108d3";
               options.ClientSecret = "l8I_LV46vuccYNTMc.s.q~459";
           });

            services.AddAuthentication()
            .AddGoogle(options =>
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                options.ClientId = "857854978384-2k3bqg4ak6cha5fhs9q0uuis51botbtg.apps.googleusercontent.com";
                options.ClientSecret = "HmdWfDhp8ddoNXHDMWF123";
            });
      

            services.AddAuthentication()
            .AddFacebook(options => {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                options.ClientId = "772892103623005";
                options.ClientSecret = "b26a3d6372b6c04b2e1412c789";
            });

            // Add cross-origin resource sharing services Configurations
            var sp = services.BuildServiceProvider();
            var configuration = sp.GetService<IOptions<ConfigSettingsBase>>();
            Cors.Enable(services, configuration.Value.ClientAuthorityEndPoint.ToArray());

            services.AddMvc();


        }

        public void Configure(IApplicationBuilder app,IHostingEnvironment env, ILoggerFactory loggerFactory, IOptions<ConfigSettingsBase> configSettingsBase)
        {

            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            loggerFactory.AddFile(Configuration.GetSection("Logging"));

            app.UseCors("AllowStackOrigin");
            app.UseIdentityServer();

            //app.UseGoogleAuthentication(new GoogleOptions
            //{
            //    AuthenticationScheme = "Google",
            //    SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme,
            //    ClientId = "857854978384-sv33ngtei50k8fn5ea37rcddo08n0ior.apps.googleusercontent.com",
            //    ClientSecret = "x1SWT89gyn5LLLyMNFxEx_Ss"
            //});

            app.UseAuthentication();
            app.AddTokenMiddlewareCustom();
            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
        
        private Action<IdentityServer4.Configuration.IdentityServerOptions> optionsConfig()
        {
            return options => { options.IssuerUri = Configuration.GetSection("ConfigSettings:IssuerUri").Value; };
        }
        
        private X509Certificate2 GetRSAParameters()
        {
            var fileCert = Path.Combine(_env.ContentRootPath, "pfx", "ids4smbasic.pfx");
            if (!File.Exists(fileCert))
                throw new InvalidOperationException("Certificado não encontrado");

            var password = "vm123s456";
            return new X509Certificate2(fileCert, password, X509KeyStorageFlags.Exportable);
        }
    }
}
