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
            throw new NotImplementedException();
        }

        private async Task UnsupportedCommand(string uid)
        {
            throw new NotImplementedException();
        }

        private async Task Gather(string uid, int year)
        {
            var user = await _dbContext.IncompleteUsers.FindAsync(uid);
            user.Year = year;
            user.Stage = Stage.GatheredYear;
            user.ListOffset = 0;
            _dbContext.IncompleteUsers.Update(user);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
            
            var classesInDb = await _mediator.Send(new GetClassesQuery());
            var dict = classesInDb.Where(x => x.Year == user.Year).OrderBy(x => x.Section).ToDictionary(x => x.ToString(), x => x.Id);
            
            var response = new SendRequest(uid, new Message(await _translator.TranslateString("class-selection", user.PreferredLanguage), _selector.GetSelector(dict, 0, Type.Class)));
            await _apiClient.Send(response);
        }
    }
}