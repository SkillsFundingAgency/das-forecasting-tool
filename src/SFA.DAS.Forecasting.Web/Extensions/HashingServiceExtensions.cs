using SFA.DAS.HashingService;
using System;

namespace SFA.DAS.Forecasting.Web.Extensions
{
    public static class HashingServiceExtensions
    {
        public static bool TryDecodeValue(this IHashingService hashingService, string input, out long result)
        {
            try
            {
                result = hashingService.DecodeValue(input);
                return true;
            }
            catch (Exception)
            {
                result = default(long);
                return false;
            }
        }
    }
}