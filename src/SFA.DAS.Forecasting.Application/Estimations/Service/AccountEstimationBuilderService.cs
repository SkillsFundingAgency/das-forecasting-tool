using SFA.DAS.Forecasting.Models.Estimation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Application.Estimations.Service
{
    public interface IAccountEstimationBuilderService
    {
       Task<AccountEstimationModel> CostBuildEstimations(string accountId, string estimationName);
    }


    public class AccountEstimationBuilderService
    {
    }
}
