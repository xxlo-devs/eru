using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherYear;
using eru.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Entities;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.Enums;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherLanguage
{
    public class GatherLanguageMessageHandler : RegistrationMessageHandler<GatherLanguageMessageHandler>
    {
        private readonly ISendApiClient _apiClient;
        private readonly ITranslator<FacebookMessengerPlatformClient> _translator;
        private readonly IConfiguration _configuration;
        private readonly RegistrationMessageHandler<GatherYearMessageHandler> _yearHandler;
        
        public GatherLanguageMessageHandler(IServiceProvider provider, ILogger<GatherLanguageMessageHandler> logger, ITranslator<FacebookMessengerPlatformClient> translator) : base(translator)
        {
            _apiClient = provider.GetService<ISendApiClient>();
            _configuration = provider.GetService<IConfiguration>();
            _translator = translator;
            _yearHandler = provider.GetService<RegistrationMessageHandler<GatherYearMessageHandler>>();
        }
        protected override async Task<IncompleteUser> GatherBase(IncompleteUser user, string data)
        {
            user.PreferredLanguage = data;
            await _yearHandler.ShowInstruction(user, 0);

            return user;
        }

        protected override async Task ShowInstructionBase(IncompleteUser user, int page)
        {
            var response = new SendRequest(user.Id, new Message(await _translator.TranslateString("greeting", _configuration["CultureSettings:DefaultCulture"]), await GetLangSelector(page)));
            await _apiClient.Send(response);
        }

        protected override async Task ShowUnsupportedCommandBase(IncompleteUser user)
        {
            var response = new SendRequest(user.Id, 
                new Message(await _translator.TranslateString("unsupported-command", _configuration["CultureSettings:DefaultCulture"]), await GetLangSelector(user.LastPage)));
            await _apiClient.Send(response);
        }

        private async Task<IEnumerable<QuickReply>> GetLangSelector(int page)
        {
            var displayCulture = _configuration["CultureSettings:DefaultCulture"];
            var supportedCultures = _configuration.GetSection("CultureSettings:AvailableCultures").AsEnumerable().Select(x => x.Value).Skip(1);
            var cultures = supportedCultures.ToDictionary(x => new CultureInfo(x).DisplayName, x => new Payload(PayloadType.Lang, x).ToJson());

            return await GetSelector(cultures, page, PayloadType.Lang, displayCulture);
        }
    }
}