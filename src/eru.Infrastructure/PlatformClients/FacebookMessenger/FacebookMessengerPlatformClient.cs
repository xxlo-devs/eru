using System;
using eru.Application.Common.Interfaces;
using eru.Domain.Entity;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using eru.Application.Users.Queries.GetUser;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendAPI;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendAPI.Static;
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
            var req = new SendRequest(id, new Message(content, new[] {new QuickReply(_localizer["cancel"], ReplyPayloads.CancelPayload)}), MessageTags.AccountUpdate);
            await Send(req);
        }

        public async Task SendMessage(string id, IEnumerable<Substitution> substitutions)
        {
            var user = await _mediator.Send(new GetUserQuery {UserId = id, Platform = PlatformId});
            CultureInfo.CurrentCulture = new CultureInfo(user.PreferredLanguage);
            CultureInfo.CurrentUICulture = new CultureInfo(user.PreferredLanguage);

            var req = new SendRequest(user.Id, new Message(_localizer["new-substitutions"]), MessageTags.ConfirmedEventUpdate);
            await Send(req);

            foreach (var x in substitutions)
            {
                var substitution = string.Format(_localizer["substitution"], x.Teacher, x.Lesson, x.Subject, x.Substituting, x.Room, x.Note);
                req = new SendRequest(user.Id, new Message(substitution), MessageTags.ConfirmedEventUpdate);
                await Send(req);
            }

            req = new SendRequest(user.Id, new Message(_localizer["closing"], new[] {new QuickReply(_localizer["cancel"], ReplyPayloads.CancelPayload)}), MessageTags.ConfirmedEventUpdate);
            await Send(req);
        }

        private async Task Send(SendRequest req)
        {
            throw new NotImplementedException();
        }
    }
}