using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps.GatherYear;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Entities;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.RegistrationSteps.GatherLanguage
{
    public class GatherLanguageMessageHandler : RegistrationStepsMessageHandler<GatherLanguageMessageHandler>, IGatherLanguageMessageHandler
    {
        private readonly ISendApiClient _apiClient;
        private readonly ITranslator<FacebookMessengerPlatformClient> _translator;
        private readonly IGatherYearMessageHandler _yearHandler;
        private readonly IApplicationCultures _cultures;
        
        public GatherLanguageMessageHandler(ISendApiClient apiClient,
            ITranslator<FacebookMessengerPlatformClient> translator, IGatherYearMessageHandler yearHandler,
            IRegistrationDbContext dbContext, ILogger<GatherLanguageMessageHandler> logger, IApplicationCultures cultures) 
            : base(dbContext, translator, logger)
        {
            _apiClient = apiClient;
            _translator = translator;
            _yearHandler = yearHandler;
            _cultures = cultures;
        }
        protected override async Task<IncompleteUser> UpdateUserBase(IncompleteUser user, string data)
        {
            var usr = user;
            
            usr.PreferredLanguage = data;
            await _yearHandler.ShowInstruction(usr);

            return usr;
        }

        protected override async Task ShowInstructionBase(IncompleteUser user, int page)
        {
            await _apiClient.Send(new SendRequest(user.Id,
                new Message(await _translator.TranslateString("greeting", _cultures.FindCulture(user.PreferredLanguage)),
                    await GetLangSelector(page, user.PreferredLanguage))));
        }

        protected override async Task UnsupportedCommandBase(IncompleteUser user)
        {
            await _apiClient.Send(new SendRequest(user.Id,
                new Message(await _translator.TranslateString("unsupported-command", _cultures.FindCulture(user.PreferredLanguage)),
                    await GetLangSelector(user.LastPage, user.PreferredLanguage))));
        }

        private async Task<IEnumerable<QuickReply>> GetLangSelector(int page, string displayCulture)
        {
            var cultures = _cultures.AvailableCultures.ToDictionary(
                x => x.Name,
                x => new Payload(PayloadType.Lang, x.Culture.Name).ToJson()
                );

            return await GetSelector(cultures, page, PayloadType.Lang, _cultures.FindCulture(displayCulture));
        }
    }
}