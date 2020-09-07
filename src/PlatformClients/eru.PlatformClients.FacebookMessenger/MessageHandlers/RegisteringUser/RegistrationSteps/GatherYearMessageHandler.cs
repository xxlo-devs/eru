using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eru.Application.Classes.Queries.GetClasses;
using eru.Application.Common.Interfaces;
using eru.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Entities;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps
{
    public class GatherYearMessageHandler : RegistrationStepsMessageHandler<GatherYearMessageHandler>
    {
        private readonly IMediator _mediator;
        private readonly ISendApiClient _apiClient;
        private readonly ITranslator<FacebookMessengerPlatformClient> _translator;
        private readonly RegistrationStepsMessageHandler<GatherClassMessageHandler> _classHandler;
        
        public GatherYearMessageHandler(IMediator mediator, ISendApiClient apiClient, ITranslator<FacebookMessengerPlatformClient> translator, RegistrationStepsMessageHandler<GatherClassMessageHandler> classHandler, IRegistrationDbContext dbContext, ILogger<GatherYearMessageHandler> logger) : base(dbContext, translator, logger)
        {
            _mediator = mediator;
            _apiClient = apiClient;
            _translator = translator;
            _classHandler = classHandler;
        }
        protected override async Task<IncompleteUser> UpdateUserBase(IncompleteUser user, string data)
        {
            user.Year = int.Parse(data);
            await _classHandler.ShowInstruction(user, 0);
            
            return user;
        }

        protected override async Task ShowInstructionBase(IncompleteUser user, int page)
        {
            var response = new SendRequest(user.Id, new Message(await _translator.TranslateString("year-selection", user.PreferredLanguage), await GetYearSelector(user.LastPage, user.PreferredLanguage)));
            await _apiClient.Send(response);
        }

        protected override async Task UnsupportedCommandBase(IncompleteUser user)
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