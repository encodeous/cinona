using System.Security.Cryptography;

namespace LibCinona.Extensions;

public class CryptoExtensions
{
    public static bool ValidateSignature(byte[] data, byte[] sig, ECPoint pubKey)
    {
        var ecd = ECDsa.Create(new ECParameters()
        {
            Q = pubKey,
            Curve = ECCurve.CreateFromValue("1.3.132.0.34") // secp384r1
        });
        return ecd.VerifyData(data, sig, HashAlgorithmName.SHA256);
    }
    public static byte[] Sign(byte[] data, ECParameters privKey)
    {
        var ecd = ECDsa.Create(privKey);
        return ecd.SignData(data, HashAlgorithmName.SHA256);
    }
}