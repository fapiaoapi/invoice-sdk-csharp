using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Tax.Invoice.Util;

public static class OtherUtil
{
    public static string GenerateInvoiceSerialNumber(string? prefix = null)
    {
        prefix = string.IsNullOrWhiteSpace(prefix) ? "FP" : prefix;
        var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        var random = Random.Shared.Next(1000, 10000);
        return $"{prefix}{timestamp}{random}";
    }

    public static bool ValidateTaxNumber(string? nsrsbh)
    {
        if (string.IsNullOrWhiteSpace(nsrsbh))
        {
            return false;
        }

        if (nsrsbh.Length == 18)
        {
            return Regex.IsMatch(nsrsbh, "^[0-9A-Z]{18}$");
        }

        if (nsrsbh.Length == 15)
        {
            return Regex.IsMatch(nsrsbh, "^[0-9]{15}$");
        }

        return false;
    }

    public static bool ValidateMobile(string? mobile)
    {
        if (string.IsNullOrWhiteSpace(mobile))
        {
            return false;
        }
        return Regex.IsMatch(mobile, "^1[3-9]\\d{9}$");
    }

    public static bool ValidateEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }
        return Regex.IsMatch(email, "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$");
    }

    public static decimal CalculateTax(decimal amount, decimal taxRate, bool isIncludeTax, int newScale)
    {
        if (taxRate <= 0)
        {
            return 0m;
        }

        decimal tax;
        if (isIncludeTax)
        {
            tax = 1m / (1m + taxRate) * taxRate * amount;
        }
        else
        {
            tax = amount * taxRate;
        }

        return Math.Round(tax, newScale, MidpointRounding.AwayFromZero);
    }

    public static decimal CalculateAmountWithoutTax(decimal amount, decimal taxRate, int newScale)
    {
        var value = amount / (1m + taxRate);
        return Math.Round(value, newScale, MidpointRounding.AwayFromZero);
    }

    public static string AmountToChinese(double amount)
    {
        if (amount == 0)
        {
            return "零元整";
        }

        var chnNumChar = new[] { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖" };
        var chnUnitChar = new[] { "", "拾", "佰", "仟", "万", "拾", "佰", "仟", "亿", "拾", "佰", "仟", "万", "拾", "佰", "仟" };
        var chnUnitSection = new[] { "", "万", "亿", "万亿" };

        var integerPart = (long)amount;
        var decimalPart = (int)Math.Round((amount - integerPart) * 100, MidpointRounding.AwayFromZero);
        var chinese = new StringBuilder();

        if (integerPart > 0)
        {
            var integerStr = integerPart.ToString(CultureInfo.InvariantCulture);
            var integerLen = integerStr.Length;
            var section = 0;
            var sectionPos = 0;
            var zero = true;

            for (var i = integerLen - 1; i >= 0; i--)
            {
                var digit = integerStr[integerLen - i - 1] - '0';
                if (digit == 0)
                {
                    zero = true;
                }
                else
                {
                    if (zero)
                    {
                        chinese.Append(chnNumChar[0]);
                    }
                    zero = false;
                    chinese.Append(chnNumChar[digit]).Append(chnUnitChar[i % 16]);
                }

                sectionPos++;
                if (sectionPos == 4)
                {
                    section++;
                    sectionPos = 0;
                    zero = true;
                    chinese.Append(chnUnitSection[section]);
                }
            }
            chinese.Append("元");
        }

        if (decimalPart > 0)
        {
            var jiao = decimalPart / 10;
            var fen = decimalPart % 10;
            if (jiao > 0)
            {
                chinese.Append(chnNumChar[jiao]).Append("角");
            }
            if (fen > 0)
            {
                chinese.Append(chnNumChar[fen]).Append("分");
            }
        }
        else
        {
            chinese.Append("整");
        }

        return chinese.ToString();
    }

    public static string AmountToChinese(string amount)
    {
        return double.TryParse(amount, NumberStyles.Any, CultureInfo.InvariantCulture, out var value)
            ? AmountToChinese(value)
            : "零元整";
    }

    public static string GenerateRandomString(int length)
    {
        if (length <= 0)
        {
            return string.Empty;
        }

        const string characters = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var random = Random.Shared;
        var buffer = new char[length];
        for (var i = 0; i < length; i++)
        {
            buffer[i] = characters[random.Next(characters.Length)];
        }
        return new string(buffer);
    }

    public static bool SaveBase64File(string base64Content, string filePath)
    {
        try
        {
            var fileContent = Convert.FromBase64String(base64Content);
            File.WriteAllBytes(filePath, fileContent);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static decimal Add(decimal a, decimal b)
    {
        return Math.Round(a + b, 2, MidpointRounding.AwayFromZero);
    }

    public static decimal Subtract(decimal a, decimal b)
    {
        return Math.Round(a - b, 2, MidpointRounding.AwayFromZero);
    }

    public static decimal Multiply(decimal a, decimal b)
    {
        return Math.Round(a * b, 2, MidpointRounding.AwayFromZero);
    }

    public static decimal Divide(decimal a, decimal b, int scale)
    {
        return Math.Round(a / b, scale, MidpointRounding.AwayFromZero);
    }
}
