using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eru.Application.Classes.Queries.GetClasses;
using eru.Application.Common.Interfaces;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps.GatherClass;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Entities;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps.GatherYear
{
    public class GatherYearMessageHandler : RegistrationStepsMessageHandler<GatherYearMessageHandler>, IGatherYearMessageHandler
    {
        private readonly IMediator _mediator;
        private readonly ISendApiClient _apiClient;
        private readonly ITranslator<FacebookMessengerPlatformClient> _translator;
        private readonly IGatherClassMessageHandler _classHandler;
        
        public GatherYearMessageHandler(IMediator mediator, ISendApiClient apiClient, ITranslator<FacebookMessengerPlatformClient> translator, IGatherClassMessageHandler classHandler, IRegistrationDbContext dbContext, ILogger<GatherYearMessageHandler> logger) : base(dbContext, translator, logger)
        {
            _mediator = mediator;
            _apiClient = apiClient;
            _translator = translator;
            _classHandler = classHandler;
        }
        protected override async Task<IncompleteUser> UpdateUserBase(IncompleteUser user, string data)
        {
            var usr = user; 
            
            usr.Year = int.Parse(data);
            await _classHandler.ShowInstruction(usr);
            
            return usr;
        }

        protected override async Task ShowInstructionBase(IncompleteUser user, int page)
        {
            await _apiClient.Send(new SendRequest(user.Id,
                new Message(await _translator.TranslateString("year-selection", user.PreferredLanguage),
                    await GetYearSelector(page, user.PreferredLanguage))));
        }

        protected override async Task UnsupportedCommandBase(IncompleteUser user)
        {
            await _apiClient.Send(new SendRequest(user.Id,
                new Message(await _translator.TranslateString("unsupported-command", user.PreferredLanguage),
                    await GetYearSelector(user.LastPage, user.PreferredLanguage))));
        }

        private async Task<IEnumerable<QuickReply>> GetYearSelector(int page, string lang)
        {
            var classes = await _mediator.Send(new GetClassesQuery());
            var years = new SortedSet<int>(classes.Select(x => x.Year)).ToDictionary(i => i.ToString(),
                i => new Payload(PayloadType.Year, i.ToString()).ToJson());

            return await GetSelector(years, page, PayloadType.Year, lang);
        }
    }
}