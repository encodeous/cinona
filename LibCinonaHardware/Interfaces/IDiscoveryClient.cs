using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibCinonaHardware.Transport;

namespace LibCinonaHardware.Interfaces
{
    public interface IDiscoveryClient
    {
        public IAsyncEnumerable<CinPeer> GetPeersAsync();
    }
}
