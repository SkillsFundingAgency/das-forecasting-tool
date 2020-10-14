using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Domain.Estimations.Validation.VirtualApprenticeships;
using SFA.DAS.Forecasting.Domain.Shared;
using SFA.DAS.Forecasting.Models.Estimation;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Domain.Estimations
{
    public class AccountEstimation
    {
        internal AccountEstimationModel Model { get; }
        private readonly IVirtualApprenticeshipValidator _validator;
        private readonly IDateTimeService _dateTimeService;

        public IReadOnlyCollection<VirtualApprenticeship> Apprenticeships => Model.Apprenticeships;

        public string Name => Model.EstimationName;
        public bool HasValidApprenticeships {
            get
            {
                if (!Model.Apprenticeships.Any())
                    return false;

                var latestPlannedEndDate = Model.Apprenticeships.Select(x => x.StartDate.AddMonths(x.TotalInstallments)).OrderByDescending(x => x).First().AddMonths(2).GetStartOfMonth();

                if (latestPlannedEndDate < _dateTimeService.GetCurrentDateTime().GetStartOfMonth())
                    return false;

                return true;
            }
       }

        public long EmployerAccountId => Model.EmployerAccountId;

        public AccountEstimation(AccountEstimationModel model, IVirtualApprenticeshipValidator validator, IDateTimeService dateTimeService)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _dateTimeService = dateTimeService;
        }

        public VirtualApprenticeship AddVirtualApprenticeship(string courseId, string courseTitle, int level, int startMonth, int startYear, int numberOfApprentices, int numberOfMonths, decimal totalCost, FundingSource fundingSource)
        {
            var virtualApprenticeship = new VirtualApprenticeship
            {
                CourseId = courseId,
                CourseTitle = courseTitle,
                Level = level,
                ApprenticesCount = numberOfApprentices,
                StartDate = new DateTime(startYear, startMonth, 1),
                TotalCost = totalCost,
                TotalInstallments = (short)numberOfMonths,
                FundingSource = fundingSource
            };
            var validationResults = _validator.Validate(virtualApprenticeship);
            if (!validationResults.All(result => result.IsValid))
                throw new InvalidOperationException($"The virtual apprenticeship is invalid.  Failures: {validationResults.Aggregate(string.Empty, (currText, failure) => $"{currText}{failure}, ")}");
            virtualApprenticeship.Id = Guid.NewGuid().ToString("N");
            virtualApprenticeship.TotalCompletionAmount = (totalCost / 100) * 20;
            virtualApprenticeship.TotalInstallmentAmount = ((totalCost / 100) * 80) / numberOfMonths;
            Model.Apprenticeships.Add(virtualApprenticeship);
            return virtualApprenticeship;
        }

        public VirtualApprenticeship UpdateApprenticeship(string apprenticeshipId, int startMonth, int startYear, int numberOfApprentices, short totalInstallments, decimal totalCost)
        {
            var apprenticeship = Model.Apprenticeships.Single(m => m.Id == apprenticeshipId);
            apprenticeship.TotalCost = totalCost;
            apprenticeship.TotalInstallments = totalInstallments;
            apprenticeship.ApprenticesCount = numberOfApprentices;
            apprenticeship.StartDate = new DateTime(startYear, startMonth, 1);

            var validationResults = _validator.Validate(apprenticeship);
            if (!validationResults.All(result => result.IsValid))
                throw new InvalidOperationException($"The virtual apprenticeship is invalid.  Failures: {validationResults.Aggregate(string.Empty, (currText, failure) => $"{currText}{failure}, ")}");

            apprenticeship.TotalCompletionAmount = (totalCost / 100) * 20;
            apprenticeship.TotalInstallmentAmount = ((totalCost / 100) * 80) / totalInstallments;

            return apprenticeship;
        }

        public bool RemoveVirtualApprenticeship(string virtualApprenticeshipId)
        {
            var virtualApprenticeship = Model.Apprenticeships.FirstOrDefault(apprenticeship =>
                apprenticeship.Id.Equals(virtualApprenticeshipId, StringComparison.OrdinalIgnoreCase));
            return virtualApprenticeship != null && Model.Apprenticeships.Remove(virtualApprenticeship);
        }

        public VirtualApprenticeship FindVirtualApprenticeship(string virtualApprenticeshipId)
        {
            var virtualApprenticeship = Model.Apprenticeships?
                .FirstOrDefault(apprenticeship => apprenticeship.Id.Equals(virtualApprenticeshipId, StringComparison.OrdinalIgnoreCase));
            return virtualApprenticeship;
        }
    }
}