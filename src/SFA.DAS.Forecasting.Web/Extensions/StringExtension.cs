using System.Text.RegularExpressions;

namespace SFA.DAS.Forecasting.Web.Extensions
{
    public static class StringExtension
    {
        public static decimal ToDecimal(this string str)
        {
            if (string.IsNullOrWhiteSpace(str) || str.StartsWith("-"))
            {
                return 0M;
            }
                
            var text = Regex.Replace(str, "[^0-9]", "");
            return decimal.TryParse(text, out var d) ? d : 0M;
        }
    }
}