namespace SFA.DAS.Forecasting.Models.Balance
{
    public class Account
    {
        public long EmployerAccountId { get; }
        public decimal Balance { get; }
        public decimal LevyDeclared { get; }
        public decimal TransferAllowance { get; }
        public decimal RemainingTransferBalance { get; }

        public Account(long employerAccountId, decimal balance, decimal levy, decimal transferAllowance, decimal remainingTransferBalance)
        {
            EmployerAccountId = employerAccountId;
            Balance = balance;
            LevyDeclared = levy;
            TransferAllowance = transferAllowance;
            RemainingTransferBalance = remainingTransferBalance;
        }
    }
}