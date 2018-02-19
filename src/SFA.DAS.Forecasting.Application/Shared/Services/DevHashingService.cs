using System;
using SFA.DAS.HashingService;

namespace SFA.DAS.Forecasting.Application.Shared.Services
{
    public class DevHashingService: IHashingService
    {
        public string HashValue(long id)
        {
            return "MDDP87";
        }

        public string HashValue(Guid id)
        {
            throw new NotImplementedException();
        }

        public string HashValue(string id)
        {
            return "MDDP87";
        }

        public long DecodeValue(string id)
        {
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
    }
}