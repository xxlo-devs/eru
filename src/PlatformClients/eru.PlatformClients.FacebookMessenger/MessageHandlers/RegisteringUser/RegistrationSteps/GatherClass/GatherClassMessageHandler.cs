using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eru.Application.Classes.Queries.GetClasses;
using eru.Application.Common.Interfaces;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationEnd.ConfirmSubscription;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Entities;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests;
using MediatR;
using Microsoft.Extensions.Logging;
using Message = eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests.Message;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps.GatherClass
{
    public class GatherClassMessageHandler : RegistrationStepsMessageHandler<GatherClassMessageHandler>, IGatherClassMessageHandler
    {
        private readonly IMediator _mediator;
        private readonly ISendApiClient _apiClient;
        private readonly ITranslator<FacebookMessengerPlatformClient> _translator;
        private readonly IConfirmSubscriptionMessageHandler _confirmHandler;
        
        public GatherClassMessageHandler(IMediator mediator, ISendApiClient apiClient, ITranslator<FacebookMessengerPlatformClient> translator, IConfirmSubscriptionMessageHandler confirmHandler, IRegistrationDbContext dbContext, ILogger<GatherClassMessageHandler> logger) : base(dbContext, translator, logger)
        {
            _mediator = mediator;
            _apiClient = apiClient;
            _translator = translator;
            _confirmHandler = confirmHandler;
        }

        protected override async Task<IncompleteUser> UpdateUserBase(IncompleteUser user, string data)
        {
            var usr = user;
            
            usr.ClassId = data;
            await _confirmHandler.ShowInstruction(usr);

            return usr;
        }

        protected override async Task ShowInstructionBase(IncompleteUser user, int page)
        {
            await _apiClient.Send(new SendRequest(user.Id,
                new Message(await _translator.TranslateString("class-selection", user.PreferredLanguage),
                    await GetClassSelector(user.Year, page, user.PreferredLanguage))));
        }

        protected override async Task UnsupportedCommandBase(IncompleteUser user)
        {
            await _apiClient.Send(new SendRequest(user.Id,
                new Message(await _translator.TranslateString("unsupported-command", user.PreferredLanguage),
                    await GetClassSelector(user.Year, user.LastPage, user.PreferredLanguage))));
        }

        private async Task<IEnumerable<QuickReply>> GetClassSelector(int year, int page, string lang)
        {
            var classesFromDb = await _mediator.Send(new GetClassesQuery());
            var classes = classesFromDb.Where(x => x.Year == year).OrderBy(x => x.Section)
                .ToDictionary(x => x.ToString(), x => new Payload(PayloadType.Class, x.Id).ToJson());

            return await GetSelector(classes, page, PayloadType.Class, lang);
        }
    }
}