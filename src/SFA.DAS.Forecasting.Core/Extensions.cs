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

        public static DateTime GeStartOfMonth(this DateTime date)
        {
            return date.AddDays(date.Day * -1).AddDays(1);
        }
    }
}