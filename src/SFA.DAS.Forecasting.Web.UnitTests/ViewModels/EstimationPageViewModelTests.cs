using NUnit.Framework;
using SFA.DAS.Forecasting.Models.Payments;
using SFA.DAS.Forecasting.Web.ViewModels;
using System.Collections.Generic;

namespace SFA.DAS.Forecasting.Web.UnitTests.ViewModels
{
    [TestFixture]
    public class EstimationPageViewModelTests
    {
        [Test]
        public void Should_find_if_any_apprenticeships_is_transfer_funded()
        {
            var viewModel = GetViewModel(FundingSource.Transfer, FundingSource.Transfer);

            Assert.IsTrue(viewModel.AnyTransferApprenticeships);
            Assert.IsFalse(viewModel.AnyLevyApprenticeships);
        }

        [Test]
        public void Should_find_if_any_apprenticeships_is_Levy_funded()
        {
            var viewModel = GetViewModel(FundingSource.Levy, FundingSource.Levy);

            Assert.IsFalse(viewModel.AnyTransferApprenticeships);
            Assert.IsTrue(viewModel.AnyLevyApprenticeships);
        }

        [Test]
        public void Should_find_if_any_apprenticeships_is_Levy_and_Transfer_funded()
        {
            var viewModel = GetViewModel(FundingSource.Levy, FundingSource.Transfer);

            Assert.IsTrue(viewModel.AnyTransferApprenticeships);
            Assert.IsTrue(viewModel.AnyLevyApprenticeships);
        }

        private EstimationPageViewModel GetViewModel(FundingSource f1, FundingSource f2)
        {
            return new EstimationPageViewModel
            {
                Apprenticeships = new EstimationApprenticeshipsViewModel
                {
                    VirtualApprenticeships = new List<EstimationApprenticeshipViewModel>
                {
                    new EstimationApprenticeshipViewModel
                    {
                        CourseTitle = "Title",
                        ApprenticesCount = 2,
                        FundingSource = f1
                    },
                    new EstimationApprenticeshipViewModel
                    {
                        CourseTitle = "Title",
                        ApprenticesCount = 2,
                        FundingSource = f2
                    }
                }
                }
            };
        }

    }
}
