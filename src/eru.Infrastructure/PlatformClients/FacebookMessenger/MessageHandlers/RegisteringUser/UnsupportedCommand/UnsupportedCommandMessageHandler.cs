using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using eru.Application.Classes.Queries.GetClasses;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.Enums;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Selector;
using eru.Infrastructure.PlatformClients.FacebookMessenger.SendAPIClient;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.UnsupportedCommand
{
    public class UnsupportedCommandMessageHandler : IUnsupportedCommandMessageHandler
    {
        private readonly IMediator _mediator; 
        private readonly IRegistrationDbContext _dbContext;
        private readonly ISendApiClient _apiClient;
        private readonly IConfiguration _configuration;
        private readonly ISelector _selector;
        
        public UnsupportedCommandMessageHandler(IRegistrationDbContext dbContext, ISendApiClient apiClient, IMediator mediator, IConfiguration configuration, ISelector selector)
        {
            _dbContext = dbContext;
            _apiClient = apiClient;
            _mediator = mediator;
            _configuration = configuration;
            _selector = selector;
        }
        public async Task Handle(string uid)
        {
            var user = await _dbContext.IncompleteUsers.FindAsync(uid);
            
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
                    var yearsSet = new SortedSet<int>(classesInDb.Select(x => x.Year));
                    var dict = yearsSet.ToDictionary(x => x.ToString(), x => $"{ReplyPayloads.YearPrefix}{x.ToString()}");
                    var replies = _selector.GetSelector(dict, user.ListOffset);
                    
                    var response = new SendRequest(uid, new Message("Now select your class, in the same way as language.", replies));
                    await _apiClient.Send(response);
                    
                    break;
                }
                
                case Stage.GatheredYear:
                {
                    var classesInDb = await _mediator.Send(new GetClassesQuery());
                    var classes = classesInDb.Where(x => x.Year == user.Year).OrderBy(x => x.Section);
                    var dict = classes.ToDictionary(x => x.ToString(), x => x.Id);
                    var replies = _selector.GetSelector(dict, user.ListOffset);
                    
                    var response = new SendRequest(uid, new Message("Great! Now you need to select your class, by clicking on a button below. If you don't see your class, use the \"arrow\" buttons to scroll the list.", replies));
                    await _apiClient.Send(response);
                    
                    break;
                }
                
                case Stage.GatheredClass:
                {
                    var response = new SendRequest(uid, new Message("Now we have all the required informations to create your subscription. If you want to get a message about all substiututions concerning you as soon as the school publish that information, click the Subscribe button. If you want to delete (or modify) your data, click Cancel.", new []
                    {
                        new QuickReply("Subscribe", ReplyPayloads.SubscribePayload),
                        new QuickReply("Cancel", ReplyPayloads.CancelPayload) 
                    }));

                    await _apiClient.Send(response);
                    break;
                }
            }
        }
    }
}