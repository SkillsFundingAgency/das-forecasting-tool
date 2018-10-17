using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Application.Shared
{
    public static class StringExtensions
    {
        public static string HtmlDecode(this string input)
        {
            var output = WebUtility.HtmlDecode(input);
            return output;
        }
    }
}