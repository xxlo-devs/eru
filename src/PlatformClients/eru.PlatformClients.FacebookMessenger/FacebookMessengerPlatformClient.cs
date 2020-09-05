using System.Collections.Generic;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Application.Subscriptions.Queries.GetSubscriber;
using eru.Domain.Entity;
using eru.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.PlatformClients.FacebookMessenger.Models.SendApi.Static;
using eru.PlatformClients.FacebookMessenger.Selector;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eru.PlatformClients.FacebookMessenger
{
    public class FacebookMessengerPlatformClient : IPlatformClient
    {
        public static string PId { get; } = "FacebookMessenger";
        public string PlatformId { get; } = PId;

        private readonly ISendApiClient _apiClient;
        private readonly IMediator _mediator;
        private readonly ISelector _selector;
        private readonly ITranslator<FacebookMessengerPlatformClient> _translator;
        private readonly ILogger _logger;

        public FacebookMessengerPlatformClient(ISendApiClient apiClient, IMediator mediator, ISelector selector, ITranslator<FacebookMessengerPlatformClient> translator, ILogger logger)
        {
            _apiClient = apiClient;
            _mediator = mediator;
            _selector = selector;
            _translator = translator;
            _logger = logger;
        }
        
        public async Task SendMessage(string id, string content)
        {
            var user = await _mediator.Send(new GetSubscriberQuery(id, PlatformId));
            var message = new SendRequest(id, new Message(content, await _selector.GetCancelSelector(user.PreferredLanguage)), MessageTags.AccountUpdate);

            await _apiClient.Send(message);
            _logger.LogInformation($"eru.PlatformClients.FacebookMessenger: FacebookMessengerPlatformClient.SendMessage sent a generic message (uid: {id}, content: {content})");
        }

        public async Task SendMessage(string id, IEnumerable<Substitution> substitutions)
        {
            var user = await _mediator.Send(new GetSubscriberQuery(id, this.PlatformId));
            
            var req = new SendRequest(id, new Message(await _translator.TranslateString("new-substitutions", user.PreferredLanguage)), MessageTags.ConfirmedEventUpdate);
            await _apiClient.Send(req);

            foreach (var x in substitutions)
            {
                if (x.Cancelled)
                {
                    var format = await _translator.TranslateString("cancellation", user.PreferredLanguage);
                    var substitution = string.Format(format, x.Lesson, x.Subject, x.Teacher, x.Room, x.Note);
                    req = new SendRequest(id, new Message(substitution), MessageTags.ConfirmedEventUpdate);
                    await _apiClient.Send(req);
                }
                else
                {
                    var format = await _translator.TranslateString("substitution", user.PreferredLanguage);
                    var substitution = string.Format(format, x.Teacher, x.Lesson, x.Subject, x.Substituting, x.Room, x.Note);
                    req = new SendRequest(id, new Message(substitution), MessageTags.ConfirmedEventUpdate);
                    await _apiClient.Send(req);
                }
            }

            req = new SendRequest(id, new Message(await _translator.TranslateString("closing-substitutions", user.PreferredLanguage), await _selector.GetCancelSelector(user.PreferredLanguage)), MessageTags.ConfirmedEventUpdate);
            await _apiClient.Send(req);
            
            _logger.LogInformation($"eru.PlatformClients.FacebookMessenger: FacebookMessengerPlatformClient.SendMessage sent substitutions to user (uid: {id}");
        }
    }
}