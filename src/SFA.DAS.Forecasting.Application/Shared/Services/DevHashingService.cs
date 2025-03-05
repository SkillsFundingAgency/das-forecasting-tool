using SFA.DAS.Encoding;

namespace SFA.DAS.Forecasting.Application.Shared.Services;

public class DevHashingService: IEncodingService
{
    public string Encode(long value, EncodingType encodingType)
    {
        switch (value)
        {
            case 497: return "MJK9XV";
            case 8509: return "MN4YKL";
            case 54321: return "RF45KJ";
            case 5521: return "ABC123";
        }

        return "MDDP87";
    }

    public long Decode(string value, EncodingType encodingType)
    {
        switch (value)
        {
            case "MJK9XV": return 497;
            case "MN4YKL": return 8509;
            case "RF45KJ": return 54321;
            case "ABC123": return 5521;
        }

        return 12345;
    }

    public bool TryDecode(string encodedValue, EncodingType encodingType, out long decodedValue)
    {
        decodedValue = 12345;
        return true;
    }
}