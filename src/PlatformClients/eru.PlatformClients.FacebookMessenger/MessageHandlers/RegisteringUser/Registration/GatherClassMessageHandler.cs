using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Classes.Queries.GetClasses;
using eru.Application.Common.Interfaces;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.ConfirmSubscription;
using eru.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Entities;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Enums;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherClass
{
    public class GatherClassMessageHandler : RegistrationMessageHandler<GatherClassMessageHandler>
    {
        private readonly ISendApiClient _apiClient;
        private readonly ITranslator<FacebookMessengerPlatformClient> _translator;
        private readonly IMediator _mediator;
        private readonly RegistrationMessageHandler<ConfirmSubscriptionMessageHandler> _confirmHandler;
        
        public GatherClassMessageHandler(IServiceProvider provider, ILogger<GatherClassMessageHandler> logger, ITranslator<FacebookMessengerPlatformClient> translator) : base(translator)
        {
            _apiClient = provider.GetService<ISendApiClient>();
            _mediator = provider.GetService<IMediator>();
            _translator = translator;
            _confirmHandler = provider.GetService<RegistrationMessageHandler<ConfirmSubscriptionMessageHandler>>();
        }

        protected override async Task GatherBase(IncompleteUser user, string data)
        {
            user.ClassId = data;
            await _confirmHandler.ShowInstruction(user, 0);
        }

        protected override async Task ShowInstructionBase(IncompleteUser user, int page)
        {
            var response = new SendRequest(user.Id, new Message(await _translator.TranslateString("class-selection", user.PreferredLanguage), await GetClassSelector(user.Year, page, user.PreferredLanguage)));
            await _apiClient.Send(response);
        }

        protected override async Task ShowUnsupportedCommandBase(IncompleteUser user)
        {
            var response = new SendRequest(user.Id, new Message(await _translator.TranslateString("unsupported-command", user.PreferredLanguage), await GetClassSelector(user.Year, user.LastPage, user.PreferredLanguage)));
            await _apiClient.Send(response);
        }

        private async Task<IEnumerable<QuickReply>> GetClassSelector(int year, int page, string lang)
        {
            var classesFromDb = await _mediator.Send(new GetClassesQuery());
            var classes = classesFromDb.Where(x => x.Year == year).OrderBy(x => x.Section).ToDictionary(x => x.ToString(), x => new Payload(PayloadType.Class, x.Id).ToJson());

            return await GetSelector(classes, page, PayloadType.Class, lang);
        }
    }
}