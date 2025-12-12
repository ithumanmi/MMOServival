using System.Globalization;

namespace Hawky.IAP
{
    public class IAPService : RuntimeSingleton<IAPService>
    {
        public string GetRevenue(decimal amount)
        {
            decimal val = decimal.Multiply(amount, 0.63m);
            NumberFormatInfo n = new CultureInfo("en-US", false).NumberFormat;
            return val.ToString("F", n);
        }
    }
}
