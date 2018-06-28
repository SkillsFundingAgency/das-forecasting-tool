using System.Text.RegularExpressions;

namespace SFA.DAS.Forecasting.Web.Extensions
{
    public static class StringExtension
    {
        public static decimal ToDecimal(this string str)
        {
            var text = Regex.Replace(str ?? string.Empty, "[^0-9]", "");
            if(decimal.TryParse(text, out decimal d))
                return d;
            return 0M;
        }
    }
}