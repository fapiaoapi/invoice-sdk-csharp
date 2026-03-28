using System.Security.Cryptography;
using System.Text;

namespace Tax.Invoice.Util;

public static class SignatureUtil
{
    public static string GenerateRandomString(int length)
    {
        if (length <= 0)
        {
            return string.Empty;
        }

        const string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var data = new byte[length];
        RandomNumberGenerator.Fill(data);
        var result = new char[length];
        for (var i = 0; i < length; i++)
        {
            result[i] = chars[data[i] % chars.Length];
        }
        return new string(result);
    }

    public static string CalculateSignature(string method, string path, string randomString, string timestamp, string appKey, string appSecret)
    {
        var content = $"Method={method}&Path={path}&RandomString={randomString}&TimeStamp={timestamp}&AppKey={appKey}";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(appSecret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(content));
        return ConvertToHex(hash).ToUpperInvariant();
    }

    public static string GetCurrentTimestamp()
    {
        var seconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        return seconds.ToString();
    }

    private static string ConvertToHex(byte[] bytes)
    {
        var builder = new StringBuilder(bytes.Length * 2);
        foreach (var b in bytes)
        {
            builder.Append(b.ToString("x2"));
        }
        return builder.ToString();
    }
}
