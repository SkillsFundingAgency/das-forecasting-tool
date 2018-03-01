using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Web.AcceptanceTests.Helpers
{
    static class StringHelper
    {
        private static readonly CultureInfo UnitedKingdom =
            CultureInfo.GetCultureInfo("en-GB");

        public static string CurrencyConverter(decimal value)
        {
            return value.ToString("C0", UnitedKingdom);
        }
    }
}
