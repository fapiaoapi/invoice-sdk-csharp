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
using System.Text;
using Tax.Invoice;

Console.OutputEncoding = Encoding.UTF8;

var appKey = "";
var appSecret = "";

var nsrsbh = "91500112MADFAXXXX";
var title = "XXXX科技有限公司";
var username = "1325580XXXX";
var password = "123456XXXX";
var token = "";
var type = "6";

var client = new InvoiceClient(appKey, appSecret);

var authResponse = client.GetAuthorization(nsrsbh, type);
if (!authResponse.IsSuccess || authResponse.Data?.Token is not { Length: > 0 })
{
    Console.WriteLine("授权失败: " + authResponse.Msg);
    return;
}

token = authResponse.Data.Token;
client.SetAuthorization(token);

var invoiceParams = new Dictionary<string, object?>
{
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
    ["fyxm[0][spbm]"] = "3040201030000000000"
};

var invoiceResponse = client.BlueTicket(invoiceParams);
Console.WriteLine($"{invoiceResponse.Code} {invoiceResponse.Msg}");
```

[基础开票示例](Example/BasicExample.cs)
[红冲示例](Example/RedInvoiceExample.cs)
[税额计算示例](Example/TaxExample.cs)


