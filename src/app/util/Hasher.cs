using System.Security.Cryptography;
using System.Text;

namespace futura.Util;

public class Hasher
{
    public string ToSha256(string data)
    {
        var bytes = Encoding.UTF8.GetBytes(data);

        using var alg = SHA256.Create();

        var hash = alg.ComputeHash(bytes);

        // Convert the byte array to hexadecimal string
        var sb = new StringBuilder();
        for (int i = 0; i < hash.Length; i++)
        {
            sb.Append(hash[i].ToString("x2"));
        }
        return sb.ToString();

    }
}