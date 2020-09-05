using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Application.Subscriptions.Queries.GetSubscriber;
using eru.Domain.Entity;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi.Static;
using eru.Infrastructure.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Selector;
using eru.Infrastructure.PlatformClients.FacebookMessenger.SendAPIClient;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger
{
    public class FacebookMessengerPlatformClient : IPlatformClient
    {
        public static string PId { get; } = "FacebookMessenger";
        public string PlatformId { get; } = PId;

        private readonly ISendApiClient _apiClient;
        private readonly IMediator _mediator;
        private readonly ISelector _selector;
        private readonly ITranslator<FacebookMessengerPlatformClient> _translator;

        public FacebookMessengerPlatformClient(ISendApiClient apiClient, IMediator mediator, ISelector selector, ITranslator<FacebookMessengerPlatformClient> translator)
        {
            _apiClient = apiClient;
            _mediator = mediator;
            _selector = selector;
            _translator = translator;
        }
        
        public async Task SendMessage(string id, string content)
        {
            var user = await _mediator.Send(new GetSubscriberQuery(id, PlatformId));
            var message = new SendRequest(id, new Message(content, await _selector.GetCancelSelector(user.PreferredLanguage)), MessageTags.AccountUpdate);

            await _apiClient.Send(message);
        }

        public async Task SendMessage(string id, IEnumerable<Substitution> substitutions)
        {
            var user = await _mediator.Send(new GetSubscriberQuery(id, this.PlatformId));
            
            var req = new SendRequest(id, new Message(await _translator.TranslateString("new-substitutions", user.PreferredLanguage)), MessageTags.ConfirmedEventUpdate);
            await _apiClient.Send(req);

            foreach (var x in substitutions)
            {
                var substitution = string.Format(await _translator.TranslateString("substitution", user.PreferredLanguage), x.Teacher, x.Lesson, x.Subject, x.Substituting, x.Room, x.Note);
                
                req = new SendRequest(id, new Message(substitution), MessageTags.ConfirmedEventUpdate);
                await _apiClient.Send(req);
            }

            req = new SendRequest(id, new Message(await _translator.TranslateString("closing-substitutions", user.PreferredLanguage), await _selector.GetCancelSelector(user.PreferredLanguage)), MessageTags.ConfirmedEventUpdate);
            await _apiClient.Send(req);
        }
    }
}