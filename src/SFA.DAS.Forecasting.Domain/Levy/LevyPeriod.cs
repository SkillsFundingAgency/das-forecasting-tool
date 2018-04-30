using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SFA.DAS.Forecasting.Domain.Levy.Validation;
using SFA.DAS.Forecasting.Domain.Shared;
using SFA.DAS.Forecasting.Models.Levy;

namespace SFA.DAS.Forecasting.Domain.Levy
{
    public class LevyPeriod
    {
        private readonly List<LevyDeclarationModel> _levyDeclarations;
        public ReadOnlyCollection<LevyDeclarationModel> LevyDeclarations => _levyDeclarations.AsReadOnly();
        
        public LevyPeriod(List<LevyDeclarationModel> levyDeclarations)
        {
            _levyDeclarations = levyDeclarations ?? throw new ArgumentNullException(nameof(levyDeclarations));
        }

        public decimal GetPeriodAmount()
        {
            return _levyDeclarations.Sum(levyDeclaration => levyDeclaration.LevyAmountDeclared);
        }

        public DateTime? GetLastTimeReceivedLevy()
        {
            return _levyDeclarations.OrderByDescending(levy => levy.DateReceived)
                .FirstOrDefault()
                ?.DateReceived;
        }
    }
}