﻿using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Application.Subscriptions.Queries.GetSubscriber;
using eru.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.PlatformClients.FacebookMessenger.Selector;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.KnownUser.UnsupportedCommand
{
    public class UnsupportedCommandMessageHandler : IUnsupportedCommandMessageHandler
    {
        private readonly ISendApiClient _apiClient;
        private readonly ITranslator<FacebookMessengerPlatformClient> _translator;
        private readonly IMediator _mediator;
        private readonly ISelector _selector;
        private readonly ILogger<UnsupportedCommandMessageHandler> _logger;
        
        public UnsupportedCommandMessageHandler(ISendApiClient apiClient, ITranslator<FacebookMessengerPlatformClient> translator, IMediator mediator, ISelector selector, ILogger<UnsupportedCommandMessageHandler> logger)
        {
            _apiClient = apiClient;
            _translator = translator;
            _mediator = mediator;
            _selector = selector;
            _logger = logger;
        }
        
        public async Task Handle(string uid)
        {
            _logger.LogTrace($"eru.PlatformClient.FacebookMessenger: UnsupportedCommandMessageHandler.UnsupportedCommand got a request (uid: {uid})");
            var user = await _mediator.Send(new GetSubscriberQuery(uid, FacebookMessengerPlatformClient.PId));
            
            var response = new SendRequest(uid, new Message(await _translator.TranslateString("unsupported-command", user.PreferredLanguage), await _selector.GetCancelSelector(user.PreferredLanguage)));
            await _apiClient.Send(response);
            _logger.LogInformation($"eru.PlatformClients.FacebookMessenger: UnsupportedCommandMessageHandler.UnsupportedCommand has processed a request from user (uid: {uid})");

        }
    }
}