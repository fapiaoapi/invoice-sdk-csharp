using System.Text;

namespace Tax.Invoice.Example;

public static class BasicExample
{
    public static void Run()
    {
        // 显式设置 System.out 的编码为 UTF-8
        Console.OutputEncoding = Encoding.UTF8;

        // 配置信息
        var appKey = "";
        var appSecret = "";

        // 统一社会信用代码
        var nsrsbh = "91500112MADFAXXXX";
        // 名称（营业执照）
        var title = "XXXX科技有限公司";
        // 手机号码（电子税务局）
        var username = "1325580XXXX";
        // 个人用户密码（电子税务局）
        var password = "123456XXXX";
        var fphm = "";
        var kprq = "";
        var token = "";
        var type = "6";
        // 6 基础 7 标准

        var redis = new Dictionary<string, string>();
        // Redis配置
        /*
          <dependency>
              <groupId>redis.clients</groupId>
              <artifactId>jedis</artifactId>
              <version>3.9.0</version>
          </dependency>
         */

        var client = new InvoiceClient(appKey, appSecret);
        // 创建客户端

        var redisKey = nsrsbh + "@" + username + "@TOKEN";
        // 从Redis获取Token
        if (redis.TryGetValue(redisKey, out var cachedToken) && !string.IsNullOrWhiteSpace(cachedToken))
        {
            token = cachedToken;
            client.SetAuthorization(token);
            Console.WriteLine("Token From Redis: ");
        }
        else
        {
            var authResponse = client.GetAuthorization(nsrsbh, type);
            // 一 获取授权
            /*
             * 获取授权Token文档
             * @see https://fa-piao.com/doc.html#api1?source=github
             */
            if (authResponse.IsSuccess && authResponse.Data?.Token is { Length: > 0 })
            {
                token = authResponse.Data.Token;
                client.SetAuthorization(token);
                redis[redisKey] = token;
                Console.WriteLine("授权成功，Token: " + token);
            }
            else
            {
                Console.WriteLine("授权失败: " + authResponse.Msg);
                return;
            }
        }

        try
        {
            var invoiceParams = new Dictionary<string, object?>
            /*
             * 前端模拟数电发票/电子发票开具 (蓝字发票)
             * @see https://fa-piao.com/fapiao.html?source=github
             */

            /*
             *
             * 开票参数说明demo
             * @see https://github.com/fapiaoapi/invoice-sdk-java/blob/master/examples/TaxExample.java
             */
            // 开具蓝票参数
            {
                ["fplxdm"] = "82",
                ["fplxdm"] = "82",
                ["fpqqlsh"] = appKey + DateTimeOffset.Now.ToUnixTimeMilliseconds(),
                ["ghdwmc"] = "个人",
                ["hjje"] = 396.04,
                ["hjse"] = 3.96,
                ["jshj"] = 400,
                ["kplx"] = 0,
                ["username"] = username,
                ["xhdwdzdh"] = "重庆市渝北区龙溪街道丽园路2号XXXX 1325580XXXX",
                ["xhdwmc"] = title,
                ["xhdwsbh"] = nsrsbh,
                ["xhdwyhzh"] = "工商银行XXXX 15451211XXXX",
                ["zsfs"] = 0,
                ["fyxm[0][fphxz]"] = 0,
                ["fyxm[0][spmc]"] = "*软件维护服务*接口服务费",
                ["fyxm[0][ggxh]"] = "",
                ["fyxm[0][dw]"] = "次",
                ["fyxm[0][spsl]"] = 100,
                ["fyxm[0][dj]"] = 1,
                ["fyxm[0][je]"] = 100,
                ["fyxm[0][sl]"] = 0.01,
                ["fyxm[0][se]"] = 0.99,
                ["fyxm[0][hsbz]"] = 1,
                ["fyxm[0][spbm]"] = "3040201030000000000",
                ["fyxm[1][fphxz]"] = 0,
                ["fyxm[1][spmc]"] = "*软件维护服务*接口服务费",
                ["fyxm[1][ggxh]"] = "",
                ["fyxm[1][spsl]"] = 150,
                ["fyxm[1][dj]"] = 2,
                ["fyxm[1][je]"] = 300,
                ["fyxm[1][sl]"] = 0.01,
                ["fyxm[1][se]"] = 2.97,
                ["fyxm[1][hsbz]"] = 1,
                ["fyxm[1][spbm]"] = "3040201030000000000"
            };

            // 二 开具蓝票
            /*
             * 开具数电发票文档
             * @see https://fa-piao.com/doc.html#api6?source=github
             *
             */
            var invoiceResponse = client.BlueTicket(invoiceParams);
            switch (invoiceResponse.Code)
            {
                case 200:
                    var invoiceData = invoiceResponse.Data;
                    if (invoiceData == null || !invoiceData.TryGetValue("Fphm", out var fphmValue) || !invoiceData.TryGetValue("Kprq", out var kprqValue))
                    {
                        Console.WriteLine("开票成功但返回字段缺失: " + invoiceResponse.Data);
                        break;
                    }

                    fphm = fphmValue?.ToString() ?? "";
                    kprq = kprqValue?.ToString() ?? "";
                    Console.WriteLine("发票号码: " + fphm);
                    Console.WriteLine("开票日期: " + kprq);

                    // 三 下载发票
                    var pdfParams = new Dictionary<string, object?>
                    {
                        ["downflag"] = "4",
                        ["nsrsbh"] = nsrsbh,
                        ["username"] = username,
                        ["fphm"] = fphm,
                        ["Kprq"] = kprq
                    };

                    var pdfResponse = client.GetPdfOfdXml(pdfParams);
                    if (pdfResponse.IsSuccess)
                    {
                        Console.WriteLine("发票下载成功");
                        Console.WriteLine(pdfResponse.Data);
                    }
                    break;
                case 420:
                    Console.WriteLine("登录(短信认证)");
                    /*
                     * 前端模拟短信认证弹窗
                     * @see https://fa-piao.com/fapiao.html?action=sms&source=github
                     */
                    // 1. 发短信验证码
                    /*
                     * @see https://fa-piao.com/doc.html#api2?source=github
                     */
                    var loginResponse = client.LoginDppt(nsrsbh, username, password, "");
                    if (loginResponse.Code == 200)
                    {
                        Console.WriteLine(loginResponse.Msg);
                        Console.WriteLine("请" + username + "接收验证码");
                    }

                    // 2. 输入验证码
                    /*
                     * @see https://fa-piao.com/doc.html#api2?source=github
                     */
                    Console.WriteLine("请输入验证码");
                    var smsCode = "";
                    var loginResponse2 = client.LoginDppt(nsrsbh, username, password, smsCode);
                    if (loginResponse2.Code == 200)
                    {
                        Console.WriteLine(loginResponse2.Data);
                        Console.WriteLine("验证成功");
                    }
                    break;
                case 430:
                    Console.WriteLine("人脸认证");
                    /*
                     * 前端模拟人脸认证弹窗
                     * @see https://fa-piao.com/fapiao.html?action=face&source=github
                     */
                    // 1. 获取人脸二维码
                    /*
                     * @see https://fa-piao.com/doc.html#api3?source=github
                     */
                    var qrCodeResponse = client.GetFaceImg(nsrsbh, username, "1");
                    var qrData = qrCodeResponse.Data;
                    if (qrData == null)
                    {
                        Console.WriteLine("人脸二维码返回为空: " + qrCodeResponse.Msg);
                        break;
                    }
                    var ewmly = qrData.GetValueOrDefault("ewmly")?.ToString() ?? "";
                    Console.WriteLine(ewmly == "swj" ? "请使用税务局app扫码" : "个人所得税app扫码");
                    var ewmObj = qrData.GetValueOrDefault("ewm");
                    if (ewmObj != null && ewmObj.ToString()?.Length < 500)
                    {
                        /*
                           <dependency>
                               <groupId>com.google.zxing</groupId>
                               <artifactId>core</artifactId>
                               <version>3.5.3</version>
                           </dependency>
                           <dependency>
                               <groupId>com.google.zxing</groupId>
                               <artifactId>javase</artifactId>
                               <version>3.5.3</version>
                           </dependency>
                         */
                        var base64 = ToBase64(ewmObj.ToString() ?? "", 300);
                        qrData["ewm"] = base64;
                        Console.WriteLine("data:image/png;base64," + base64);
                    }

                    // 2. 认证完成后获取人脸二维码认证状态
                    /*
                     * @see https://fa-piao.com/doc.html#api4?source=github
                     */
                    var rzid = qrData.GetValueOrDefault("rzid")?.ToString() ?? "";
                    var faceStatusResponse = client.GetFaceState(nsrsbh, rzid, username, "1");
                    Console.WriteLine("code: " + faceStatusResponse.Code);
                    Console.WriteLine("data: " + faceStatusResponse.Data);

                    if (faceStatusResponse.Data != null && faceStatusResponse.Data.GetValueOrDefault("slzt") != null)
                    {
                        var slzt = faceStatusResponse.Data.GetValueOrDefault("slzt")?.ToString() ?? "";
                        var status = slzt == "1" ? "未认证" : (slzt == "2" ? "成功" : "二维码过期");
                        Console.WriteLine("认证状态: " + status);
                    }
                    break;
                case 401:
                    // token过期 重新获取并缓存token
                    Console.WriteLine("授权失败:" + invoiceResponse.Msg);
                    // 重新获取token的逻辑
                    break;
                default:
                    // Console.WriteLine("参数:" + invoiceParams);
                    Console.WriteLine(invoiceResponse.Code + " " + invoiceResponse.Msg);
                    break;
            }
        }
        catch (Exception e)
        {
            // 处理开票异常
            Console.WriteLine("错误: " + e.Message);
        }
    }

    // 生成二维码（示例）
    public static string ToBase64(string text, int size)
    {
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
    }
}
