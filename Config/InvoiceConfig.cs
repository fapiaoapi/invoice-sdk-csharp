namespace Tax.Invoice.Config;

public sealed class InvoiceConfig
{
    public string BaseUrl { get; }
    public string AppKey { get; }
    public string AppSecret { get; }

    public InvoiceConfig(string appKey, string appSecret)
        : this("https://api.fa-piao.com", appKey, appSecret)
    {
    }

    public InvoiceConfig(string baseUrl, string appKey, string appSecret)
    {
        BaseUrl = baseUrl;
        AppKey = appKey;
        AppSecret = appSecret;
    }
}
