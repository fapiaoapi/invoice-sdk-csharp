using System;
using System.Collections.Generic;
using Tax.Invoice;
using System.Text;

public class RedInvoiceExample
{
    /**
     * 示例入口，演示POST multipart/form-data请求
     */
    public static void Main(string[] args)
    {
        try
        {
            var appKey = "";
            var appSecret = "";
            var nsrsbh = "";
            var username = "";
            // var password = "";
            var type = "7";
            // var title = "";
            var fphm = "26502000000569538151";
            // var kprq = "";
            var token = "";

            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("dotnet " + Environment.Version);

            var client = new InvoiceClient(appKey, appSecret);
            /*
             * 获取授权Token文档
             * @see https://fa-piao.com/doc.html#api1?source=github
             */
            var authResponse = client.GetAuthorization(nsrsbh, type);
            if (authResponse.IsSuccess)
            {
                token = authResponse.Data.Token;
                client.SetAuthorization(token);
            }
            /*
               * 1. 数电申请红字前查蓝票信息接口
               * @link https://fa-piao.com/doc.html#api8?source=github
               */
            var queryInvoiceParams = new Dictionary<string, object>
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
                System.Threading.Thread.Sleep(2000);
                /*
                 * 2. 申请红字信息表
                 * @link https://fa-piao.com/doc.html#api9?source=github
                 */
                var applyRedParams = new Dictionary<string, object>
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
                    System.Threading.Thread.Sleep(2000);
                    /*
                     * 3. 开具红字发票
                     * @link https://fa-piao.com/doc.html#api10?source=github
                     */
                    var redInvoiceParams = new Dictionary<string, object>
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
        catch (Exception e)
        {
            Console.WriteLine("请求异常：" + e.Message);
            Console.WriteLine(e);
        }
    }
}
