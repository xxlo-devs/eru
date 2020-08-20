using System;
using eru.Application.Common.Interfaces;
using eru.Domain.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;
using eru.Application.Users.Queries.GetUser;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendAPI;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.Webhook.Messaging;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDbContext;
using MediatR;
using Microsoft.Extensions.Localization;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger
{
    public class FacebookMessengerPlatformClient : IPlatformClient
    {
        public string PlatformId { get; } = "FacebookMessenger";
        private readonly IMediator _mediator;
        private readonly FacebookMessengerRegistrationDbContext _localDbContext;
        private readonly IStringLocalizer<FacebookMessengerPlatformClient> _localizer;

        public FacebookMessengerPlatformClient(IMediator mediator, FacebookMessengerRegistrationDbContext localDbContext, IStringLocalizer<FacebookMessengerPlatformClient> localizer)
        {
            _mediator = mediator;
            _localDbContext = localDbContext;
            _localizer = localizer;
        }

        public async Task SendMessage(string id, string content)
        {
            throw new NotImplementedException();
        }

        public async Task SendMessage(string id, IEnumerable<Substitution> substitutions)
        {
            throw new NotImplementedException();
        }

        public async Task HandleIncomingMessage(Messaging message)
        {
            if (await _mediator.Send(new GetUserQuery {UserId = message.Sender.Id, Platform = PlatformId}) != null)
            {
                await HandleRegistratedUserRequest(message);
                return;
            }

            if (await _localDbContext.IncompleteUsers.FindAsync(message.Sender.Id) != null)
            {
                await HandleUserBeingRegistratedRequest(message);
                return;
            }

            await HandleNewUserRequest(message);
            return;
        }

        private async Task HandleRegistratedUserRequest(Messaging message)
        {
            throw new NotImplementedException();
        }

        private async Task HandleUserBeingRegistratedRequest(Messaging message)
        {
            throw new NotImplementedException();
        }

        private async Task HandleNewUserRequest(Messaging message)
        {
            throw new NotImplementedException();
        }

        private async Task Send(Request req)
        {
            throw new NotImplementedException();
        }
    }
}