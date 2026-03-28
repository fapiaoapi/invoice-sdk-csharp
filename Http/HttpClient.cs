using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Tax.Invoice.Config;
using Tax.Invoice.Util;

namespace Tax.Invoice.Http;

public sealed class HttpClient
{
    private readonly InvoiceConfig _config;
    private readonly System.Net.Http.HttpClient _client;

    public HttpClient(InvoiceConfig config)
    {
        _config = config;
        _client = new System.Net.Http.HttpClient
        {
            DefaultRequestVersion = HttpVersion.Version20
        };
    }

    public string Post(string path, Dictionary<string, object?> formData, string? authorization)
    {
        var method = "POST";
        var randomString = SignatureUtil.GenerateRandomString(20);
        var timestamp = SignatureUtil.GetCurrentTimestamp();
        var signature = SignatureUtil.CalculateSignature(method, path, randomString, timestamp, _config.AppKey, _config.AppSecret);
        var boundary = "----CsInvoiceBoundary" + SignatureUtil.GenerateRandomString(20);
        var requestBody = BuildMultipartBody(formData, boundary);

        var request = BuildRequest(path, signature, timestamp, randomString, $"multipart/form-data; boundary={boundary}", authorization);
        request.Content = new ByteArrayContent(requestBody);
        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse($"multipart/form-data; boundary={boundary}");

        using var response = _client.Send(request);
        return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
    }

    public string PostJson(string path, Dictionary<string, object?> formData, string? authorization)
    {
        var method = "POST";
        var randomString = SignatureUtil.GenerateRandomString(20);
        var timestamp = SignatureUtil.GetCurrentTimestamp();
        var signature = SignatureUtil.CalculateSignature(method, path, randomString, timestamp, _config.AppKey, _config.AppSecret);
        var requestBody = BuildJsonBody(formData);

        var request = BuildRequest(path, signature, timestamp, randomString, "application/json; charset=UTF-8", authorization);
        request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

        using var response = _client.Send(request);
        return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
    }

    public string Get(string path, Dictionary<string, string>? queryParams, string? authorization)
    {
        var method = "GET";
        var randomString = SignatureUtil.GenerateRandomString(20);
        var timestamp = SignatureUtil.GetCurrentTimestamp();

        var urlBuilder = new StringBuilder(_config.BaseUrl).Append(path);
        if (queryParams is { Count: > 0 })
        {
            urlBuilder.Append("?");
            foreach (var entry in queryParams)
            {
                urlBuilder.Append(Uri.EscapeDataString(entry.Key))
                    .Append("=")
                    .Append(Uri.EscapeDataString(entry.Value))
                    .Append("&");
            }
            urlBuilder.Length -= 1;
        }

        var signature = SignatureUtil.CalculateSignature(method, path, randomString, timestamp, _config.AppKey, _config.AppSecret);

        var request = new HttpRequestMessage(HttpMethod.Get, urlBuilder.ToString());
        request.Headers.TryAddWithoutValidation("AppKey", _config.AppKey);
        request.Headers.TryAddWithoutValidation("Sign", signature);
        request.Headers.TryAddWithoutValidation("TimeStamp", timestamp);
        request.Headers.TryAddWithoutValidation("RandomString", randomString);
        request.Headers.TryAddWithoutValidation("Accept-Charset", "UTF-8");
        if (!string.IsNullOrWhiteSpace(authorization))
        {
            request.Headers.TryAddWithoutValidation("Authorization", authorization);
        }

        using var response = _client.Send(request);
        return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
    }

    private HttpRequestMessage BuildRequest(string path, string signature, string timestamp, string randomString, string contentType, string? authorization)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_config.BaseUrl}{path}");
        request.Headers.TryAddWithoutValidation("AppKey", _config.AppKey);
        request.Headers.TryAddWithoutValidation("Sign", signature);
        request.Headers.TryAddWithoutValidation("TimeStamp", timestamp);
        request.Headers.TryAddWithoutValidation("RandomString", randomString);
        request.Headers.TryAddWithoutValidation("Accept-Charset", "UTF-8");
        if (!string.IsNullOrWhiteSpace(authorization))
        {
            request.Headers.TryAddWithoutValidation("Authorization", authorization);
        }
        request.Headers.TryAddWithoutValidation("Content-Type", contentType);
        return request;
    }

    private static byte[] BuildMultipartBody(Dictionary<string, object?> data, string boundary)
    {
        const string lineEnd = "\r\n";
        using var output = new MemoryStream();

        foreach (var entry in data)
        {
            var value = entry.Value;
            if (value is null)
            {
                continue;
            }
            WriteUtf8(output, "--" + boundary + lineEnd);
            WriteUtf8(output, "Content-Disposition: form-data; name=\"" + EscapeFormFieldName(entry.Key) + "\"" + lineEnd);
            WriteUtf8(output, lineEnd);
            WriteUtf8(output, value + lineEnd);
        }

        WriteUtf8(output, "--" + boundary + "--" + lineEnd);
        return output.ToArray();
    }

    private static void WriteUtf8(Stream output, string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        output.Write(bytes, 0, bytes.Length);
    }

    private static string EscapeFormFieldName(string fieldName)
    {
        return fieldName.Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\r", string.Empty)
            .Replace("\n", string.Empty);
    }

    private static string BuildJsonBody(Dictionary<string, object?> data)
    {
        if (data.Count == 0)
        {
            return "{}";
        }
        return JsonSerializer.Serialize(data);
    }
}
