using System.Globalization;
using System.Text;
using Tax.Invoice.Util;

namespace Tax.Invoice.Example;

public static class TaxExample
{
    public static void Run()
    {
        // 设置标准输出流编码为 UTF-8
        Console.OutputEncoding = Encoding.UTF8;

        /**
         * 含税金额计算示例
         *
         *   不含税单价 = 含税单价/(1+ 税率)  noTaxDj = dj / (1 + sl)
         *   不含税金额 = 不含税单价*数量  noTaxJe = noTaxDj * spsl
         *   含税金额 = 含税单价*数量  je = dj * spsl
         *   税额 = 1 / (1 + 税率) * 税率 * 含税金额  se = 1 / (1 + sl) * sl * je
         *    hjse= se1 + se2 + ... + seN
         *    jshj= je1 + je2 + ... + jeN
         *   价税合计 =合计金额+合计税额 jshj = hjje + hjse
         *
         */

        /**
         * 含税计算示例1  无价格  无数量
         * @link https://fa-piao.com/fapiao.html?action=data1&source=github
         *
         */
        Example1();
        Console.WriteLine("---------------------------------------------");

        /**
         * 含税计算示例2  有价格 有数量
         * @link https://fa-piao.com/fapiao.html?action=data3&source=github
         *
         */
        Example2();
        Console.WriteLine("---------------------------------------------");

        /**
         * 含税计算示例3  有价格自动算数量  购买猪肉1000元,16.8元/斤
         * @link https://fa-piao.com/fapiao.html?action=data5&source=github
         *
         */
        Example3();
        Console.WriteLine("---------------------------------------------");

        /**
         * 含税计算示例4  有数量自动算价格  购买接口服务1000元7次
         * @link https://fa-piao.com/fapiao.html?action=data7&source=github
         *
         */
        Example4();
        Console.WriteLine("---------------------------------------------");

        /**
         * 不含税计算示例
         *  金额 = 单价 * 数量  je = dj * spsl
         *  税额 = 金额 * 税率  se = je * sl
         *   hjse= se1 + se2 + ... + seN
         *   hjje= je1 + je2 + ... + jeN
         *  价税合计 =合计金额+合计税额 jshj = hjje + hjse
         *
         */

        /**
         * 不含税计算示例1 无价格 无数量
         * @link https://fa-piao.com/fapiao.html?action=data2&source=github
         */
        NoTaxExample1();
        Console.WriteLine("---------------------------------------------");

        /**
         * 不含税计算示例2  有价格 有数量
         *   一阶水费 1吨，单价2元/吨，税率0.03
         *   二阶水费 1吨，单价3元/吨，税率0.01
         * @link https://fa-piao.com/fapiao.html?action=data4&source=github
         */
        NoTaxExample2();
        Console.WriteLine("---------------------------------------------");

        /**
         * 不含税计算示例3  有价格自动算数量  购买猪肉1000元,16.8元/斤
         * @link https://fa-piao.com/fapiao.html?action=data6&source=github
         *
         */
        NoTaxExample3();
        Console.WriteLine("---------------------------------------------");

        /**
         * 不含税计算示例4  有数量自动算价格  购买接口服务1000元7次
         *
         * @link https://fa-piao.com/fapiao.html?action=data8&source=github
         *
         */
        NoTaxExample4();
        Console.WriteLine("---------------------------------------------");

        /**
         * 免税计算示例
         *  金额 = 单价 * 数量  je = dj * spsl
         *  税额 = 0
         *  hjse = se1 + se2 + ... + seN
         *  jshj = je1 + je2 + ... + jeN
         *  价税合计 =合计金额+合计税额 jshj = hjje + hjse
         * @link https://fa-piao.com/fapiao.html?action=data9&source=github
         */
        TaxFreeExample();
    }

    /**
     * 含税计算示例1 - 无价格无数量
     */
    public static void Example1()
    {
        var hsbz = 1;
        var isIncludeTax = hsbz == 1;
        var amount = 200m;
        var sl = 0.01m;
        var se = OtherUtil.CalculateTax(amount, sl, isIncludeTax, 2);

        var data = new InvoiceData
        {
            Fyxm =
            [
                new InvoiceItem
                {
                    Fphxz = 0,
                    Hsbz = hsbz,
                    Spmc = "*软件维护服务*接口服务费",
                    Spbm = "3040201030000000000",
                    Je = amount,
                    Sl = sl,
                    Se = se
                }
            ]
        };

        CalculateTotals(data, isIncludeTax);
        Console.WriteLine("含税计算示例1  无价格  无数量: ");
        Console.WriteLine(data.ToJson());
    }

