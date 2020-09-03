using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Classes.Queries.GetClasses;
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
        
        public GatherYearMessageHandler(IRegistrationDbContext dbContext, ISendApiClient apiClient, IMediator mediator, ISelector selector)
        {
            _dbContext = dbContext;
            _apiClient = apiClient;
            _mediator = mediator;
            _selector = selector;
        }
        public async Task Handle(string uid, string payload)
        {
            if (payload == ReplyPayloads.PreviousPage)
            {
                await ToPreviousPage(uid);
                return;
            }

            if (payload == ReplyPayloads.NextPage)
            {
                await ToNextPage(uid);
                return;
            }

            if (payload.StartsWith(ReplyPayloads.YearPrefix))
            {
                await Gather(uid, int.Parse(payload.Substring(ReplyPayloads.YearPrefix.Length)));
                return;
            }

            await UnsupportedCommand(uid);
        }
        
        private async Task ToPreviousPage(string uid)
        {
            throw new NotImplementedException();
        }

        private async Task ToNextPage(string uid)
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
            
            var response = new SendRequest(uid, new Message("Great! Now you need to select your class, by clicking on a button below. If you don't see your class, use the \"arrow\" buttons to scroll the list.", _selector.GetSelector(dict, user.ListOffset)));
            await _apiClient.Send(response);
        }
    }
}