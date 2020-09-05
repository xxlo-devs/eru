using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Classes.Queries.GetClasses;
using eru.Application.Common.Interfaces;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.Enums;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Selector;
using eru.Infrastructure.PlatformClients.FacebookMessenger.SendAPIClient;
using MediatR;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherYear
{
    public class GatherYearMessageHandler : IGatherYearMessageHandler
    {        
        private readonly IRegistrationDbContext _dbContext;
        private readonly ISendApiClient _apiClient;
        private readonly IMediator _mediator;
        private readonly ISelector _selector;
        private readonly ITranslator<FacebookMessengerPlatformClient> _translator;
        
        public GatherYearMessageHandler(IRegistrationDbContext dbContext, ISendApiClient apiClient, IMediator mediator, ISelector selector, ITranslator<FacebookMessengerPlatformClient> translator)
        {
            _dbContext = dbContext;
            _apiClient = apiClient;
            _mediator = mediator;
            _selector = selector;
            _translator = translator;
        }
        public async Task Handle(string uid, Payload payload)
        {
            if (payload.Type == Type.Year)
            {
                if (payload.Page != null)
                {
                    await ShowPage(uid, payload.Page.Value);
                    return;
                }

                if (payload.Id != null)
                {
                    await Gather(uid, int.Parse(payload.Id));
                    return; 
                }
            }

            await UnsupportedCommand(uid);
        }

        private async Task ShowPage(string uid, int page)
        {
            var user = await _dbContext.IncompleteUsers.FindAsync(uid);
            user.LastPage = page;
            
            var response = new SendRequest(uid, new Message(await _translator.TranslateString("year-selection", user.PreferredLanguage), await _selector.GetYearSelector(user.LastPage, user.PreferredLanguage)));
            await _apiClient.Send(response);
        }

        private async Task UnsupportedCommand(string uid)
        {
            var user = await _dbContext.IncompleteUsers.FindAsync(uid);

            var response = new SendRequest(uid, new Message(await _translator.TranslateString("unsupported-command", user.PreferredLanguage), await _selector.GetYearSelector(user.LastPage, user.PreferredLanguage)));
            await _apiClient.Send(response);        
        }

        private async Task Gather(string uid, int year)
        {
            var user = await _dbContext.IncompleteUsers.FindAsync(uid);
            user.Year = year;
            user.Stage = Stage.GatheredYear;
            user.LastPage = 0;
            _dbContext.IncompleteUsers.Update(user);
            await _dbContext.SaveChangesAsync(CancellationToken.None);

            var response = new SendRequest(uid, new Message(await _translator.TranslateString("class-selection", user.PreferredLanguage), await _selector.GetClassSelector(0, user.Year, user.PreferredLanguage)));
            await _apiClient.Send(response);
        }
    }
}