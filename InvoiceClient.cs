using Tax.Invoice.Config;
using InvoiceHttpClient = Tax.Invoice.Http.HttpClient;
using Tax.Invoice.Model;

namespace Tax.Invoice;

public sealed class InvoiceClient
{
    private readonly InvoiceConfig _config;
    private readonly InvoiceHttpClient _httpClient;
    private string? _authorization;

    public InvoiceClient(string appKey, string appSecret)
        : this(new InvoiceConfig(appKey, appSecret))
    {
    }

    public InvoiceClient(InvoiceConfig config)
    {
        _config = config;
        _httpClient = new InvoiceHttpClient(config);
    }

    public ApiResponse<AuthorizationResponse> GetAuthorization(string nsrsbh)
    {
        var formData = new Dictionary<string, object?>
        {
            ["nsrsbh"] = nsrsbh
        };

        var response = _httpClient.Post("/v5/enterprise/authorization", formData, null);
        var apiResponse = ApiResponse<AuthorizationResponse>.FromJson<AuthorizationResponse>(response);

        if (apiResponse.IsSuccess && apiResponse.Data?.Token is { Length: > 0 })
        {
            _authorization = apiResponse.Data.Token;
        }

        return apiResponse;
    }

    public ApiResponse<AuthorizationResponse> GetAuthorization(string nsrsbh, string type)
    {
        var formData = new Dictionary<string, object?>
        {
            ["nsrsbh"] = nsrsbh,
            ["type"] = type
        };

        var response = _httpClient.Post("/v5/enterprise/authorization", formData, null);
        var apiResponse = ApiResponse<AuthorizationResponse>.FromJson<AuthorizationResponse>(response);

        if (apiResponse.IsSuccess && apiResponse.Data?.Token is { Length: > 0 })
        {
            _authorization = apiResponse.Data.Token;
        }

        return apiResponse;
    }

    public ApiResponse<AuthorizationResponse> GetAuthorization(string nsrsbh, string type, string username, string password)
    {
        var formData = new Dictionary<string, object?>
        {
            ["nsrsbh"] = nsrsbh,
            ["type"] = type,
            ["username"] = username,
            ["password"] = password
        };

        var response = _httpClient.Post("/v5/enterprise/authorization", formData, null);
        var apiResponse = ApiResponse<AuthorizationResponse>.FromJson<AuthorizationResponse>(response);

        if (apiResponse.IsSuccess && apiResponse.Data?.Token is { Length: > 0 })
        {
            _authorization = apiResponse.Data.Token;
        }

        return apiResponse;
    }

    public ApiResponse<Dictionary<string, object?>> LoginDppt(string nsrsbh, string username, string password, string? sms)
    {
        var formData = new Dictionary<string, object?>
        {
            ["nsrsbh"] = nsrsbh,
            ["username"] = username,
            ["password"] = password
        };

        if (!string.IsNullOrWhiteSpace(sms))
        {
            formData["sms"] = sms;
        }

        var response = _httpClient.Post("/v5/enterprise/loginDppt", formData, _authorization);
        return ApiResponse<Dictionary<string, object?>>.FromJsonMap(response);
    }

    public ApiResponse<Dictionary<string, object?>> FaceLoginDppt(string nsrsbh, string username, string password, string sf, string ewmlx, string? ewmid)
    {
        var formData = new Dictionary<string, object?>
        {
            ["nsrsbh"] = nsrsbh,
            ["username"] = username,
            ["password"] = password,
            ["sf"] = sf,
            ["ewmlx"] = ewmlx
        };

        if (!string.IsNullOrWhiteSpace(ewmid))
        {
            formData["ewmid"] = ewmid;
        }

        var response = _httpClient.Post("/v5/enterprise/loginDppt", formData, _authorization);
        return ApiResponse<Dictionary<string, object?>>.FromJsonMap(response);
    }

