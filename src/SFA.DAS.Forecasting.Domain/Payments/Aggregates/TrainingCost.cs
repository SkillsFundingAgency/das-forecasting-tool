using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Payments.Entities;
using SFA.DAS.Forecasting.Domain.Payments.Repositories;

namespace SFA.DAS.Forecasting.Domain.Payments.Aggregates
{
    public class TrainingCost
    {
		private readonly ITrainingCostRepository _trainingCostStorage;

	    public TrainingCost(ITrainingCostRepository trainingCostStorage)
	    {
		    _trainingCostStorage = trainingCostStorage;
	    }

		public EmployerTotalCostOfTraining AggregateEmployerPayments(List<Payment> payments)
	    {
		    var response = new EmployerTotalCostOfTraining
		    {
			    EmployerAccountId = payments.FirstOrDefault().EmployerAccountId,
			    PeriodMonth = payments.FirstOrDefault().CollectionPeriod.Month,
			    PeriodYear = payments.FirstOrDefault().CollectionPeriod.Year,
			    TotalCostOfTraining = 0
		    };

		    foreach (var payment in payments)
		    {
			    response.TotalCostOfTraining += payment.Amount;
		    }

		    return response;
	    }

	    public bool IsAggregationAllowed(List<Payment> payments, int delayInSeconds)
	    {
		    var response = true;

		    foreach (var payment in payments)
		    {
			    if (payment.ReceivedTime > DateTime.Now.AddSeconds(delayInSeconds * -1))
			    {
				    response = false;
			    }
		    }

		    return response;
	    }

	    public async Task AddTotalCostOfTraining(EmployerTotalCostOfTraining employerTotalCostOfTraining)
	    {
		    await _trainingCostStorage.StoreTrainingCost(employerTotalCostOfTraining);
	    }
	}
}