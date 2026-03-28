using System.Text;
using Tax.Invoice.Model;

namespace Tax.Invoice.Example;

// 数电发票SDK使用示例
public static class RedInvoiceExample
{
    public static void Run()
    {
        // 显式设置 System.out 的编码为 UTF-8
        Console.OutputEncoding = Encoding.UTF8;

        // 配置信息
        var appKey = "";
        var appSecret = "";

        // 统一社会信用代码
        var nsrsbh = "";
        // 手机号码（电子税务局）
        var username = "";

        var fphm = "";
        var token = "";

        // 创建客户端
        var client = new InvoiceClient(appKey, appSecret);
        if (!string.IsNullOrWhiteSpace(token))
        {
            client.SetAuthorization(token);
        }
        else
        {
            /*
             * 获取授权Token文档
             * @see https://fa-piao.com/doc.html#api1?source=github
             */
            var authResponse = client.GetAuthorization(nsrsbh);
            if (authResponse.IsSuccess)
            {
                Console.WriteLine("授权成功，Token: " + authResponse.Data?.Token);
            }
        }

        /*
         * 1. 数电申请红字前查蓝票信息接口
         * @link https://fa-piao.com/doc.html#api8?source=github
         */
        var queryInvoiceParams = new Dictionary<string, object?>
        {
            ["nsrsbh"] = nsrsbh,
            ["fphm"] = fphm,
            ["username"] = username,
            ["sqyy"] = "2"
        };
        var queryInvoiceResponse = client.QueryBlueTicketInfo(queryInvoiceParams);

        if (queryInvoiceResponse.IsSuccess)
        {
            Console.WriteLine("1 可以申请红字");
            Thread.Sleep(2000);
            /*
             * 2. 申请红字信息表
             * @link https://fa-piao.com/doc.html#api9?source=github
             */
            var applyRedParams = new Dictionary<string, object?>
            {
                ["xhdwsbh"] = nsrsbh,
                ["yfphm"] = fphm,
                ["username"] = username,
                ["sqyy"] = "2",
                ["chyydm"] = "01"
            };

            var applyRedResponse = client.ApplyRedInfo(applyRedParams);
            if (applyRedResponse.IsSuccess)
            {
                Console.WriteLine("2 申请红字信息表");
                Thread.Sleep(2000);
                /*
                 * 3. 开具红字发票
                 * @link https://fa-piao.com/doc.html#api10?source=github
                 */
                var redInvoiceParams = new Dictionary<string, object?>
                {
                    ["fpqqlsh"] = "red" + fphm,
                    ["username"] = username,
                    ["xhdwsbh"] = nsrsbh
                };

                var applyRedData = applyRedResponse.Data;
                if (applyRedData == null || applyRedData.GetValueOrDefault("xxbbh") == null)
                {
                    Console.WriteLine("红字信息表返回字段缺失: " + applyRedResponse.Data);
                    return;
                }
                redInvoiceParams["tzdbh"] = applyRedData.GetValueOrDefault("xxbbh")?.ToString();
                redInvoiceParams["yfphm"] = fphm;

                var redInvoiceResponse = client.RedTicket(redInvoiceParams);
                if (redInvoiceResponse.IsSuccess)
                {
                    Console.WriteLine("3 负数开具成功");
                }
                else
                {
                    Console.WriteLine(redInvoiceResponse.Code + "数电票负数开具失败:" + redInvoiceResponse.Msg);
                    Console.WriteLine(redInvoiceResponse.Data);
                }
            }
            else
            {
                Console.WriteLine(applyRedResponse.Code + "申请红字信息表失败:" + applyRedResponse.Msg);
                Console.WriteLine(applyRedResponse.Data);
            }
        }
        else
        {
            Console.WriteLine(queryInvoiceResponse.Code + "查询发票信息失败:" + queryInvoiceResponse.Msg);
            Console.WriteLine(queryInvoiceResponse.Data);
        }
    }
}
