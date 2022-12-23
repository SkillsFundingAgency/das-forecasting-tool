using System;
using SFA.DAS.HashingService;

namespace SFA.DAS.Forecasting.Application.Shared.Services
{
    public class DevHashingService: IHashingService
    {
        public string HashValue(long id)
        {
            switch (id)
            {
                case 497: return "MJK9XV";
                case 8509: return "MN4YKL";
                case 54321: return "RF45KJ";
                case 5521: return "ABC123";
            }

            return "MDDP87";
        }

        public string HashValue(Guid id)
        {
            throw new NotImplementedException();
        }

        public string HashValue(string id)
        {
            switch (id)
            {
                case "497": return "MJK9XV";
                case "8509": return "MN4YKL";
                case "54321": return "RF45KJ";
                case "5521": return "ABC123";
            }

            return "MDDP87";
        }

        public long DecodeValue(string id)
        {
            switch (id)
            {
                case "MJK9XV": return 497;
                case "MN4YKL": return 8509;
                case "RF45KJ": return 54321;
                case "ABC123": return 5521;
            }

            return 12345;
        }

        public Guid DecodeValueToGuid(string id)
        {
            throw new NotImplementedException();
        }

        public string DecodeValueToString(string id)
        {
            return "12345";
        }

        public bool TryDecodeValue(string input, out long output)
        {
            output = 12345;
            return true;
        }
    }
}