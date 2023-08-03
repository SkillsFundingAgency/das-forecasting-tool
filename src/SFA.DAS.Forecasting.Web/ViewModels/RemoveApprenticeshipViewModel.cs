
using System.ComponentModel.DataAnnotations;

namespace SFA.DAS.Forecasting.Web.ViewModels
{
    public class RemoveApprenticeshipViewModel
    {
        public string ApprenticeshipId { get; set; }
        public string EstimationName { get; set; }
        public string HashedAccountId { get; set; }
        public string CourseTitle { get; set; }
        public int Level { get; set; }
        public int NumberOfApprentices { get; set; }
        [Required(ErrorMessage = "Please choose an option")]
        public bool? ConfirmedDeletion { get; set; }
    }
}