﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Data;
using SFA.DAS.Forecasting.Domain.Projections.Services;
using SFA.DAS.Forecasting.Models.Projections;

namespace SFA.DAS.Forecasting.Application.Projections.Services
{
    public class AccountProjectionDataSession : IAccountProjectionDataSession
    {
        private readonly IForecastingDataContext _dataContext;

        public AccountProjectionDataSession(IForecastingDataContext dataContext)
        {
            _dataContext = dataContext ?? throw new ArgumentNullException(nameof(dataContext));
        }

        public async Task<List<AccountProjectionModel>> Get(long employerAccountId)
        {
            return await _dataContext.AccountProjections
                .Where(projection => projection.EmployerAccountId == employerAccountId)
                .ToListAsync();
        }

        public void Store(IEnumerable<AccountProjectionModel> accountProjections)
        {
            foreach (var accountProjectionModel in accountProjections)
            {
                _dataContext.AccountProjections.Add(accountProjectionModel);
            }
        }

        public async Task DeleteAll(long employerAccountId)
        {
            _dataContext.AccountProjectionCommitments.RemoveRange(
                _dataContext.AccountProjectionCommitments
                    .Where(apc => apc.AccountProjection.EmployerAccountId == employerAccountId).ToList());
            var projections = await Get(employerAccountId);
            _dataContext.AccountProjections.RemoveRange(projections);
        }

        public async Task SaveChanges()
        {
            await _dataContext.SaveChangesAsync();
        }
    }
}
