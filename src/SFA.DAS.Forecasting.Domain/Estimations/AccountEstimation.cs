using System;
using System.Collections.Generic;
using SFA.DAS.Forecasting.Models.Estimation;
using SFA.DAS.Forecasting.Models.Projections;

namespace SFA.DAS.Forecasting.Domain.Estimations
{
    public class AccountEstimation
    {
        private readonly AccountEstimationModel _model;

        public IReadOnlyCollection<VirtualApprenticeship> VirtualApprenticeships => _model.Apprenticeships.AsReadOnly();
        public string Name => _model.EstimationName;
        public bool HasValidApprenticeships => throw new NotImplementedException();

        public AccountEstimation(AccountEstimationModel model)
        {
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public bool AddVirtualAppreniceship(VirtualApprenticeship virtualApprenticeship)
        {
            throw new NotImplementedException();
        }

        public void RemoveVirtualApprenticeship(string virtualApprenticeshipId)
        {
            throw new NotImplementedException();
        }
    }
}