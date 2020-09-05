using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using eru.Application.Classes.Queries.GetClasses;
using eru.Application.Common.Interfaces;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.ReplyPayload;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.Selector
{
    public class Selector : ISelector
    {
        private readonly ITranslator<FacebookMessengerPlatformClient> _translator;
        private readonly IConfiguration _configuration;
        private readonly IMediator _mediator;
        
        public Selector(ITranslator<FacebookMessengerPlatformClient> translator, IConfiguration configuration, IMediator mediator)
        {
            _translator = translator;
            _configuration = configuration;
            _mediator = mediator;
        }
        
        public async Task<IEnumerable<QuickReply>> GetLangSelector(int page)
        {
            var displayCulture = _configuration["CultureSettings:DefaultCulture"];
            var supportedCultures = _configuration.GetSection("CultureSettings:AvailableCultures").AsEnumerable().Select(x => x.Value).Skip(1);
            var cultures = supportedCultures.ToDictionary(x => new CultureInfo(x).DisplayName, x => new Payload(PayloadType.Lang, x).ToJson());

            return await GetSelector(cultures, page, PayloadType.Lang, displayCulture);
        }

        public async Task<IEnumerable<QuickReply>> GetYearSelector(int page, string preferredLanguage)
        {
            var classes = await _mediator.Send(new GetClassesQuery());
            var years = new SortedSet<int>(classes.Select(x => x.Year)).ToDictionary(x => x.ToString(), x => new Payload(PayloadType.Year, x.ToString()).ToJson());

            return await GetSelector(years, page, PayloadType.Year, preferredLanguage);
        }

        public async Task<IEnumerable<QuickReply>> GetClassSelector(int page, int year, string preferredLanguage)
        {
            var classesFromDb = await _mediator.Send(new GetClassesQuery());
            var classes = classesFromDb.Where(x => x.Year == year).OrderBy(x => x.Section).ToDictionary(x => x.ToString(), x => new Payload(PayloadType.Class, x.Id).ToJson());

            return await GetSelector(classes, page, PayloadType.Class, preferredLanguage);
        }

        public async Task<IEnumerable<QuickReply>> GetConfirmationSelector(string preferredLanguage)
        {
            return new[]
            {
                new QuickReply(await _translator.TranslateString("subscribe-button", preferredLanguage), new Payload(PayloadType.Subscribe).ToJson()), 
                new QuickReply(await _translator.TranslateString("cancel-button", preferredLanguage), new Payload(PayloadType.Cancel).ToJson())
            };
        }

        public async Task<IEnumerable<QuickReply>> GetCancelSelector(string preferredLanguage)
        {
            return new[]
            {
                new QuickReply(await _translator.TranslateString("cancel-button", preferredLanguage), new Payload(PayloadType.Cancel).ToJson())
            };
        }
        
        private async Task<IEnumerable<QuickReply>> GetSelector(Dictionary<string, string> items, int page, PayloadType payloadType, string displayCulture)
        {
            var offset = page * 10;

            var scope = items.Skip(offset).Take(10);

            var replies = scope.Select(x => new QuickReply(x.Key, x.Value)).ToList();

            if (page > 0)
            {
                replies.Add(new QuickReply(await _translator.TranslateString("previous-page", displayCulture), new Payload(payloadType, page - 1).ToJson()));
            }

            if (items.Count() - offset - 10 > 0)
            {
                replies.Add(new QuickReply(await _translator.TranslateString("next-page", displayCulture), new Payload(payloadType, page + 1).ToJson()));
            }

            replies.Add(new QuickReply(await _translator.TranslateString("cancel-button", displayCulture), new Payload(PayloadType.Cancel).ToJson()));

            return replies;
        }
    }
}