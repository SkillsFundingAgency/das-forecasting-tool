using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Domain.Events
{
    public interface IEventPublisher
    {
        Task Publish<T>(T domainEvent);
    }
}