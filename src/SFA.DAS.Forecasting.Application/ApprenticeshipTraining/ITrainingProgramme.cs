namespace SFA.DAS.Forecasting.Application.ApprenticeshipTraining
{
    public interface ITrainingProgramme
    {
        string Id { get; set; }
        string Title { get; set; }
        int Level { get; set; }
        int Duration { get; set; }
        int MaxFunding { get; set; }
    }
}
