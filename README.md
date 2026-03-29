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
dotnet add package Tax.Invoice --version 1.0.2
```

[📦 查看NuGet最新版本](https://www.nuget.org/packages/Tax.Invoice)

本SDK基于 C# 12 与 .NET 10 开发

---

[📚 查看完整中文文档](https://fa-piao.com/doc.html?source=github) | [💡 更多示例代码](https://github.com/fapiaoapi/invoice-sdk-csharp/tree/master/Example)

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

## 🎯 快速开始：5分钟开出一张数电发票

```csharp
using System;
using System.Collections.Generic;
using System.Text;
using Tax.Invoice;
using System.Text.Json;
using StackExchange.Redis;
using QRCoder;
using System.Drawing;

public class BasicExample
{

    public static void Main(string[] args)
    {
        try
        {
            var appKey = "";
            var appSecret = "";
            var nsrsbh = ""; //统一社会信用代码
            var title = ""; //名称（营业执照）
            var username = ""; //手机号码（电子税务局）
            var password = ""; //个人用户密码（电子税务局）
            var type = "6";

            var fphm = "";
            var kprq = "";
            var token = "";
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("dotnet " + Environment.Version);
            var client = new InvoiceClient(appKey, appSecret);

            // dotnet add package StackExchange.Redis
            // 从Redis获取token
            var redis = LazyConnection.Connect("127.0.0.1:6379,password=test123456,abortConnect=false");
            var redisService = redis.GetDatabase();
            var key = nsrsbh + "@" + username + "@TOKEN";
            token = redisService.StringGet(key);
            if (token != null)
            {
                client.SetAuthorization(token);
                Console.WriteLine("从Redis获取token成功");
            }
            else
            {
                var authResponse = client.GetAuthorization(nsrsbh, type);
                // var authResponse = client.GetAuthorization(nsrsbh, type, username, password);
                if (authResponse.IsSuccess)
                {
                    token = authResponse.Data.Token;
                    client.SetAuthorization(token);
                    redisService.StringSet(key, token, TimeSpan.FromDays(30));
                }
            }

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
                ["xhdwdzdh"] = "重庆市渝北区龙溪街道丽园路xxx 1325580xxxx",
                ["xhdwmc"] = title,
                ["xhdwsbh"] = nsrsbh,
                ["xhdwyhzh"] = "工商银行xxx 15451211xxx",
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
                    /*
                     * 获取销项数电版式文件
                     * @see https://fa-piao.com/doc.html#api7?source=github
                     *
                     */
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
                        Console.WriteLine(JsonSerializer.Serialize(pdfResponse.Data, new JsonSerializerOptions
                        {
                            WriteIndented = true
                        }));
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
                    var smsCode = "764621";
                    var loginResponse2 = client.LoginDppt(nsrsbh, username, password, smsCode);
                    if (loginResponse2.Code == 200)
                    {
                        Console.WriteLine(JsonSerializer.Serialize(loginResponse2.Data, new JsonSerializerOptions
                        {
                            WriteIndented = true
                        }));
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
                        // 生成二维码图片base64字符串
                        Console.WriteLine("生成二维码图片base64字符串");
                        //dotnet add package QRCoder
                        //dotnet add package System.Drawing.Common
                        using var generator = new QRCodeGenerator();
                        using var qrCodeData = generator.CreateQrCode(ewmObj.ToString(), QRCodeGenerator.ECCLevel.Q);
                        using var qrCode = new PngByteQRCode(qrCodeData); // 使用 PngByte 效率更高
                        byte[] pngBytes = qrCode.GetGraphic(20); // 20 是每个像素的大小(像素密度)
                        string base64 = Convert.ToBase64String(pngBytes);
                        qrData["ewm"] = base64;
                        Console.WriteLine("二维码生成成功！");
                        //前端使用示例: <img src='data:image/png;base64,{base64}' width='200' />
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
            Console.WriteLine("请求异常：" + e.Message);
            Console.WriteLine(e);
        }
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


