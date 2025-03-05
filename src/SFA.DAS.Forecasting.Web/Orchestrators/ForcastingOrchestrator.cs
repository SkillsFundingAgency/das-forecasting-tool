using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Web.Extensions;
using SFA.DAS.Forecasting.Domain.Projections.Services;
using SFA.DAS.Forecasting.Web.Orchestrators.Mappers;
using SFA.DAS.Forecasting.Web.ViewModels;
using SFA.DAS.Encoding;
using SFA.DAS.Forecasting.Core.Configuration;
using SFA.DAS.Forecasting.Domain.Commitments.Services;
using SFA.DAS.Forecasting.Models.Commitments;

namespace SFA.DAS.Forecasting.Web.Orchestrators;

public interface IForecastingOrchestrator
{
    Task<IEnumerable<BalanceCsvItemViewModel>> BalanceCsv(string hashedAccountId);
    Task<IEnumerable<ApprenticeshipCsvItemViewModel>> ApprenticeshipsCsv(string hashedAccountId);
}

public class ForecastingOrchestrator : IForecastingOrchestrator
{
    private readonly IEncodingService _encodingService;
    private readonly IAccountProjectionDataSession _accountProjection;
    private readonly ForecastingConfiguration _applicationConfiguration;
    private readonly IForecastingMapper _mapper;
    private readonly ICommitmentsDataService _commitmentsDataService;

    private static readonly DateTime BalanceMaxDate = DateTime.Parse("2019-05-01");

    public ForecastingOrchestrator(
        IEncodingService encodingService,
        IAccountProjectionDataSession accountProjection,
        ForecastingConfiguration applicationConfiguration,
        IForecastingMapper mapper,
        ICommitmentsDataService commitmentsDataService
    )
    {
        _encodingService = encodingService;
        _accountProjection = accountProjection;
        _applicationConfiguration = applicationConfiguration;
        _mapper = mapper;
        _commitmentsDataService = commitmentsDataService;
    }
    
    public async Task<IEnumerable<BalanceCsvItemViewModel>> BalanceCsv(string hashedAccountId)
    {
        var accountId = _encodingService.Decode(hashedAccountId, EncodingType.AccountId);

        return (await ReadProjection(accountId))
            .Projections
            .Select(m => _mapper.ToCsvBalance(m));
    }

    public async Task<IEnumerable<ApprenticeshipCsvItemViewModel>> ApprenticeshipsCsv(string hashedAccountId)
    {
        var accountId = _encodingService.Decode(hashedAccountId, EncodingType.AccountId);
        var commitments = await _commitmentsDataService.GetCurrentCommitments(accountId, _applicationConfiguration.LimitForecast ? BalanceMaxDate : (DateTime?)null);
        var csvCommitments = new List<CommitmentModel>();
        csvCommitments = csvCommitments.Concat(commitments.LevyFundedCommitments)
            .Concat(commitments.ReceivingEmployerTransferCommitments)
            .Concat(commitments.SendingEmployerTransferCommitments).OrderBy(c=>c.StartDate).ToList();

        return csvCommitments
            .Select(m => _mapper.ToCsvApprenticeship(m, accountId));
    }
    
    private async Task<ProjectionModel> ReadProjection(long accountId)
    {
        var result = await _accountProjection.Get(accountId);

        var date = result.FirstOrDefault()?.ProjectionCreationDate;

        var d = _mapper.MapProjections(result);

        var projections = d.Where(m => m.Date.IsAfterOrSameMonth(DateTime.Today.AddMonths(1)) && (!_applicationConfiguration.LimitForecast || m.Date < BalanceMaxDate))
            .OrderBy(m => m.Date)
            .Take(48)
            .ToList();

        return new ProjectionModel
        {
            CreatedOn = date,
            Projections = projections

        };
    }

    private struct ProjectionModel
    {
        public List<ProjectiontemViewModel> Projections { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}