    /**
     * 含税计算示例2 - 有价格有数量
     */
    public static void Example2()
    {
        var hsbz = 1;
        var isIncludeTax = hsbz == 1;
        var spsl1 = 1m;
        var dj1 = 2m;
        var sl1 = 0.03m;
        var je1 = Math.Round(dj1 * spsl1, 2, MidpointRounding.AwayFromZero);
        var se1 = OtherUtil.CalculateTax(je1, sl1, isIncludeTax, 2);

        var spsl2 = 1m;
        var dj2 = 3m;
        var sl2 = 0.01m;
        var je2 = Math.Round(dj2 * spsl2, 2, MidpointRounding.AwayFromZero);
        var se2 = OtherUtil.CalculateTax(je2, sl2, isIncludeTax, 2);

        var data = new InvoiceData
        {
            Fyxm =
            [
                new InvoiceItem
                {
                    Fphxz = 0,
                    Hsbz = hsbz,
                    Spmc = "*水冰雪*一阶水费",
                    Spbm = "1100301030000000000",
                    Ggxh = "",
                    Dw = "吨",
                    Dj = dj1,
                    Spsl = spsl1,
                    Je = je1,
                    Sl = sl1,
                    Se = se1
                },
                new InvoiceItem
                {
                    Fphxz = 0,
                    Hsbz = hsbz,
                    Spmc = "*水冰雪*二阶水费",
                    Spbm = "1100301030000000000",
                    Ggxh = "",
                    Dw = "吨",
                    Dj = dj2,
                    Spsl = spsl2,
                    Je = je2,
                    Sl = sl2,
                    Se = se2
                }
            ]
        };

        CalculateTotals(data, isIncludeTax);
        Console.WriteLine("含税计算示例2  有价格 有数量: ");
        Console.WriteLine(data.ToJson());
    }

    /**
     * 含税计算示例3 - 有价格自动算数量
     */
    public static void Example3()
    {
        var hsbz = 1;
        var isIncludeTax = hsbz == 1;
        var amount = 1000m;
        var dj = 16.8m;
        var sl = 0.01m;
        var se = OtherUtil.CalculateTax(amount, sl, isIncludeTax, 2);

        var data = new InvoiceData
        {
            Fyxm =
            [
                new InvoiceItem
                {
                    Fphxz = 0,
                    Hsbz = hsbz,
                    Spmc = "*肉类*猪肉",
                    Spbm = "1030107010100000000",
                    Ggxh = "",
                    Dw = "斤",
                    Dj = dj,
                    Spsl = Math.Round(amount / dj, 13, MidpointRounding.AwayFromZero),
                    Je = amount,
                    Sl = sl,
                    Se = se
                }
            ]
        };

        CalculateTotals(data, isIncludeTax);
        Console.WriteLine("含税计算示例3  有价格自动算数量 购买猪肉1000元,16.8元/斤: ");
        Console.WriteLine(data.ToJson());
    }

    /**
     * 含税计算示例4 - 有数量自动算价格
     */
    public static void Example4()
    {
        var hsbz = 1;
        var isIncludeTax = hsbz == 1;
        var amount = 1000m;
        var spsl = 7m;
        var sl = 0.01m;
        var se = OtherUtil.CalculateTax(amount, sl, isIncludeTax, 2);

        var data = new InvoiceData
        {
            Fyxm =
            [
                new InvoiceItem
                {
                    Fphxz = 0,
                    Hsbz = hsbz,
                    Spmc = "*软件维护服务*接口服务费",
                    Spbm = "3040201030000000000",
                    Ggxh = "",
                    Dw = "次",
                    Dj = Math.Round(amount / spsl, 13, MidpointRounding.AwayFromZero),
                    Spsl = spsl,
                    Je = amount,
                    Sl = sl,
                    Se = se
                }
            ]
        };

        CalculateTotals(data, isIncludeTax);
        Console.WriteLine("含税计算示例4  有数量自动算价格 购买接口服务1000元7次: ");
        Console.WriteLine(data.ToJson());
    }

    /**
     * 计算发票数据的合计值
     */
    private static void CalculateTotals(InvoiceData data, bool isIncludeTax)
    {
        var jshj = 0m;
        var hjse = 0m;

        if (isIncludeTax)
        {
            foreach (var item in data.Fyxm)
            {
                jshj += item.Je;
                hjse += item.Se;
            }

            data.Jshj = Math.Round(jshj, 2, MidpointRounding.AwayFromZero);
            data.Hjse = Math.Round(hjse, 2, MidpointRounding.AwayFromZero);
            data.Hjje = Math.Round(jshj - hjse, 2, MidpointRounding.AwayFromZero);
        }
        else
        {
            var hjje = 0m;
            foreach (var item in data.Fyxm)
            {
                hjje += item.Je;
                hjse += item.Se;
            }
            data.Jshj = Math.Round(hjje + hjse, 2, MidpointRounding.AwayFromZero);
            data.Hjje = Math.Round(hjje, 2, MidpointRounding.AwayFromZero);
            data.Hjse = Math.Round(hjse, 2, MidpointRounding.AwayFromZero);
        }
    }