    public ApiResponse<Dictionary<string, object?>> GetFaceImg(string nsrsbh, string? username, string? type)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["nsrsbh"] = nsrsbh
        };

        if (!string.IsNullOrWhiteSpace(username))
        {
            queryParams["username"] = username;
        }

        if (!string.IsNullOrWhiteSpace(type))
        {
            queryParams["type"] = type;
        }

        var response = _httpClient.Get("/v5/enterprise/getFaceImg", queryParams, _authorization);
        return ApiResponse<Dictionary<string, object?>>.FromJsonMap(response);
    }

    public ApiResponse<Dictionary<string, object?>> GetFaceState(string nsrsbh, string rzid, string? username, string? type)
    {
        var queryParams = new Dictionary<string, string>
        {
            ["nsrsbh"] = nsrsbh,
            ["rzid"] = rzid
        };

        if (!string.IsNullOrWhiteSpace(username))
        {
            queryParams["username"] = username;
        }

        if (!string.IsNullOrWhiteSpace(type))
        {
            queryParams["type"] = type;
        }

        var response = _httpClient.Get("/v5/enterprise/getFaceState", queryParams, _authorization);
        return ApiResponse<Dictionary<string, object?>>.FromJsonMap(response);
    }

    public ApiResponse<Dictionary<string, object?>> QueryFaceAuthState(string nsrsbh, string? username)
    {
        var formData = new Dictionary<string, object?>
        {
            ["nsrsbh"] = nsrsbh
        };

        if (!string.IsNullOrWhiteSpace(username))
        {
            formData["username"] = username;
        }

        var response = _httpClient.Post("/v5/enterprise/queryFaceAuthState", formData, _authorization);
        return ApiResponse<Dictionary<string, object?>>.FromJsonMap(response);
    }

    public ApiResponse<Dictionary<string, object?>> BlueTicket(Dictionary<string, object?> parameters)
    {
        var response = _httpClient.Post("/v5/enterprise/blueTicket", parameters, _authorization);
        return ApiResponse<Dictionary<string, object?>>.FromJsonMap(response);
    }

    public ApiResponse<Dictionary<string, object?>> GetPdfOfdXml(Dictionary<string, object?> parameters)
    {
        var response = _httpClient.Post("/v5/enterprise/pdfOfdXml", parameters, _authorization);
        return ApiResponse<Dictionary<string, object?>>.FromJsonMap(response);
    }

    public ApiResponse<Dictionary<string, object?>> QueryBlueTicketInfo(Dictionary<string, object?> parameters)
    {
        var response = _httpClient.Post("/v5/enterprise/retMsg", parameters, _authorization);
        return ApiResponse<Dictionary<string, object?>>.FromJsonMap(response);
    }

    public ApiResponse<Dictionary<string, object?>> ApplyRedInfo(Dictionary<string, object?> parameters)
    {
        var response = _httpClient.Post("/v5/enterprise/hzxxbsq", parameters, _authorization);
        return ApiResponse<Dictionary<string, object?>>.FromJsonMap(response);
    }

    public ApiResponse<Dictionary<string, object?>> RedTicket(Dictionary<string, object?> parameters)
    {
        var response = _httpClient.Post("/v5/enterprise/hzfpkj", parameters, _authorization);
        return ApiResponse<Dictionary<string, object?>>.FromJsonMap(response);
    }

    public ApiResponse<Dictionary<string, object?>> SyncRedInfo(Dictionary<string, object?> parameters)
    {
        var response = _httpClient.Post("/v5/enterprise/hzxxbtb", parameters, _authorization);
        return ApiResponse<Dictionary<string, object?>>.FromJsonMap(response);
    }

    public ApiResponse<Dictionary<string, object?>> SwitchAccount(Dictionary<string, object?> parameters)
    {
        var response = _httpClient.Post("/v5/enterprise/changeUser", parameters, _authorization);
        return ApiResponse<Dictionary<string, object?>>.FromJsonMap(response);
    }

    public ApiResponse<Dictionary<string, object?>> QueryCreditLimit(Dictionary<string, object?> parameters)
    {
        var response = _httpClient.Post("/v5/enterprise/creditLine", parameters, _authorization);
        return ApiResponse<Dictionary<string, object?>>.FromJsonMap(response);
    }

    public ApiResponse<List<Dictionary<string, object?>>> QueryList(string path, Dictionary<string, object?> parameters, string method)
    {
        if (string.Equals(method, "GET", StringComparison.OrdinalIgnoreCase))
        {
            var query = new Dictionary<string, string>();
            foreach (var entry in parameters)
            {
                if (entry.Value is null)
                {
                    continue;
                }
                query[entry.Key] = entry.Value.ToString() ?? string.Empty;
            }
            var response = _httpClient.Get(path, query, _authorization);
            return ApiResponse<List<Dictionary<string, object?>>>.FromJsonListMap(response);
        }

        var postResponse = _httpClient.Post(path, parameters, _authorization);
        return ApiResponse<List<Dictionary<string, object?>>>.FromJsonListMap(postResponse);
    }

    public ApiResponse<Dictionary<string, object?>> HttpPost(string path, Dictionary<string, object?> parameters)
    {
        var response = _httpClient.Post(path, parameters, _authorization);
        return ApiResponse<Dictionary<string, object?>>.FromJsonMap(response);
    }

    public ApiResponse<Dictionary<string, object?>> HttpPostJson(string path, Dictionary<string, object?> parameters)
    {
        var response = _httpClient.PostJson(path, parameters, _authorization);
        return ApiResponse<Dictionary<string, object?>>.FromJsonMap(response);
    }

    public ApiResponse<Dictionary<string, object?>> HttpGet(string path, Dictionary<string, string> parameters)
    {
        var response = _httpClient.Get(path, parameters, _authorization);
        return ApiResponse<Dictionary<string, object?>>.FromJsonMap(response);
    }

    public void SetAuthorization(string authorization)
    {
        _authorization = authorization;
    }

    public string? GetAuthorizationToken()
    {
        return _authorization;
    }
}
