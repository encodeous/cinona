using LibCinonaHardware.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibCinonaHardware.Transport
{
    public partial class CinDiscoveryClient
    {
        ILogger _logger;

        public CinDiscoveryClient(ILogger logger)
        {
            _logger = logger;
        }
    }
}
