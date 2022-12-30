using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibCinonaHardware.Interfaces
{
    public interface IDiscoveryServer
    {
        Task<bool> IsSupported();
        /// <summary>
        /// Starts listening for Cinona Clients
        /// </summary>
        /// <returns>true if it started successfully</returns>
        Task<bool> Start();
        void Stop();
    }
}
