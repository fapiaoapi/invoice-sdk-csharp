# 电子发票/数电发票 C# SDK | 开票、验真、红冲一站式集成

[![NuGet](https://img.shields.io/nuget/v/Tax.Invoice?label=NuGet)](https://www.nuget.org/packages/Tax.Invoice)
[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://github.com/fapiaoapi/invoice-sdk-csharp/blob/master/LICENSE)
[![.NET 10](https://img.shields.io/badge/.NET-10-blueviolet.svg)](https://dotnet.microsoft.com/zh-cn/download/dotnet/10.0)

**发票 C# SDK** 面向电子发票、数电发票（全电发票）场景，支持**开票、红冲、版式文件下载**等核心功能，快速对接税务平台API。

**关键词**: 电子发票SDK,数电票C#,开票接口,发票api,发票开具,发票红冲,全电发票集成

---

## 📖 核心功能

### 基础认证
- ✅ **获取授权** - 快速接入税务平台身份认证
- ✅ **人脸二维码登录** - 支持数电发票平台扫码登录
- ✅ **认证状态查询** - 实时获取纳税人身份状态

### 发票开具
- 🎫 **数电蓝票开具** - 支持增值税普通/专用电子发票
- 📄 **版式文件下载** - 自动获取销项发票PDF/OFD/XML文件

### 发票红冲
- 🔍 **红冲前蓝票查询** - 精确检索待红冲的电子发票
- 🛑 **红字信息表申请** - 生成红冲凭证
- 🔄 **负数发票开具** - 自动化红冲流程

---

## 🚀 快速安装

### NuGet
```bash
dotnet add package Tax.Invoice --version 1.0.9
```

[📦 查看NuGet最新版本](https://www.nuget.org/packages/Tax.Invoice)

本SDK基于 C# 12 与 .NET 10 开发

---

[📚 查看完整中文文档](https://fa-piao.com/doc.html?source=github) | [💡 更多示例代码](https://github.com/fapiaoapi/invoice-sdk-csharp/tree/master/Example)

---

其他版本请用旧版本

[C#8-C#11开发票demo](https://github.com/fapiaoapi/invoice-sdk-csharp/blob/master/Example/BasicExample.cs "C#8-C#11开发票demo")

[C#8-C#11红冲发票demo](https://github.com/fapiaoapi/invoice-sdk-csharp/blob/master/Example/RedInvoiceExample.cs "C#8-C#11红冲发票demo")

[C#8-C#11发票税额demo](https://github.com/fapiaoapi/invoice-sdk-csharp/blob/master/Example/TaxExample.cs "C#8-C#11发票税额demo")

---

## 🔍 为什么选择此SDK？
- **精准覆盖中国数电发票标准** - 严格遵循国家最新接口规范
- **开箱即用** - 无需处理XML/签名等底层细节，专注业务逻辑
- **企业级验证** - 已在生产环境处理超100万张电子发票

---

## 📊 支持的开票类型
| 发票类型       | 状态   |
|----------------|--------|
| 数电发票（普通发票） | ✅ 支持 |
| 数电发票（增值税专用发票） | ✅ 支持 |
| 数电发票（铁路电子客票）  | ✅ 支持 |
| 数电发票（航空运输电子客票行程单） | ✅ 支持  |
| 数电票（二手车销售统一发票） | ✅ 支持  |
| 数电纸质发票（增值税专用发票） | ✅ 支持  |
| 数电纸质发票（普通发票） | ✅ 支持  |
| 数电纸质发票（机动车发票） | ✅ 支持  |
| 数电纸质发票（二手车发票） | ✅ 支持  |

---

## 🤝 贡献与支持
- 提交Issue: [问题反馈](https://github.com/fapiaoapi/invoice-sdk-csharp/issues)
- 商务合作: yuejianghe@qq.com

---

## 🎯 快速开始：

```csharp
using System;
using System.Collections.Generic;
using System.Text;
using Tax.Invoice;
using Tax.Invoice.Model;
using System.Text.Json;
using StackExchange.Redis;
using System.Threading.Tasks;
using System.Runtime.Versioning;
using ZXing;
using ZXing.QrCode;

using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

public class BasicExample
{

    public static String appKey = "";
    public static String appSecret = "";

    public static String nsrsbh = "";// 统一社会信用代码
    public static String title = "";// 名称（营业执照）
    public static String username = "";// 手机号码（电子税务局）
    public static String password = "";// 个人用户密码（电子税务局）
    public static String type = "6";// 6 基础 7标准
    public static String xhdwdzdh = "重庆市渝北区龙溪街道丽园路2号XXXX 1325580XXXX"; // 地址和电话 空格隔开
    public static String xhdwyhzh = "工商银行XXXX 15451211XXXX";// 开户行和银行账号 空格隔开
    public static String token = "";
    public static Boolean debug = true; // 是否打印日志

    // 创建客户端
    public static InvoiceClient client = new InvoiceClient(appKey, appSecret, debug);
    // redis
    public static ConnectionMultiplexer redis = LazyConnection.Connect("127.0.0.1:6379,password=test123456,abortConnect=false");
    public static async Task Main(string[] args)
    {
        try
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("dotnet " + Environment.Version);
            // dotnet add package StackExchange.Redis
            // 一 获取token 从Redis获取token
            GetToken(false);
            // 二 开具蓝票
            /*
             * 前端模拟数电发票/电子发票开具 (蓝字发票)
             * @see https://fa-piao.com/fapiao.html?source=github
             */
            var invoiceResponse = BlueTicket();
            switch (invoiceResponse.Code)
            {
                case 200:
                    // 三 下载发票
                    DownloadPdfOfdXml(invoiceResponse.Data["fphm"].ToString(), invoiceResponse.Data["kprq"].ToString());
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
                        Console.WriteLine("请输入验证码");
                        var smsTimeout = TimeSpan.FromSeconds(300);
                        Console.WriteLine($"请在{smsTimeout.TotalSeconds}秒内输入验证码，过期时间为: {DateTime.UtcNow.Add(smsTimeout).ToString("yyyy-MM-dd HH:mm:ss")}前)输入验证码: ");
                        var inputSmsTask = Task.Run(Console.ReadLine);
                        var completedSmsTask = await Task.WhenAny(inputSmsTask, Task.Delay(smsTimeout));
                        if (completedSmsTask == inputSmsTask)
                        {
                            Console.WriteLine($"输入: {inputSmsTask.Result}");
                            // 2. 输入验证码
                            /*
                             * @see https://fa-piao.com/doc.html#api2?source=github
                             */
                            var loginResponse2 = client.LoginDppt(nsrsbh, username, password, inputSmsTask.Result);
                            if (loginResponse2.Code == 200)
                            {
                                Console.WriteLine("短信验证成功");
                                Console.WriteLine("请再次调用BlueTicket");
                                invoiceResponse = BlueTicket();
                                if (invoiceResponse.Code == 200)
                                {
                                    DownloadPdfOfdXml(invoiceResponse.Data["fphm"].ToString(), invoiceResponse.Data["kprq"].ToString());
                                }
                                else
                                {
                                    Console.WriteLine(invoiceResponse.Code + "开具蓝票失败: " + invoiceResponse.Msg);
                                }
                            }
                            else
                            {
                                Console.WriteLine(loginResponse2.Code + "短信验证失败: " + loginResponse2.Msg);
                            }
                        }
                        else
                        {
                            Console.WriteLine("超时！未输入验证码！");
                        }
                    }
                    else
                    {
                        Console.WriteLine(loginResponse.Code + "发短信验证码失败: " + loginResponse.Msg);
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
                        Console.WriteLine(qrCodeResponse.Code + "人脸二维码返回为空: " + qrCodeResponse.Msg);
                        break;
                    }
                    var ewmly = qrData.GetValueOrDefault("ewmly")?.ToString() ?? "";
                    Console.WriteLine(ewmly == "swj" ? "请使用电子税务局app扫码" : "个人所得税app扫码");
                    Console.WriteLine("成功做完人脸认证,请输入数字 1");
                    var ewmObj = qrData.GetValueOrDefault("ewm");
                    if (ewmObj != null && ewmObj.ToString()?.Length < 500)
                    {
                        // 生成二维码图片base64字符串
                        // var base64 = StringToQrcodeBase64(ewmObj.ToString());
                        // qrData["ewm"] = base64;
                        // Console.WriteLine("生成二维码图片base64字符串");
                        //前端使用示例: <img src='data:image/png;base64,{base64}' width='200' />
                    }
                    StringToQrcode(ewmObj.ToString());
                    var faceTimeout = TimeSpan.FromSeconds(300);
                    Console.WriteLine($"请在{faceTimeout.TotalSeconds}秒内输入内容，过期时间为: {DateTime.UtcNow.Add(faceTimeout).ToString("yyyy-MM-dd HH:mm:ss")}");
                    var inputFaceTask = Task.Run(Console.ReadLine);
                    var completedFaceTask = await Task.WhenAny(inputFaceTask, Task.Delay(faceTimeout));
                    if (completedFaceTask == inputFaceTask)
                    {
                        Console.WriteLine($"输入: {inputFaceTask.Result}");
                        // 2. 认证完成后获取人脸二维码认证状态
                        /*
                        * @see https://fa-piao.com/doc.html#api4?source=github
                        */
                        var rzid = qrData.GetValueOrDefault("rzid")?.ToString() ?? "";
                        var faceStatusResponse = client.GetFaceState(nsrsbh, rzid, username, "1");
                        if (faceStatusResponse.Data != null && faceStatusResponse.Data.GetValueOrDefault("slzt") != null)
                        {
                            var slzt = faceStatusResponse.Data.GetValueOrDefault("slzt")?.ToString() ?? "";
                            if (slzt == "2")
                            {
                                Console.WriteLine("人脸认证成功");
                                Console.WriteLine("请再次调用BlueTicket");
                                invoiceResponse = BlueTicket();
                                if (invoiceResponse.Code == 200)
                                {
                                    DownloadPdfOfdXml(invoiceResponse.Data["fphm"].ToString(), invoiceResponse.Data["kprq"].ToString());
                                }
                                else
                                {
                                    Console.WriteLine(invoiceResponse.Code + "开具蓝票失败: " + invoiceResponse.Msg);
                                }
                                break;
                            }
                            else
                            {
                                Console.WriteLine(slzt == "1" ? "人脸未认证" : "人脸认证二维码过期");
                                break;
                            }
                        }
                        else
                        {
                            Console.WriteLine("获取人脸二维码认证状态失败: " + faceStatusResponse.Msg);
                            break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("人脸认证 超时！");
                    }
                    break;
                case 401:
                    Console.WriteLine("授权失败:" + invoiceResponse.Msg);
                    Console.WriteLine("401  token过期 重新获取并缓存token");
                    GetToken(true);
                    Console.WriteLine("再调用BlueTicket");
                    invoiceResponse = BlueTicket();
                    if (invoiceResponse.Code == 200)
                    {
                        DownloadPdfOfdXml(invoiceResponse.Data["fphm"].ToString(), invoiceResponse.Data["kprq"].ToString());
                    }
                    else
                    {
                        Console.WriteLine(invoiceResponse.Code + "开具蓝票失败: " + invoiceResponse.Msg);
                    }
                    break;
                default:
                    // Console.WriteLine("参数:" + invoiceParams);
                    Console.WriteLine(invoiceResponse.Code + "异常" + invoiceResponse.Msg);
                    break;
            }


        }
        catch (Exception e)
        {
            Console.WriteLine("系统错误：" + e.Message);
            Console.WriteLine(e);
        }
    }

    // 字符串转二维码 在命令行输出
    public static void StringToQrcode(String str)
    {
        var writer = new QRCodeWriter();
        var hints = new Dictionary<EncodeHintType, object>
        {
            [EncodeHintType.MARGIN] = 2,
            [EncodeHintType.ERROR_CORRECTION] = "M"
        };
        var matrix = writer.encode(str, BarcodeFormat.QR_CODE, 0, 0, hints);
        var width = matrix.Width + 4;
        var height = matrix.Height + 4;
        for (var y = 0; y < height; y += 2)
        {
            var line = new StringBuilder(width);
            for (var x = 0; x < width; x++)
            {
                var top = IsDark(matrix, x - 2, y - 2);
                var bottom = IsDark(matrix, x - 2, y - 1);
                line.Append(top
                    ? (bottom ? "█" : "▀")
                    : (bottom ? "▄" : " "));
            }
            Console.WriteLine(line.ToString());
        }
    }
    public static bool IsDark(ZXing.Common.BitMatrix matrix, int x, int y)
    {
        if (x < 0 || y < 0 || x >= matrix.Width || y >= matrix.Height)
        {
            return false;
        }

        return matrix[x, y];
    }

    public static void DownloadPdfOfdXml(String fphm, String kprq)
    {
        /*
         * 获取销项数电版式文件
         * @see https://fa-piao.com/doc.html#api7?source=github
         *
         */
        var pdfParams = new Dictionary<string, object>
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
            Console.WriteLine(JsonSerializer.Serialize(pdfResponse.Data, new JsonSerializerOptions
            {
                WriteIndented = true
            }));
        }
    }
    public static ApiResponse<Dictionary<string, object>> BlueTicket()
    {
        /*
         *
         * 开票税额计算demo
         * @see https://github.com/fapiaoapi/invoice-sdk-java/blob/master/examples/TaxExample.java
        */
        // 开具蓝票参数
        var invoiceParams = new Dictionary<string, object>
        {
            ["fplxdm"] = "82",
            ["fpqqlsh"] = appKey + DateTimeOffset.Now.ToUnixTimeMilliseconds(),
            ["ghdwmc"] = "个人",
            ["hjje"] = 396.04,
            ["hjse"] = 3.96,
            ["jshj"] = 400,
            ["kplx"] = 0,
            ["username"] = username,
            ["xhdwdzdh"] = xhdwdzdh,
            ["xhdwmc"] = title,
            ["xhdwsbh"] = nsrsbh,
            ["xhdwyhzh"] = xhdwyhzh,
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
        return client.BlueTicket(invoiceParams);
    }

    // 获取token
    public static void GetToken(Boolean forceUpdate)
    {
        var key = nsrsbh + "@" + username + "@TOKEN";
        var redisService = redis.GetDatabase();
        if (forceUpdate)
        {
            /*
            * 获取授权
            * @see https://fa-piao.com/doc.html#api1?source=github
            *
            */
            var authResponse = client.GetAuthorization(nsrsbh, type);
            if (authResponse.IsSuccess)
            {
                token = authResponse.Data.Token;
                client.SetAuthorization(token);
                redisService.StringSet(key, token, TimeSpan.FromDays(30));
            }
        }
        else
        {
            token = redisService.StringGet(key);
            if (token != null)
            {
                client.SetAuthorization(token);
                Console.WriteLine("从Redis获取token成功");
            }
            else
            {
                var authResponse = client.GetAuthorization(nsrsbh, type);
                if (authResponse.IsSuccess)
                {
                    token = authResponse.Data.Token;
                    client.SetAuthorization(token);
                    redisService.StringSet(key, token, TimeSpan.FromDays(30));
                }
            }
        }
    }

    // 生成二维码图片base64
    [SupportedOSPlatform("windows6.1")]
    public static string StringToQrcodeBase64(string str)
    {
        var writer = new QRCodeWriter();
        var hints = new Dictionary<EncodeHintType, object>
        {
            [EncodeHintType.MARGIN] = 2,
            [EncodeHintType.ERROR_CORRECTION] = "M"
        };
        var matrix = writer.encode(str, BarcodeFormat.QR_CODE, 300, 300, hints);

        using var bitmap = new Bitmap(matrix.Width, matrix.Height);
        for (var y = 0; y < matrix.Height; y++)
        {
            for (var x = 0; x < matrix.Width; x++)
            {
                bitmap.SetPixel(x, y, matrix[x, y] ? Color.Black : Color.White);
            }
        }

        using var ms = new MemoryStream();
        bitmap.Save(ms, ImageFormat.Png);
        return Convert.ToBase64String(ms.ToArray());
    }
}

public static class LazyConnection
{
    private static Lazy<ConnectionMultiplexer> _lazy;
    private static string _connectionString;

    public static ConnectionMultiplexer Connect(string connectionString)
    {
        if (_lazy == null || _connectionString != connectionString)
        {
            _connectionString = connectionString;
            _lazy = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(connectionString));
        }
        return _lazy.Value;
    }
}

```

[基础开票示例](Example/BasicExample.cs)
[红冲示例](Example/RedInvoiceExample.cs)
[税额计算示例](Example/TaxExample.cs)


