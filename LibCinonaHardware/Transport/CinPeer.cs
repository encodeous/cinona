using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LibCinonaHardware.Transport;

public partial class CinPeer
{
    public string Name { get; init; }
    public ECPoint PubKey { get; init; }
}