using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Classes.Queries.GetClasses;
using eru.Application.Common.Interfaces;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherClass;
using eru.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Entities;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Enums;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using Hangfire.Logging;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherYear
{
    public class GatherYearMessageHandler : RegistrationMessageHandler<GatherYearMessageHandler>
    {
        private readonly ISendApiClient _apiClient;
        private readonly ITranslator<FacebookMessengerPlatformClient> _translator;
        private readonly IMediator _mediator;
        private readonly RegistrationMessageHandler<GatherClassMessageHandler> _classHandler;
        
        public GatherYearMessageHandler(IServiceProvider provider, ILogger<GatherYearMessageHandler> logger, ITranslator<FacebookMessengerPlatformClient> translator) : base(translator)
        {
            _apiClient = provider.GetService<ISendApiClient>();
            _translator = translator;
            _classHandler = provider.GetService<RegistrationMessageHandler<GatherClassMessageHandler>>();
            _mediator = provider.GetService<IMediator>();
        }
        protected override async Task GatherBase(IncompleteUser user, string data)
        {
            user.Year = int.Parse(data);
            await _classHandler.ShowInstruction(user, 0);
        }

        protected override async Task ShowInstructionBase(IncompleteUser user, int page)
        {
            var response = new SendRequest(user.Id, new Message(await _translator.TranslateString("year-selection", user.PreferredLanguage), await GetYearSelector(user.LastPage, user.PreferredLanguage)));
            await _apiClient.Send(response);
        }

        protected override async Task ShowUnsupportedCommandBase(IncompleteUser user)
        {
            var response = new SendRequest(user.Id, new Message(await _translator.TranslateString("unsupported-command", user.PreferredLanguage), await GetYearSelector(user.LastPage, user.PreferredLanguage)));
            await _apiClient.Send(response);
        }

        private async Task<IEnumerable<QuickReply>> GetYearSelector(int page, string lang)
        {
            var classes = await _mediator.Send(new GetClassesQuery());
            var years = new SortedSet<int>(classes.Select(x => x.Year)).ToDictionary(x => x.ToString(), x => new Payload(PayloadType.Year, x.ToString()).ToJson());

            return await GetSelector(years, page, PayloadType.Year, lang);
        }
    }
}