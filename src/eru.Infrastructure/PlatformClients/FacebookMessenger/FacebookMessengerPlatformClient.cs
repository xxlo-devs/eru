using System;
using eru.Application.Common.Interfaces;
using eru.Domain.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.Webhook.Messaging;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDbContext;
using MediatR;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger
{
    public class FacebookMessengerPlatformClient : IPlatformClient
    {
        public string PlatformId { get; } = "FacebookMessenger";
        private readonly IMediator _mediator;
        private readonly FacebookMessengerRegistrationDbContext _localDbContext;

        public FacebookMessengerPlatformClient(IMediator mediator, FacebookMessengerRegistrationDbContext localDbContext)
        {
            _mediator = mediator;
            _localDbContext = localDbContext;
        }

        public async Task SendMessage(string id, string content)
        {
            throw new System.NotImplementedException();
        }

        public async Task SendMessage(string id, IEnumerable<Substitution> substitutions)
        {
            throw new System.NotImplementedException();
        }

        public async Task HandleIncomingMessage(Messaging message)
        {
            throw new NotFiniteNumberException();
        }
    }
}