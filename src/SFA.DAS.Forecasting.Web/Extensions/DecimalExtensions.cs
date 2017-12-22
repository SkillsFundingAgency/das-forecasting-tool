namespace SFA.DAS.Forecasting.Web.Extensions
{
    public static class DecimalExtensions
    {
        public static string FormatCost(this decimal value)
        {
            return $"£{value:n0}";
        }
    }
}