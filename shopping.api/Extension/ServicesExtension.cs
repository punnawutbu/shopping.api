using shopping.api.Models;
using shopping.api.Shared.Facades;
using shopping.api.Shared.Repositories;
using shopping.api.Shared.Services;
using Microsoft.Extensions.DependencyInjection;

namespace shopping.api.Extension
{
    public static class ServicesExtension
    {
        public static void ConfigureScopeFacades(this IServiceCollection services, AppSettings appSettings)
        {
            services.AddTransient<IProductFacades, ProductFacades>();
            services.AddTransient<IOrderFacades, OrderFacades>();

        }
        public static void ConfigureScopeService(this IServiceCollection services, AppSettings appSettings)
        {
            var PublicKey = appSettings?.CredentialSetting?.PublicKey;
            var HashKey = appSettings.CredentialSetting.HashKey;
            var SecertKey = appSettings.CredentialSetting.SecertKey;
            var securityEncryption = new SecurityEncryption($"{PublicKey}");
            services.AddSingleton<ISecurityEncryption, SecurityEncryption>(x => securityEncryption);
            services.AddTransient<ISecurityService, SecurityService>(x => new SecurityService(securityEncryption, $"{HashKey}"));
            services.AddTransient<IJwtService, JwtService>(x => new JwtService($"{SecertKey}"));
        }
        public static void ConfigureScopeRepository(this IServiceCollection services, AppSettings appSettings)
        {
            services.AddTransient<IShoppingRepository, ShoppingRepository>(s => new ShoppingRepository(
                appSettings.CredentialSetting.ShoppingConnectionString,
                appSettings.CredentialSetting.SslMode,
                appSettings.CredentialSetting.Certificate
                ));
        }
    }
}