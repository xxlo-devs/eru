﻿using System.Collections.Generic;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Application.Subscriptions.Queries.GetSubscriber;
using eru.Domain.Entity;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests;
using eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests.Static;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eru.PlatformClients.FacebookMessenger
{
    public class FacebookMessengerPlatformClient : IPlatformClient
    {
        public static string PId { get; } = "FacebookMessenger";
        public string PlatformId => PId;

        private readonly ISendApiClient _apiClient;
        private readonly IMediator _mediator;
        private readonly ITranslator<FacebookMessengerPlatformClient> _translator;
        private readonly ILogger<FacebookMessengerPlatformClient> _logger;

        public FacebookMessengerPlatformClient(ISendApiClient apiClient, IMediator mediator,
            ITranslator<FacebookMessengerPlatformClient> translator, ILogger<FacebookMessengerPlatformClient> logger)
        {
            _apiClient = apiClient;
            _mediator = mediator;
            _translator = translator;
            _logger = logger;
        }
        
        public async Task SendMessage(string id, string content)
        {
            var user = await _mediator.Send(new GetSubscriberQuery(id, PlatformId));
            
            await _apiClient.Send(new SendRequest(id,
                new Message(content, await GetCancelButton(user.PreferredLanguage)), MessageTags.AccountUpdate));
            
            _logger.LogInformation(
                "Facebook Messenger Platform Client: sent a generic message to user (uid: {user.Id}) with content (content: {content})",
                user, content);
        }

        public async Task SendMessage(string id, IEnumerable<Substitution> substitutions)
        {
            var user = await _mediator.Send(new GetSubscriberQuery(id, PlatformId));
            
            await _apiClient.Send(new SendRequest(id,
                new Message(await _translator.TranslateString("new-substitutions", user.PreferredLanguage)),
                MessageTags.ConfirmedEventUpdate));

            foreach (var x in substitutions)
            {
                var substitution = x.Cancelled
                    ? string.Format(await _translator.TranslateString("cancellation", user.PreferredLanguage), 
                        x.Lesson, x.Subject, x.Teacher, x.Room, x.Note)
                    : string.Format(await _translator.TranslateString("substitution", user.PreferredLanguage), 
                        x.Teacher, x.Lesson, x.Subject, x.Substituting, x.Room, x.Note);

                await _apiClient.Send(new SendRequest(id, new Message(substitution), MessageTags.ConfirmedEventUpdate));
            }
            
            await _apiClient.Send(new SendRequest(id,
                new Message(await _translator.TranslateString("closing-substitutions", user.PreferredLanguage),
                    await GetCancelButton(user.PreferredLanguage)), MessageTags.ConfirmedEventUpdate));
            
            _logger.LogInformation("Facebook Messenger Platform Client: sent substitutions to user {user.Id}", user);
        }

        private async Task<IEnumerable<QuickReply>> GetCancelButton(string lang)
            => new[]
            {
                new QuickReply(await _translator.TranslateString("cancel-button", lang),
                    new Payload(PayloadType.Cancel).ToJson())
            };
    }
}