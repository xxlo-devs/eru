using System.Collections.Generic;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Application.Subscriptions.Queries.GetSubscriber;
using eru.Domain.Entity;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi.Static;
using eru.Infrastructure.PlatformClients.FacebookMessenger.SendAPIClient;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger
{
    public class FacebookMessengerPlatformClient : IPlatformClient
    {
        public string PlatformId { get; } = "FacebookMessenger";
        private readonly ISendApiClient _apiClient;
        private readonly IMediator _mediator;

        public FacebookMessengerPlatformClient(ISendApiClient apiClient, IMediator mediator)
        {
            _apiClient = apiClient;
            _mediator = mediator;
        }
        
        public async Task SendMessage(string id, string content)
        {
            var message = new SendRequest(id, new Message(content, new[]
            {
                new QuickReply("Cancel", ReplyPayloads.CancelPayload), 
            }), MessageTags.AccountUpdate);

            await _apiClient.Send(message);
        }

        public async Task SendMessage(string id, IEnumerable<Substitution> substitutions)
        {
            var user = await _mediator.Send(new GetSubscriberQuery(id, this.PlatformId));
            
            var req = new SendRequest(id, new Message("Here are substitutions for the next day!"), MessageTags.ConfirmedEventUpdate);
            await _apiClient.Send(req);

            foreach (var x in substitutions)
            {
                var substitution =
                    $"Teacher {x.Teacher} on {x.Lesson} lesson (course: {x.Subject}) will be substituted by teacher {x.Substituting} in {x.Room} room. School notes: {x.Note}.";
                req = new SendRequest(id, new Message(substitution), MessageTags.ConfirmedEventUpdate);
                await _apiClient.Send(req);
            }

            req = new SendRequest(id, new Message("If you want to stop getting these notifications, just click Cancel. ", new[] {new QuickReply("Cancel", ReplyPayloads.CancelPayload)}), MessageTags.ConfirmedEventUpdate);
            await _apiClient.Send(req);
        }
    }
}