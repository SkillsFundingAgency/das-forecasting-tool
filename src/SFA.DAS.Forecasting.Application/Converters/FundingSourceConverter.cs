using FundingSourcePayments = SFA.DAS.Forecasting.Models.Payments.FundingSource;
using FundingSource = SFA.DAS.Provider.Events.Api.Types.FundingSource;

namespace SFA.DAS.Forecasting.Application.Converters
{
	public static class FundingSourceConverter
	{
		public static FundingSourcePayments ConvertToPaymentsFundingSource(FundingSource commitmentFundingSource)
		{
			switch (commitmentFundingSource)
			{
				case FundingSource.Levy:
					return FundingSourcePayments.Levy;
				case FundingSource.LevyTransfer:
					return FundingSourcePayments.Transfer;
				case FundingSource.CoInvestedEmployer:
					return FundingSourcePayments.CoInvestedEmployer;
				case FundingSource.CoInvestedSfa:
					return FundingSourcePayments.CoInvestedSfa;
				case FundingSource.FullyFundedSfa:
					return FundingSourcePayments.FullyFundedSfa;
				default:
					return FundingSourcePayments.FullyFundedSfa;
			}
		}

		public static FundingSource ConvertToApiFundingSource(FundingSourcePayments commitmentFundingSource)
		{
			switch (commitmentFundingSource)
			{
				case FundingSourcePayments.Levy:
					return FundingSource.Levy;
				case FundingSourcePayments.Transfer:
					return FundingSource.LevyTransfer;
				case FundingSourcePayments.CoInvestedEmployer:
					return FundingSource.CoInvestedEmployer;
				case FundingSourcePayments.CoInvestedSfa:
					return FundingSource.CoInvestedSfa;
				case FundingSourcePayments.FullyFundedSfa:
					return FundingSource.FullyFundedSfa;
				default:
					return FundingSource.FullyFundedSfa;
			}
		}
	}
}
