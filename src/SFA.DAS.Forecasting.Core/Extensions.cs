using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    }
}