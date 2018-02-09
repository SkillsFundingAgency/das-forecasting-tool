using System;
using System.Collections.Generic;

namespace SFA.DAS.Forecasting.AcceptanceTests.EmployerApiStub
{
    public class TestData
    {
        public static List<LevyDeclarationViewModel> GetLevy(string accountId)
        {
            return new List<LevyDeclarationViewModel>
            {
                new LevyDeclarationViewModel
                {
                    HashedAccountId = accountId,
                    PayrollYear = "2018-19",
                    PayrollMonth = 1,
                    SubmissionDate = DateTime.UtcNow,
                    PayeSchemeReference = "World",
                    TotalAmount = 200
                },
                new LevyDeclarationViewModel
                {
                    HashedAccountId = accountId,
                    PayrollYear = "2017-18",
                    PayrollMonth = 12,
                    SubmissionDate = DateTime.UtcNow,
                    PayeSchemeReference = "World",
                    TotalAmount = 300
                }
            };
        }
    }
}
