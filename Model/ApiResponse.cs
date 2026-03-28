using System.Text.Json;

namespace Tax.Invoice.Model;

public sealed class ApiResponse<T>
{
    public int Code { get; set; }
    public string? Msg { get; set; }
    public T? Data { get; set; }
    public int Total { get; set; }

    public bool IsSuccess => Code == 200;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static ApiResponse<TData> FromJson<TData>(string json)
    {
        try
        {
            var response = JsonSerializer.Deserialize<ApiResponse<TData>>(json, JsonOptions);
            return response ?? BuildErrorResponse<TData>(ExtractErrorMessage(json));
        }
        catch
        {
            return BuildErrorResponse<TData>(ExtractErrorMessage(json));
        }
    }

    public static ApiResponse<string> FromJson(string json)
    {
        return FromJson<string>(json);
    }

    public static ApiResponse<Dictionary<string, object?>> FromJsonMap(string json)
    {
        var response = FromJson<JsonElement>(json);
        return ConvertMapResponse(response, json);
    }

    public static ApiResponse<List<Dictionary<string, object?>>> FromJsonListMap(string json)
    {
        var response = FromJson<JsonElement>(json);
        if (!response.IsSuccess || response.Data.ValueKind == JsonValueKind.Undefined)
        {
            return BuildErrorResponse<List<Dictionary<string, object?>>>(response.Code, response.Msg, json, response.Total);
        }

        var list = new List<Dictionary<string, object?>>();
        if (response.Data.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in response.Data.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.Object)
                {
                    list.Add(ToDictionary(item));
                }
            }
        }
        else if (response.Data.ValueKind == JsonValueKind.Object)
        {
            list.Add(ToDictionary(response.Data));
        }

        return new ApiResponse<List<Dictionary<string, object?>>>
        {
            Code = response.Code,
            Msg = response.Msg,
            Data = list,
            Total = response.Total
        };
    }

    private static ApiResponse<Dictionary<string, object?>> ConvertMapResponse(ApiResponse<JsonElement> response, string json)
    {
        if (!response.IsSuccess || response.Data.ValueKind == JsonValueKind.Undefined)
        {
            return BuildErrorResponse<Dictionary<string, object?>>(response.Code, response.Msg, json, response.Total);
        }

        var data = response.Data.ValueKind == JsonValueKind.Object
            ? ToDictionary(response.Data)
            : new Dictionary<string, object?>();

        return new ApiResponse<Dictionary<string, object?>>
        {
            Code = response.Code,
            Msg = response.Msg,
            Data = data,
            Total = response.Total
        };
    }

    private static Dictionary<string, object?> ToDictionary(JsonElement element)
    {
        var dict = new Dictionary<string, object?>();
        foreach (var property in element.EnumerateObject())
        {
            dict[property.Name] = ToObject(property.Value);
        }
        return dict;
    }

    private static object? ToObject(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number => element.TryGetInt64(out var l) ? l : element.GetDouble(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Object => ToDictionary(element),
            JsonValueKind.Array => element.EnumerateArray().Select(ToObject).ToList(),
            _ => null
        };
    }

    private static string ExtractErrorMessage(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return "Empty response body";
        }

        try
        {
            using var document = JsonDocument.Parse(json);
            if (document.RootElement.ValueKind == JsonValueKind.String)
            {
                return document.RootElement.GetString() ?? json.Trim();
            }
        }
        catch
        {
        }

        var trimmed = json.Trim();
        if (trimmed.Length >= 2 && trimmed.StartsWith('\"') && trimmed.EndsWith('\"'))
        {
            return trimmed.Substring(1, trimmed.Length - 2).Replace("\\/", "/");
        }
        return trimmed;
    }

    private static ApiResponse<TData> BuildErrorResponse<TData>(string message)
    {
        return new ApiResponse<TData>
        {
            Code = -1,
            Msg = message,
            Data = default
        };
    }

    private static ApiResponse<TData> BuildErrorResponse<TData>(int code, string? message, string json, int total)
    {
        var resolvedMessage = string.IsNullOrWhiteSpace(message) ? ExtractErrorMessage(json) : message;
        return new ApiResponse<TData>
        {
            Code = code,
            Msg = resolvedMessage,
            Data = default,
            Total = total
        };
    }
}
