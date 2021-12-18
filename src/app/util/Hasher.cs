using System.Security.Cryptography;
using System.Text;

namespace futura.Util;

public static class Hasher
{
    public static string ToSha256(string data)
    {
        using var alg = SHA256.Create();
        var hash = alg.ComputeHash(Encoding.UTF8.GetBytes(data));

        // Convert the byte array to hexadecimal string
        var sb = new StringBuilder();
        for (int i = 0; i < hash.Length; i++)
        {
            sb.Append(hash[i].ToString("X2"));
        }
        return sb.ToString();
    }
}