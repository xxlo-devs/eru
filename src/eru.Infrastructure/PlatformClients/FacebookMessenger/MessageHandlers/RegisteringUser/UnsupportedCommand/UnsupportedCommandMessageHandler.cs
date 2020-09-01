using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eru.Application.Classes.Queries.GetClasses;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.Enums;
using eru.Infrastructure.PlatformClients.FacebookMessenger.SendAPIClient;
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
        
        public UnsupportedCommandMessageHandler(IRegistrationDbContext dbContext, ISendApiClient apiClient, IMediator mediator, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _apiClient = apiClient;
            _mediator = mediator;
            _configuration = configuration;
        }
        public async Task Handle(string uid)
        {
            var user = await _dbContext.IncompleteUsers.FindAsync(uid);
            
            switch (user.Stage)
            {
                case Stage.Created:
                {
                    var response = new SendRequest(uid, new Message("Great! Now you need to select your class, by clicking on a button below. If you don't see your class, use the \"arrow\" buttons to scroll the list.", new []
                    {
                        new QuickReply("Cancel", "cancel")
                    }));
                    await _apiClient.Send(response);
                    
                    break;
                }

                case Stage.GatheredLanguage:
                {
                    var classesInDb = await _mediator.Send(new GetClassesQuery());
                    var yearsSet = new SortedSet<int>();
                    foreach (var x in classesInDb)
                    {
                        yearsSet.Add(x.Year);
                    }

                    var years = yearsSet.Skip(user.ListOffset).Take(10).AsEnumerable();
                    
                    var replies = new List<QuickReply>();
                    foreach (var x in years)
                    {
                        replies.Add(new QuickReply(x.ToString(), $"{ReplyPayloads.YearPrefix}{x.ToString()}"));
                    }
                    
                    if(user.ListOffset > 0)
                        replies.Add(new QuickReply("<-", ReplyPayloads.PreviousPage));
                    
                    if(yearsSet.Count - user.ListOffset - 10 > 0)
                        replies.Add(new QuickReply("->", ReplyPayloads.NextPage));
                    
                    replies.Add(new QuickReply("Cancel", ReplyPayloads.CancelPayload));
                    
                    var response = new SendRequest(uid, new Message("", replies));
                    await _apiClient.Send(response);
                    
                    break;
                }
                
                case Stage.GatheredYear:
                {
                    var classesInDb = await _mediator.Send(new GetClassesQuery());
                    var classes = classesInDb.Where(x => x.Year == user.Year).OrderBy(x => x.Section).Skip(user.ListOffset).Take(10).AsEnumerable();

                    var replies = new List<QuickReply>();
                    foreach (var x in classes)
                    {
                        replies.Add(new QuickReply(x.ToString(), string.Format("{0}{1}", ReplyPayloads.ClassPrefix, x.Id)));
                    }
                    
                    if(user.ListOffset > 0)
                        replies.Add(new QuickReply("<-", ReplyPayloads.PreviousPage));
                    if(classesInDb.Count() - user.ListOffset - 10 > 0)
                        replies.Add(new QuickReply("->", ReplyPayloads.NextPage));
                    
                    replies.Add(new QuickReply("Cancel", ReplyPayloads.CancelPayload));
                    
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