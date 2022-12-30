using System.Security.Cryptography;
using System.Text.Json.Serialization;

namespace LibCinonaHardware.Transport;

internal class CinInfoPacket
{
    public string DeviceName { get; init; }
    public byte[] DeviceNameSig { get; init; }
    [JsonIgnore]
    public ECPoint PubKey
    {
        get
        {
            return new ECPoint()
            {
                X = X,
                Y = Y
            };
        }
        init
        {
            X = value.X;
            Y = value.Y;
        }
    }
    public byte[]? X { get; init; }
    public byte[]? Y { get; init; }
}