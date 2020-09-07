using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eru.Application.Classes.Queries.GetClasses;
using eru.Application.Common.Interfaces;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationEnd;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Entities;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps
{
    public class GatherClassMessageHandler : RegistrationStepsMessageHandler<GatherClassMessageHandler>
    {
        private readonly IMediator _mediator;
        private readonly ISendApiClient _apiClient;
        private readonly ITranslator<FacebookMessengerPlatformClient> _translator;
        private readonly RegistrationEndMessageHandler<ConfirmSubscriptionMessageHandler> _confirmHandler;
        
        public GatherClassMessageHandler(IMediator mediator, ISendApiClient apiClient, ITranslator<FacebookMessengerPlatformClient> translator, RegistrationEndMessageHandler<ConfirmSubscriptionMessageHandler> confirmHandler, IRegistrationDbContext dbContext, ILogger<GatherClassMessageHandler> logger) : base(dbContext, translator, logger)
        {
            _mediator = mediator;
            _apiClient = apiClient;
            _translator = translator;
            _confirmHandler = confirmHandler;
        }

        protected override async Task<IncompleteUser> UpdateUserBase(IncompleteUser user, string data)
        {
            user.ClassId = data;
            await _confirmHandler.ShowInstruction(user);
            
            return user;
        }

        protected override async Task ShowInstructionBase(IncompleteUser user, int page)
        {
            var response = new SendRequest(user.Id, new Message(await _translator.TranslateString("class-selection", user.PreferredLanguage), await GetClassSelector(user.Year, page, user.PreferredLanguage)));
            await _apiClient.Send(response);
        }

        protected override async Task UnsupportedCommandBase(IncompleteUser user)
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