    /**
     * 不含税计算示例1 - 无价格无数量
     */
    public static void NoTaxExample1()
    {
        var hsbz = 0;
        var isIncludeTax = hsbz == 1;
        var amount = 200m;
        var sl = 0.01m;
        var se = OtherUtil.CalculateTax(amount, sl, isIncludeTax, 2);

        var data = new InvoiceData
        {
            Fyxm =
            [
                new InvoiceItem
                {
                    Fphxz = 0,
                    Hsbz = hsbz,
                    Spmc = "*软件维护服务*接口服务费",
                    Spbm = "3040201030000000000",
                    Je = amount,
                    Sl = sl,
                    Se = se
                }
            ]
        };

        CalculateTotals(data, isIncludeTax);
        Console.WriteLine("不含税计算示例1  无价格  无数量: ");
        Console.WriteLine(data.ToJson());
    }

    /**
     * 不含税计算示例2 - 有价格有数量
     */
    public static void NoTaxExample2()
    {
        var hsbz = 0;
        var isIncludeTax = hsbz == 1;
        var spsl1 = 1m;
        var dj1 = 2m;
        var sl1 = 0.03m;
        var je1 = Math.Round(dj1 * spsl1, 2, MidpointRounding.AwayFromZero);
        var se1 = OtherUtil.CalculateTax(je1, sl1, isIncludeTax, 2);

        var spsl2 = 1m;
        var dj2 = 3m;
        var sl2 = 0.01m;
        var je2 = Math.Round(dj2 * spsl2, 2, MidpointRounding.AwayFromZero);
        var se2 = OtherUtil.CalculateTax(je2, sl2, isIncludeTax, 2);

        var data = new InvoiceData
        {
            Fyxm =
            [
                new InvoiceItem
                {
                    Fphxz = 0,
                    Hsbz = hsbz,
                    Spmc = "*水冰雪*一阶水费",
                    Spbm = "1100301030000000000",
                    Ggxh = "",
                    Dw = "吨",
                    Dj = dj1,
                    Spsl = spsl1,
                    Je = je1,
                    Sl = sl1,
                    Se = se1
                },
                new InvoiceItem
                {
                    Fphxz = 0,
                    Hsbz = hsbz,
                    Spmc = "*水冰雪*二阶水费",
                    Spbm = "1100301030000000000",
                    Ggxh = "",
                    Dw = "吨",
                    Dj = dj2,
                    Spsl = spsl2,
                    Je = je2,
                    Sl = sl2,
                    Se = se2
                }
            ]
        };

        CalculateTotals(data, isIncludeTax);
        Console.WriteLine("不含税计算示例2  有价格 有数量: ");
        Console.WriteLine(data.ToJson());
    }

    /**
     * 不含税计算示例3 - 有价格自动算数量
     */
    public static void NoTaxExample3()
    {
        var hsbz = 0;
        var isIncludeTax = hsbz == 1;
        var amount = 1000m;
        var dj = 16.8m;
        var sl = 0.01m;
        var se = OtherUtil.CalculateTax(amount, sl, isIncludeTax, 2);

        var data = new InvoiceData
        {
            Fyxm =
            [
                new InvoiceItem
                {
                    Fphxz = 0,
                    Hsbz = hsbz,
                    Spmc = "*肉类*猪肉",
                    Spbm = "1030107010100000000",
                    Ggxh = "",
                    Dw = "斤",
                    Dj = dj,
                    Spsl = Math.Round(amount / dj, 13, MidpointRounding.AwayFromZero),
                    Je = amount,
                    Sl = sl,
                    Se = se
                }
            ]
        };

        CalculateTotals(data, isIncludeTax);
        Console.WriteLine("不含税计算示例3  有价格自动算数量 购买猪肉1000元,16.8元/斤: ");
        Console.WriteLine(data.ToJson());
    }

