﻿using eru.Application.Common.Interfaces;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationEnd.ConfirmSubscription;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps.GatherClass;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps.GatherLanguage;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps.GatherYear;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Enums;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace eru.PlatformClients.FacebookMessenger.Tests.MessageHandlers.RegisteringUser.RegistrationSteps
{
    public class GatherLanguageMessageHandlerTests
    {
        [Fact]
        public async void ShouldUpdateUserCorrectly()
        {
            var context = new FakeRegistrationDb();
            var config = new ConfigurationBuilder().Build();
            var client = new Mock<ISendApiClient>();
            var translator = new Mock<ITranslator<FacebookMessengerPlatformClient>>();
            var yearHandler = new Mock<IGatherYearMessageHandler>();

            var handler = new GatherLanguageMessageHandler(config, client.Object, translator.Object, yearHandler.Object, context, new Mock<ILogger<GatherLanguageMessageHandler>>().Object);
            await handler.Handle(await context.IncompleteUsers.FindAsync("sample-registering-user"), new Payload(PayloadType.Lang, "pl"));

            context.IncompleteUsers.Should().ContainSingle(x =>
                x.Id == "sample-registering-user" && x.PreferredLanguage == "pl" && x.LastPage == 0 &&
                x.Stage == Stage.GatheredLanguage);
        }

        [Fact]
        public async void ShouldShowInstructionCorrectly()
        {
            
        }

        [Fact]
        public async void ShouldHandleUnsupportedCommandCorrectly()
        {
            
        }
    }
}