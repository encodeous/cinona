using LibCinona.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LibCinona.Extensions;

public static class CinonaServiceExtensions
{
    public static void AddCinona(this IServiceCollection services)
    {
        services.AddSingleton<CinConfigService>();
        services.AddSingleton<CinCryptoService>();
    }
}