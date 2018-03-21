using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.EmployerUsers;

namespace SFA.DAS.Forecasting.Web.Authentication
{
    public interface IMembershipService
    {
        Task<MembershipContext> GetMembershipContext();
        Task<bool> ValidateMembership();
    }
}