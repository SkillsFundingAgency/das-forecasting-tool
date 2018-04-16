using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Forecasting.Domain.Estimations.Validation.VirtualApprenticeships;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Domain.Estimations
{
    public class AccountEstimation
    {
        internal AccountEstimationModel Model { get; }
        private readonly IVirtualApprenticeshipValidator _validator;

        public IReadOnlyCollection<VirtualApprenticeship> VirtualApprenticeships => Model.Apprenticeships.AsReadOnly();
        public string Name => Model.EstimationName;
        public bool HasValidApprenticeships => Model.Apprenticeships.Any(); //TODO: will also need to make sure all courses start after today
        public long EmployerAccountId => Model.EmployerAccountId;

        public AccountEstimation(AccountEstimationModel model, IVirtualApprenticeshipValidator validator)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public VirtualApprenticeship AddVirtualApprenticeship(string courseId, string courseTitle, int level, int startMonth, int startYear, int numberOfApprentices, int numberOfMonths, decimal totalCost)
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

        public bool RemoveVirtualApprenticeship(string virtualApprenticeshipId)
        {
            var virtualApprenticeship = Model.Apprenticeships.FirstOrDefault(apprenticeship =>
                apprenticeship.Id.Equals(virtualApprenticeshipId, StringComparison.OrdinalIgnoreCase));
            return virtualApprenticeship != null && Model.Apprenticeships.Remove(virtualApprenticeship);
        }
    }
}