    /**
     * 不含税计算示例4 - 有数量自动算价格
     */
    public static void NoTaxExample4()
    {
        var hsbz = 0;
        var isIncludeTax = hsbz == 1;
        var amount = 1000m;
        var spsl = 7m;
        var sl = 0.01m;
        var se = OtherUtil.CalculateTax(amount, sl, isIncludeTax, 2);

        var data = new InvoiceData
        {
            Fyxm =
            [
                new InvoiceItem
                {
                    Fphxz = 0,
                    Hsbz = hsbz,
                    Spmc = "*软件维护服务*接口服务费",
                    Spbm = "3040201030000000000",
                    Ggxh = "",
                    Dw = "次",
                    Dj = Math.Round(amount / spsl, 13, MidpointRounding.AwayFromZero),
                    Spsl = spsl,
                    Je = amount,
                    Sl = sl,
                    Se = se
                }
            ]
        };

        CalculateTotals(data, isIncludeTax);
        Console.WriteLine("不含税计算示例4  有数量自动算价格 购买接口服务1000元7次: ");
        Console.WriteLine(data.ToJson());
    }

    /**
     * 免税计算示例
     */
    public static void TaxFreeExample()
    {
        var hsbz = 0;
        var isIncludeTax = hsbz == 1;
        var dj = 32263.98m;
        var sl = 0m;
        var se = 0m;

        var data = new InvoiceData
        {
            Fyxm =
            [
                new InvoiceItem
                {
                    Fphxz = 0,
                    Hsbz = hsbz,
                    Spmc = "*经纪代理服务*国际货物运输代理服务",
                    Spbm = "3040802010200000000",
                    Ggxh = "",
                    Dw = "次",
                    Spsl = 1m,
                    Dj = dj,
                    Je = dj,
                    Sl = sl,
                    Se = se,
                    Yhzcbs = 1,
                    Lslbs = 1,
                    Zzstsgl = "免税"
                }
            ]
        };

        CalculateTotals(data, isIncludeTax);
        Console.WriteLine("免税计算示例: ");
        Console.WriteLine(data.ToJson());
    }

    /**
     * 发票数据类
     */
    private sealed class InvoiceData
    {
        public decimal Hjje { get; set; }
        public decimal Hjse { get; set; }
        public decimal Jshj { get; set; }
        public List<InvoiceItem> Fyxm { get; set; } = [];

        public string ToJson()
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine("  \"hjje\": " + Hjje.ToString(CultureInfo.InvariantCulture) + ",");
            sb.AppendLine("  \"hjse\": " + Hjse.ToString(CultureInfo.InvariantCulture) + ",");
            sb.AppendLine("  \"jshj\": " + Jshj.ToString(CultureInfo.InvariantCulture) + ",");
            sb.AppendLine("  \"fyxm\": [");
            for (var i = 0; i < Fyxm.Count; i++)
            {
                sb.Append(Fyxm[i].ToJson());
                sb.AppendLine(i < Fyxm.Count - 1 ? "," : "");
            }
            sb.AppendLine("  ]");
            sb.Append("}");
            return sb.ToString();
        }
    }

    /**
     * 发票项目类
     */
    private sealed class InvoiceItem
    {
        public int Fphxz { get; set; }
        public int Hsbz { get; set; }
        public string Spmc { get; set; } = "";
        public string Spbm { get; set; } = "";
        public string Ggxh { get; set; } = "";
        public string Dw { get; set; } = "";
        public decimal Dj { get; set; }
        public decimal Spsl { get; set; }
        public decimal Je { get; set; }
        public decimal Sl { get; set; }
        public decimal Se { get; set; }
        public int Yhzcbs { get; set; }
        public int Lslbs { get; set; }
        public string Zzstsgl { get; set; } = "";

        public string ToJson()
        {
            var sb = new StringBuilder();
            sb.AppendLine("    {");
            sb.AppendLine("      \"fphxz\": " + Fphxz + ",");
            sb.AppendLine("      \"hsbz\": " + Hsbz + ",");
            sb.AppendLine("      \"spmc\": \"" + Spmc + "\",");
            sb.AppendLine("      \"spbm\": \"" + Spbm + "\",");
            sb.AppendLine("      \"ggxh\": \"" + Ggxh + "\",");
            sb.AppendLine("      \"dw\": \"" + Dw + "\",");
            sb.AppendLine("      \"dj\": " + Dj.ToString(CultureInfo.InvariantCulture) + ",");
            sb.AppendLine("      \"spsl\": " + Spsl.ToString(CultureInfo.InvariantCulture) + ",");
            sb.AppendLine("      \"je\": " + Je.ToString(CultureInfo.InvariantCulture) + ",");
            sb.AppendLine("      \"sl\": " + Sl.ToString(CultureInfo.InvariantCulture) + ",");
            sb.AppendLine("      \"se\": " + Se.ToString(CultureInfo.InvariantCulture) + ",");
            sb.AppendLine("      \"yhzcbs\": " + Yhzcbs + ",");
            sb.AppendLine("      \"lslbs\": " + Lslbs + ",");
            sb.AppendLine("      \"zzstsgl\": \"" + Zzstsgl + "\"");
            sb.Append("    }");
            return sb.ToString();
        }
    }
}
