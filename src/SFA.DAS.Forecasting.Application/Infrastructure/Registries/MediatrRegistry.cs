// using MediatR;
// using StructureMap;
//
// namespace SFA.DAS.Forecasting.Application.Infrastructure.Registries
// {
//     public class MediatrRegistry: Registry
//     {
//         public MediatrRegistry()
//         {
//             Scan(scanner =>
//             {
//                 scanner.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<>)); // Handlers with no response
//                 scanner.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>)); // Handlers with a response
//                 scanner.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
//             });
//             For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => ctx.GetInstance);
//             For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => ctx.GetAllInstances);
//             For<IMediator>().Use<Mediator>();
//
//         }
//     }
// }