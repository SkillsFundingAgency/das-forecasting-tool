using System.Text.RegularExpressions;
using Microsoft.Ajax.Utilities;

namespace SFA.DAS.Forecasting.Web.Extensions
{
    public static class StringExtension
    {
        public static decimal ToDecimal(this string str)
        {
            if (str.IsNullOrWhiteSpace() || str.StartsWith("-"))
            {
                return 0M;
            }
                
            var text = Regex.Replace(str, "[^0-9]", "");
            return decimal.TryParse(text, out var d) ? d : 0M;
        }
    }
}