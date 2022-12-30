using LibCinona.Services;
using LibCinonaHardware.Interfaces;
using LibCinonaHardware.Transport;
using Microsoft.Extensions.Logging;

namespace LibCinonaHardware;

public static class CinonaHardwareExtensions
{
    public static void AddCinonaHardware(this IServiceCollection service)
    {
        service.AddSingleton(a
            => (IDiscoveryClient)new CinDiscoveryClient(a.GetService<ILogger<IDiscoveryClient>>()));
        service.AddSingleton(a
            => (IDiscoveryServer)new CinDiscoveryServer(
                a.GetService<ILogger<IDiscoveryClient>>(),
                a.GetService<CinCryptoService>(),
                a.GetService<CinConfigService>()
                ));
    }
}