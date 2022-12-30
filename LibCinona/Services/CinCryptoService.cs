using System.Security.Cryptography;
using LibCinona.Extensions;

namespace LibCinona.Services;

public class CinCryptoService
{
    private ECParameters privKey;

    public CinCryptoService()
    {
        var ecd = ECDsa.Create();
        // secp384r1
        ecd.GenerateKey(ECCurve.CreateFromValue("1.3.132.0.34"));
        privKey = ecd.ExportParameters(true);
    }

    public ECPoint GetPublicKey()
    {
        return privKey.Q;
    }

    public byte[] Sign(byte[] data)
    {
        return CryptoExtensions.Sign(data, privKey);
    }
}