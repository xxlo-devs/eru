﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Classes.Queries.GetClasses;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.Enums;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Selector;
using eru.Infrastructure.PlatformClients.FacebookMessenger.SendAPIClient;
using FluentAssertions;
using MediatR;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherLanguage
{
    public class GatherLanguageMessageHandler : IGatherLanguageMessageHandler
    {
        private readonly IRegistrationDbContext _dbContext;
        private readonly ISendApiClient _apiClient;
        private readonly IMediator _mediator;
        private readonly ISelector _selector;

        public GatherLanguageMessageHandler(IRegistrationDbContext dbContext, ISendApiClient apiClient, IMediator mediator, ISelector selector)
        {
            _dbContext = dbContext;
            _apiClient = apiClient;
            _mediator = mediator;
            _selector = selector;
        }
        public async Task Handle(string uid, string payload)
        {
            var user = await _dbContext.IncompleteUsers.FindAsync(uid);
            user.PreferredLanguage = payload.Substring(ReplyPayloads.LangPrefix.Length);
            user.ListOffset = 0;
            user.Stage = Stage.GatheredLanguage;
            _dbContext.IncompleteUsers.Update(user);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
            
            var classesInDb = await _mediator.Send(new GetClassesQuery());
            var dict = new SortedSet<int>(classesInDb.Select(x => x.Year)).ToDictionary(x => x.ToString(), x => $"{ReplyPayloads.YearPrefix}{x.ToString()}");

            var response = new SendRequest(uid, new Message("", _selector.GetSelector(dict, user.ListOffset)));
            await _apiClient.Send(response);
        }
    }
}