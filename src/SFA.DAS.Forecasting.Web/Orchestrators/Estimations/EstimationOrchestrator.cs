﻿using SFA.DAS.Forecasting.Domain.Balance;
using SFA.DAS.Forecasting.Domain.Estimations;
using SFA.DAS.Forecasting.Models.Estimation;
using SFA.DAS.Forecasting.Web.Extensions;
using SFA.DAS.Forecasting.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Encoding;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Application.ExpiredFunds.Service;
using SFA.DAS.Forecasting.Models.Projections;

namespace SFA.DAS.Forecasting.Web.Orchestrators.Estimations
{
    public class EstimationOrchestrator : IEstimationOrchestrator
    {
        private readonly IAccountEstimationProjectionRepository _estimationProjectionRepository;
        private readonly IAccountEstimationRepository _estimationRepository;
        private readonly IEncodingService _encodingService;
        private readonly ICurrentBalanceRepository _currentBalanceRepository;
        private readonly IApprenticeshipCourseDataService _apprenticeshipCourseService;
        private readonly IExpiredFundsService _expiredFundsService;

        public EstimationOrchestrator(IAccountEstimationProjectionRepository estimationProjectionRepository,
            IAccountEstimationRepository estimationRepository,
            IEncodingService encodingService,
            ICurrentBalanceRepository currentBalanceRepository,
            IApprenticeshipCourseDataService apprenticeshipCourseService,
            IExpiredFundsService expiredFundsService)
        {
            _estimationProjectionRepository = estimationProjectionRepository ;
            _estimationRepository = estimationRepository;
            _encodingService = encodingService;
            _currentBalanceRepository = currentBalanceRepository;
            _apprenticeshipCourseService = apprenticeshipCourseService;
            _expiredFundsService = expiredFundsService;
        }

        public async Task<EstimationPageViewModel> CostEstimation(string hashedAccountId, string estimateName,
            bool? apprenticeshipRemoved)
        {
            var accountId = GetAccountId(hashedAccountId);
            await RefreshCurrentBalance(accountId);
            var accountEstimation = await _estimationRepository.Get(accountId);
            var estimationProjector = await _estimationProjectionRepository.Get(accountEstimation);
            estimationProjector.BuildProjections();
            var projection = estimationProjector.Projections.FirstOrDefault();
            var projectionType = projection?.ProjectionGenerationType ?? ProjectionGenerationType.LevyDeclaration;
            var expiredFunds = await _expiredFundsService.GetExpiringFunds(estimationProjector.Projections, accountId, projectionType, DateTime.UtcNow);

            if (expiredFunds.Any())
            {
                estimationProjector.ApplyExpiredFunds(expiredFunds);
            }

            var viewModel = new EstimationPageViewModel
            {
                HashedAccountId = hashedAccountId,
                EstimationName = accountEstimation == null ? estimateName : accountEstimation.Name,
                ApprenticeshipRemoved = apprenticeshipRemoved.GetValueOrDefault(),
                Apprenticeships = new EstimationApprenticeshipsViewModel
                {
                    VirtualApprenticeships = accountEstimation?.Apprenticeships?.Select(o =>
                        new EstimationApprenticeshipViewModel
                        {
                            Id = o.Id,
                            CompletionPayment = o.TotalCompletionAmount,
                            ApprenticesCount = o.ApprenticesCount,
                            CourseTitle = o.CourseTitle,
                            Level = o.Level,
                            MonthlyPayment = o.TotalInstallmentAmount,
                            MonthlyPaymentCount = o.TotalInstallments,
                            StartDate = o.StartDate,
                            TotalCost = o.TotalCost,
                            FundingSource = o.FundingSource
                        }).ToList(),
                },
                TransferAllowances = new EstimationTransferAllowanceVewModel
                {
                    AnnualTransferAllowance = estimationProjector.TransferAllowance,
                    Records = estimationProjector?.Projections?.Skip(1)
                        .Select(o => new EstimationTransferAllowance
                        {
                            Date = new DateTime(o.Year, o.Month, 1),
                            ActualCost = o.ActualCosts.TransferFundsOut,
                            EstimatedCost = o.TransferModelledCosts.TransferFundsOut,
                            RemainingAllowance = o.AvailableTransferFundsBalance
                        }).ToList(),
                },
                AccountFunds =
                    new AccountFundsViewModel
                    {
                        MonthlyInstallmentAmount = estimationProjector.MonthlyInstallmentAmount,
                        Records = GetAccountFunds(estimationProjector.Projections?.Skip(1))
                    }
            };
            return viewModel;
        }

        private long GetAccountId(string hashedAccountId) => _encodingService.Decode(hashedAccountId, EncodingType.AccountId);

        public async Task<bool> HasValidApprenticeships(string hashedAccountId)
        {
            var accountEstimation = await _estimationRepository.Get(GetAccountId(hashedAccountId));
            return accountEstimation.HasValidApprenticeships;
        }


        public async Task RefreshCurrentBalance(long accountId)
        {
            var currentBalance = await _currentBalanceRepository.Get(accountId);
            if (!await currentBalance.RefreshBalance())
                return;
            await _currentBalanceRepository.Store(currentBalance);
        }

        private IReadOnlyList<AccountFundsItem> GetAccountFunds(
            IEnumerable<AccountEstimationProjectionModel> estimations)
        {
            var accountFunds = estimations.Select(estimation => new AccountFundsItem
            {
                Date = new DateTime(estimation.Year, estimation.Month, 1),
                ActualCost = estimation.ActualCosts.FundsOut,
                EstimatedCost = estimation.AllModelledCosts.FundsOut,
                ExpiredFunds =  estimation.AllModelledCosts.ExpiredFunds,
                Balance = estimation.EstimatedProjectionBalance,
                FormattedBalance = estimation.EstimatedProjectionBalance > 0
                    ? estimation.EstimatedProjectionBalance.FormatCost()
                    : "-"
            });

            return accountFunds.ToList();
        }

        public async Task<AddEditApprenticeshipsViewModel> EditApprenticeshipModel(string hashedAccountId,
            string apprenticeshipsId, string estimationName)
        {
            var accountId = _encodingService.Decode(hashedAccountId, EncodingType.AccountId);
            var estimations = await _estimationRepository.Get(accountId);

            var model = estimations.FindVirtualApprenticeship(apprenticeshipsId);
            var course = await _apprenticeshipCourseService.GetApprenticeshipCourse(model.CourseId);

            return new AddEditApprenticeshipsViewModel
            {
                Course = course,
                ApprenticeshipsId = apprenticeshipsId,
                EstimationName = estimationName,

                NumberOfApprentices = model.ApprenticesCount,
                TotalInstallments = model.TotalInstallments,
                TotalCostAsString = model.TotalCost.FormatValue(),
                StartDateMonth = model.StartDate.Month,
                StartDateYear = model.StartDate.Year,
                HashedAccountId = hashedAccountId
            };
        }
    }
}