using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Levy.Services;
using SFA.DAS.Forecasting.Domain.Shared;
using SFA.DAS.Forecasting.Models.Levy;

namespace SFA.DAS.Forecasting.Domain.Levy
{
    public interface ILevyDeclarationRepository
    {
        Task<LevyDeclaration> Get(long employerAccountId, string scheme, string payrollYear,
            byte payrollMonth);

        Task<IEnumerable<LevyDeclaration>> GetNetValue(long employerAccountId);
        Task Store(LevyDeclaration model);
    }

    public class LevyDeclarationRepository : ILevyDeclarationRepository
    {
        private readonly IPayrollDateService _payrollDateService;
        private readonly ILevyDataSession _dataSession;

        public LevyDeclarationRepository(IPayrollDateService payrollDateService, ILevyDataSession dataSession)
        {
            _payrollDateService = payrollDateService ?? throw new ArgumentNullException(nameof(payrollDateService));
            _dataSession = dataSession ?? throw new ArgumentNullException(nameof(dataSession));
        }

        public async Task<LevyDeclaration> Get(long employerAccountId, string scheme, string payrollYear,
            byte payrollMonth)
        {
            var model = await _dataSession.Get(employerAccountId, scheme, payrollYear, payrollMonth) ??
                        new LevyDeclarationModel
                        {
                            EmployerAccountId = employerAccountId,
                            Scheme = scheme,
                            PayrollMonth = payrollMonth,
                            PayrollYear = payrollYear
                        };
            return new LevyDeclaration(_payrollDateService, model);
        }

        public async Task<IEnumerable<LevyDeclaration>> GetNetValue(long employerAccountId)
        {
            var model = await _dataSession.GetNetLevyDeclarations(employerAccountId);

            return model.Select(s => new LevyDeclaration(_payrollDateService, s));
            
        }

        public async Task Store(LevyDeclaration levyDeclaration)
        {
            _dataSession.Store(levyDeclaration.Model);
            await _dataSession.SaveChanges();
        }
    }
}