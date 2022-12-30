using LibCinonaHardware.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibCinona.Services;

namespace LibCinonaHardware.Transport
{
    public partial class CinDiscoveryServer
    {
        ILogger _logger;
        CinCryptoService _cinCryptoService;
        CinConfigService _cinConfigService;

        public CinDiscoveryServer(ILogger logger, CinCryptoService cinCryptoService, CinConfigService cinConfigService)
        {
            _logger = logger;
            _cinCryptoService = cinCryptoService;
            _cinConfigService = cinConfigService;
        }
    }
}
