namespace SFA.DAS.Forecasting.Models.Payments
{
    public enum FundingSource: byte
    {
		Levy = 1,
		Transfer = 2,
		CoInvestedSfa = 3,
		CoInvestedEmployer = 4,
		FullyFundedSfa = 5,
        ApprovedPledgeApplication = 6,
        AcceptedPledgeApplication = 7
	}
}