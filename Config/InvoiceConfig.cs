namespace Tax.Invoice.Config;

public sealed class InvoiceConfig
{
    public string BaseUrl { get; }
    public string AppKey { get; }
    public string AppSecret { get; }
    public bool Debug { get; }

    public InvoiceConfig(string appKey, string appSecret, bool debug = false)
        : this("https://api.fa-piao.com", appKey, appSecret, debug)
    {
    }

    public InvoiceConfig(string baseUrl, string appKey, string appSecret, bool debug = false)
    {
        BaseUrl = baseUrl;
        AppKey = appKey;
        AppSecret = appSecret;
        Debug = debug;
    }
}
