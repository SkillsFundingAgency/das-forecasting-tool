﻿using System;

namespace SFA.DAS.Forecasting.Domain.Entities
{
    public class Apprenticeship
    {
        public long EmployerAccountId { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int TrainingName { get; set; }
        public int TrainingLevel { get; set; }
        public string TrainingProviderName { get; set; }
        public DateTime StartDate { get; set; }
        public decimal MonthlyPayment { get; set; }
        public int Instalments { get; set; }
        public decimal CompletionPayment { get; set; }
    }
}