namespace SFA.DAS.Forecasting.Application.ApprenticeshipTraining
{
    internal class TrainingProgramme : ITrainingProgramme
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public int Level { get; set; }
        public int Duration { get; set; }
        public int MaxFunding { get; set; }
    }
}
