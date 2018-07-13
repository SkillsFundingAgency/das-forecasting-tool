namespace SFA.DAS.Forecasting.Models.Payments
{
    public enum FundingSource: byte
    {
		Levy = 1,
		CoInvestedSfa = 2,
		CoInvestedEmployer = 3,
		FullyFundedSfa = 4,
		Transfer = 5
    }
}