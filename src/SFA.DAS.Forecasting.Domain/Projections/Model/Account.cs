﻿namespace SFA.DAS.Forecasting.Domain.Projections.Model
{
    public class Account
    {
        public long EmployerAccountId { get; }
        public decimal Balance { get; }
        public decimal LevyDeclared { get; }

        public Account(long employerAccountId, decimal balance, decimal levy)
        {
            EmployerAccountId = employerAccountId;
            Balance = balance;
            LevyDeclared = levy;
        }
    }
}