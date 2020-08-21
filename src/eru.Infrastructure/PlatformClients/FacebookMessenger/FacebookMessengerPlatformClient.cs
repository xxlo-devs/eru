using System;
using eru.Application.Common.Interfaces;
using eru.Domain.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendAPI;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger
{
    public class FacebookMessengerPlatformClient : IPlatformClient
    {
        public string PlatformId { get; } = "FacebookMessenger";
        public static string StaticPlatformId { get; } = "FacebookMessenger";

        private readonly IMediator _mediator;
        private readonly IStringLocalizer<FacebookMessengerPlatformClient> _localizer;
        private readonly IConfiguration _configuration;

        public FacebookMessengerPlatformClient(IMediator mediator, IStringLocalizer<FacebookMessengerPlatformClient> localizer, IConfiguration configuration)
        {
            _mediator = mediator;
            _localizer = localizer;
            _configuration = configuration;
        }

        public async Task SendMessage(string id, string content)
        {
            throw new NotImplementedException();
        }

        public async Task SendMessage(string id, IEnumerable<Substitution> substitutions)
        {
            throw new NotImplementedException();
        }

        private async Task Send(SendRequest req)
        {
            throw new NotImplementedException();
        }
    }
}