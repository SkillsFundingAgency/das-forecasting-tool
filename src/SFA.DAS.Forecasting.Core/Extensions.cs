using System;
using System.Collections;
using Newtonsoft.Json;

namespace SFA.DAS.Forecasting.Core
{
    public static class Extensions
    {
        public static string ToJson(this object source)
        {
            return JsonConvert.SerializeObject(source);
        }

        public static string ToDebugJson<T>(this T source)
        {
            return $"Type: {(typeof(T).FullName)}, Data: {source?.ToJson() ?? "null"}";

        }

        public static bool IsNullDate(this DateTime? date)
        {
            return !date.HasValue || date.Value.Year == 1;
        }

        public static DateTime GetStartOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static bool IsLastDayOfMonth(this DateTime date)
        {
            return DateTime.DaysInMonth(date.Year, date.Month) == date.Day;
        }

        public static DateTime GetLastPaymentDate(this DateTime plannedEndDate)
        {
            return plannedEndDate.IsLastDayOfMonth() ? plannedEndDate.AddMonths(1) : plannedEndDate;
        }
    }
}