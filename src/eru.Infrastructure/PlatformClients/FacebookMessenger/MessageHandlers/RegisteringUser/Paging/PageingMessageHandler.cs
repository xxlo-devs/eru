using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using eru.Application.Classes.Queries.GetClasses;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.Enums;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Selector;
using eru.Infrastructure.PlatformClients.FacebookMessenger.SendAPIClient;
using MediatR;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.Paging
{
    public class PageingMessageHandler : IPagingMessageHandler
    {
        private readonly IRegistrationDbContext _dbContext;
        private readonly IMediator _mediator;
        private readonly ISelector _selector;
        private readonly ISendApiClient _apiClient;
        private readonly IConfiguration _configuration;

        public PageingMessageHandler(IRegistrationDbContext dbContext, ISelector selector, ISendApiClient apiClient, IMediator mediator, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _selector = selector;
            _apiClient = apiClient;
            _mediator = mediator;
            _configuration = configuration;
        }
        
        public async Task ShowPreviousPage(string uid)
        {
            var user = await _dbContext.IncompleteUsers.FindAsync(uid);
            user.ListOffset -= 10;
            _dbContext.IncompleteUsers.Update(user);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
            
            switch (user.Stage)
            {
                case Stage.Created:
                {
                    var supportedCultures = _configuration.GetSection("CultureSettings:AvailableCultures").AsEnumerable().Select(x => x.Value).Skip(1);
                    var dict = new Dictionary<string, string>();
                    foreach (var x in supportedCultures)
                    {
                        var culture = new CultureInfo(x);
                        dict.Add(culture.DisplayName, $"{ReplyPayloads.LangPrefix}{x}");
                    }

                    var replies = _selector.GetSelector(dict, user.ListOffset);
                    var response = new SendRequest(uid, new Message("Hello! Eru is a substitution information system that enables you to get personalized notifications about all substitutions directly from school. If you want to try it, choose your language by clicking on a correct flag below. If you don't want to use this bot, just click Cancel at any time.", replies));
                    await _apiClient.Send(response);
                    
                    break;
                }
                
                case Stage.GatheredLanguage:
                {
                    var classesInDb = await _mediator.Send(new GetClassesQuery());
                    var years = new SortedSet<int>(classesInDb.Select(x => x.Year)).ToDictionary(x => x.ToString(), x => $"{ReplyPayloads.YearPrefix}{x.ToString()}");

                    var response = new SendRequest(uid, new Message("Now select your class year, in the same way as language.", _selector.GetSelector(years, user.ListOffset)));
                    await _apiClient.Send(response);
                    
                    break;
                }

                case Stage.GatheredYear:
                {
                    var classesInDb = await _mediator.Send(new GetClassesQuery());
                    var classes = classesInDb.Where(x => x.Year == user.Year).OrderBy(x => x.Section).ToDictionary(x => x.ToString(), x => x.Id);

                    var response = new SendRequest(uid, new Message("Great! Now you need to select your class, by clicking on a button below. If you don't see your class, use the \"arrow\" buttons to scroll the list.", _selector.GetSelector(classes, user.ListOffset)));
                    await _apiClient.Send(response);
                    
                    break;
                }
            }
        }

        public async Task ShowNextPage(string uid)
        {
            var user = await _dbContext.IncompleteUsers.FindAsync(uid);
            user.ListOffset += 10;
            _dbContext.IncompleteUsers.Update(user);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
            
            switch (user.Stage)
            {
                case Stage.Created:
                {
                    var supportedCultures = _configuration.GetSection("CultureSettings:AvailableCultures").AsEnumerable().Select(x => x.Value).Skip(1);
                    var dict = new Dictionary<string, string>();
                    foreach (var x in supportedCultures)
                    {
                        var culture = new CultureInfo(x);
                        dict.Add(culture.DisplayName, $"{ReplyPayloads.LangPrefix}{x}");
                    }
                    
                    var response = new SendRequest(uid, new Message("Hello! Eru is a substitution information system that enables you to get personalized notifications about all substitutions directly from school. If you want to try it, choose your language by clicking on a correct flag below. If you don't want to use this bot, just click Cancel at any time.", _selector.GetSelector(dict, user.ListOffset)));
                    await _apiClient.Send(response);
                    
                    break;
                }
                
                case Stage.GatheredLanguage:
                {
                    var classesInDb = await _mediator.Send(new GetClassesQuery());
                    var years = new SortedSet<int>(classesInDb.Select(x => x.Year)).ToDictionary(x => x.ToString(), x => $"{ReplyPayloads.YearPrefix}{x.ToString()}");
                    
                    var response = new SendRequest(uid, new Message("Now select your class year, in the same way as language.", _selector.GetSelector(years, user.ListOffset)));
                    await _apiClient.Send(response);
                    
                    break;
                }

                case Stage.GatheredYear:
                {
                    var classesInDb = await _mediator.Send(new GetClassesQuery());
                    var classes = classesInDb.Where(x => x.Year == user.Year).OrderBy(x => x.Section).ToDictionary(x => x.ToString(), x => x.Id);

                    var response = new SendRequest(uid, new Message("Great! Now you need to select your class, by clicking on a button below. If you don't see your class, use the \"arrow\" buttons to scroll the list.", _selector.GetSelector(classes, user.ListOffset)));
                    await _apiClient.Send(response);
                    
                    break;
                }
            }
        }
    }
}