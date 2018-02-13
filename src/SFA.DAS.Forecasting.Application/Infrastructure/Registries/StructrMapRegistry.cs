using MediatR;
using StructureMap;

namespace SFA.DAS.Forecasting.Application.Infrastructure.Registries
{
    public class StructrMapRegistry: Registry
    {
        public StructrMapRegistry()
        {
            Scan(scanner =>
            {
                scanner.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<>)); // Handlers with no response
                scanner.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>)); // Handlers with a response
                scanner.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
            });
            For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => ctx.GetInstance);
            For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => ctx.GetAllInstances);
            For<IMediator>().Use<Mediator>();

        }
    }
}