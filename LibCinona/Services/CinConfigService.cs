using LibCinona.Models;

namespace LibCinona.Services;

public class CinConfigService
{
    public DeviceInfo GetInfo()
    {
        // TODO: configure device information
        return new DeviceInfo()
        {
            Name = Environment.MachineName
        };
    }
}