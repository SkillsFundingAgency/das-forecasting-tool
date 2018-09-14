using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.Provider.Events.Api.Types;
using System.Collections.Generic;

namespace SFA.DAS.Forecasting.StubApi.Functions
{
    public static class StubDataStore
    {
        public static PageOfResults<Payment> PaymentsData { get; set; }
        public static IDictionary<string, IEnumerable<LevyDeclarationViewModel>> LevyData { get; set; }
            = new Dictionary<string, IEnumerable<LevyDeclarationViewModel>>();

        public static IDictionary<string, string> Apprenticeships { get; set; }
            = new Dictionary<string,string>();

        public static decimal Balance { get; set; } = 500000;
    }
}
