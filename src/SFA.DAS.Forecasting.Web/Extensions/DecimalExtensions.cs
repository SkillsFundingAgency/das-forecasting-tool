using System.Globalization;

namespace SFA.DAS.Forecasting.Web.Extensions
{
    public static class DecimalExtensions
    {
        public static string FormatCost(this decimal value)
        {
            return value.ToString("C0", CultureInfo.GetCultureInfo("en-GB"));
        }

        public static string FormatCost(this decimal? value)
        {
            return value?.ToString("C0", CultureInfo.GetCultureInfo("en-GB"));
        }

        public static string FormatValue(this decimal? value)
        {
            return value.HasValue ?
                FormatValue(value.Value):
                null;
        }

        public static string FormatValue(this decimal value)
        {
            return $"{value:n0}";
        }
    }
}