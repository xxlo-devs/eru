﻿using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Application.Subscriptions.Queries.GetSubscriber;
using eru.PlatformClients.FacebookMessenger.Middleware.Webhook.Messages;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests;
using MediatR;
using Microsoft.Extensions.Logging;
using Message = eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests.Message;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser.UnsupportedCommand
{
    public class UnsupportedCommandMessageHandler : MessageHandler<UnsupportedCommandMessageHandler>,
        IUnsupportedCommandMessageHandler
    {
        private readonly IMediator _mediator;
        private readonly ISendApiClient _apiClient;
        private readonly ITranslator<FacebookMessengerPlatformClient> _translator;

        public UnsupportedCommandMessageHandler(IMediator mediator, ISendApiClient apiClient,
            ITranslator<FacebookMessengerPlatformClient> translator,
            ILogger<UnsupportedCommandMessageHandler> logger) : base(logger)
        {
            _mediator = mediator;
            _apiClient = apiClient;
            _translator = translator;
        }

        protected override async Task Base(Messaging message)
        {
            var uid = message.Sender.Id;
            var user = await _mediator.Send(new GetSubscriberQuery(uid, FacebookMessengerPlatformClient.PId));
            
            await _apiClient.Send(new SendRequest(uid,
                new Message(await _translator.TranslateString("unsupported-command", user.PreferredLanguage),
                    new[]
                    {
                        new QuickReply(await _translator.TranslateString("cancel-button", user.PreferredLanguage),
                            new Payload(PayloadType.Cancel).ToJson())
                    })));
        }
    }
}