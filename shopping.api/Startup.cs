using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using shopping.api.Models;
using NLog;
using shopping.api.Shared.DefaultException;
using shopping.api.Shared.Services;
using static shopping.api.Shared.Models.Vault;
using Newtonsoft.Json;
using Flurl.Http;
using System.Text.Json.Serialization;
using shopping.api.Extension;

namespace shopping.api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add services for controllers
            services
                .AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                });

            // Adds health check service
            services.AddHealthChecks();

            var appSettings = new AppSettings
            {
                VaultHost = this.Configuration.GetSection("VaultHost").Get<string>(),
                Shopping = this.Configuration.GetSection("Shopping").Get<string>(),
            };

            appSettings.CredentialSetting = this._Credential(appSettings);

            #region Swagger
            // Read Swagger settings
            var swagger = this.Configuration.GetSection("Swagger").Get<SwaggerSettings>();

            // Add Swagger generator
            services.AddSwaggerGen(setupAction =>
            {
                setupAction.SwaggerDoc(
                    swagger.Version,
                    new OpenApiInfo
                    {
                        Title = swagger.Title,
                        Version = swagger.Version,
                        Contact = new OpenApiContact()
                        {
                            Name = swagger.Name,
                            Email = swagger.Email
                        }
                    }
                );
            });
            #endregion

            #region Dependency Injection
            services.ConfigureScopeFacades(appSettings);
            services.ConfigureScopeService(appSettings);
            services.ConfigureScopeRepository(appSettings);
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                var swaggerSettings = this.Configuration.GetSection("Swagger").Get<SwaggerSettings>();
                var serviceName = this.Configuration.GetValue<string>("Consul:Discovery:ServiceName");

                // Use Swagger middleware
                app.UseSwagger(c =>
                {
                    c.PreSerializeFilters.Add((swagger, httpReq) =>
                    {
                        var forwardedHost = httpReq.Headers["X-Forwarded-Host"].FirstOrDefault();

                        if (string.IsNullOrEmpty(forwardedHost))
                            swagger.Servers = new List<OpenApiServer>
                            {
                                new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}" }
                            };
                        else
                            swagger.Servers = new List<OpenApiServer>
                            {
                                new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Headers["X-Forwarded-Host"].FirstOrDefault()}/{serviceName}/" }
                            };
                    });
                });

                // Use Swagger UI middleware
                app.UseSwaggerUI(
                    c =>
                    {
                        c.SwaggerEndpoint($"{swaggerSettings.Endpoint}", swaggerSettings.Title);
                    }
                );
                app.UseDefaultException(_logger, true);
            }
            else
            {
                app.UseDefaultException(_logger, false);
            }

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });
        }
        private CredentialSetting _Credential(AppSettings appSettings)
        {
            var securityEncrpytion = new SecurityEncryption();
            var vaultHost = securityEncrpytion.RSADecrypt(appSettings.VaultHost);
            var Token = securityEncrpytion.RSADecrypt(appSettings.Shopping);

            var vaultService = new VaultService(new VaultConfig
            {
                Url = new FlurlClient(vaultHost),
                Token = Token,
            });

            var credentialTxt = vaultService.GetCredential("secret/data/shopping").Result;

            var credential = JsonConvert.DeserializeObject<CredentialSetting>(credentialTxt);
            return credential;
        }
    }
}