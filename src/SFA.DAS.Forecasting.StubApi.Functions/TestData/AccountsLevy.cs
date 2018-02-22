using System;
using System.Collections.Generic;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.Forecasting.AcceptanceTests.EmployerApiStub.TestData
{
    public class AccountsLevy
    {
        public static List<LevyDeclarationViewModel> GetLevy(string accountId)
        {
            return new List<LevyDeclarationViewModel>
            {
                new LevyDeclarationViewModel
                {
                    HashedAccountId = accountId,
                    PayrollYear = "18-19",
                    PayrollMonth = 1,
                    CreatedDate = DateTime.UtcNow,
                    PayeSchemeReference = "World",
                    TotalAmount = 200
                },
                new LevyDeclarationViewModel
                {
                    HashedAccountId = accountId,
                    PayrollYear = "18-19",
                    PayrollMonth = 2,
                    CreatedDate = DateTime.UtcNow,
                    PayeSchemeReference = "World",
                    TotalAmount = 300
                },
                new LevyDeclarationViewModel
                {
                    HashedAccountId = accountId,
                    PayrollYear = "17-18",
                    PayrollMonth = 12,
                    CreatedDate = DateTime.UtcNow,
                    PayeSchemeReference = "World",
                    TotalAmount = 300
                }
            };
        }

        public static AccountDetailViewModel GetAccountDetail(decimal balance)
        {
            return new AccountDetailViewModel
            {
                AccountId = 12345,
                Balance = balance,
                DasAccountName = "Test Employer",
                DateRegistered = DateTime.Today,
                HashedAccountId = "MDDP87",
            };
        }
    }
}
