using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Classes.Queries.GetClasses;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherLanguage;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.Enums;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Selector;
using eru.Infrastructure.PlatformClients.FacebookMessenger.SendAPIClient;
using FluentAssertions;
using MediatR;
using Moq;
using Xunit;

namespace eru.Infrastructure.Tests.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUserTests
{
    public class GatherLanguageHandlerTests
    {
        // [Fact]
        // public async void ShoudGatherLanguageCorrectly()
        // {
        //     var context = new FakeRegistrationDb();
        //     var apiClient = new Mock<ISendApiClient>();
        //     var mediator = new Mock<IMediator>();
        //     mediator.Setup(x => x.Send(It.IsAny<GetClassesQuery>(), It.IsAny<CancellationToken>())).Returns(
        //         (GetClassesQuery query, CancellationToken cancellationToken) =>
        //         {
        //             return Task.FromResult(new[]
        //             {
        //                 new ClassDto {Id = "sample-class-1", Year = 1, Section = "a"},
        //                 new ClassDto {Id = "sample-class-2", Year = 1, Section = "b"},
        //                 new ClassDto {Id = "sample-class-3", Year = 2, Section = "c"},
        //             }.AsEnumerable());
        //         });
        //     var selector = new Infrastructure.PlatformClients.FacebookMessenger.Selector.Selector();
        //     
        //     var handler = new GatherLanguageMessageHandler(context, apiClient.Object, mediator.Object, selector);
        //     await handler.Handle("sample-registering-user", "lang:en");
        //
        //     context.IncompleteUsers.Should().ContainSingle(x =>
        //         x.Id == "sample-registering-user" && x.Platform == "FacebookMessenger" && x.PreferredLanguage == "en" &&
        //         x.Stage == Stage.GatheredLanguage);
        //     apiClient.Verify(x => x.Send(It.IsAny<SendRequest>()), Times.Once);
        // }
    }
}