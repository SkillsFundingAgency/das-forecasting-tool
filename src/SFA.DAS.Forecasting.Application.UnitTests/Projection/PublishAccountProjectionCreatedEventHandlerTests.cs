using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Common;
using Moq;
using NServiceBus.Testing;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
using SFA.DAS.Forecasting.Application.Projections.Handlers;
using SFA.DAS.Forecasting.Messages.Projections;

namespace SFA.DAS.Forecasting.Application.UnitTests.Projection
{
    [TestFixture]
    public class PublishAccountProjectionCreatedEventHandlerTests
    {

        private Mock<ITelemetry> _telementryMock;
        private Mock<IApplicationConfiguration> _applicationConfigMock;
        private TestableMessageSession _messageSession = new TestableMessageSession();

        private PublishAccountProjectionHandler sut;

        [SetUp]
        public void SetUp()
        {
            _telementryMock = new Mock<ITelemetry>();
            _applicationConfigMock = new Mock<IApplicationConfiguration>();
            sut = new PublishAccountProjectionHandler(_applicationConfigMock.Object, _telementryMock.Object, _messageSession);
        }

        [Test]
        public async Task Handler_Publishes_Projection_Event_Message()
        {

            var message = new AccountProjectionCreatedEvent(12345);
            await sut.Handle(message);

            _messageSession.PublishedMessages.Any().Should().BeTrue("At least one Event should have been published");
            _messageSession.PublishedMessages.FirstOrDefault()?.Should().IsSameOrEqualTo(message);
        }

    }
}