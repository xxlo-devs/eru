using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Classes.Queries.GetClasses;
using eru.Application.Common.Interfaces;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.Enums;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Selector;
using eru.Infrastructure.PlatformClients.FacebookMessenger.SendAPIClient;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.GatherLanguage
{
    public class GatherLanguageMessageHandler : IGatherLanguageMessageHandler
    {
        private readonly IRegistrationDbContext _dbContext;
        private readonly ISendApiClient _apiClient;
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        private readonly ISelector _selector;
        private readonly ITranslator<FacebookMessengerPlatformClient> _translator;

        public GatherLanguageMessageHandler(IRegistrationDbContext dbContext, ISendApiClient apiClient, IMediator mediator, ISelector selector, IConfiguration configuration, ITranslator<FacebookMessengerPlatformClient> translator)
        {
            _dbContext = dbContext;
            _apiClient = apiClient;
            _mediator = mediator;
            _selector = selector;
            _configuration = configuration;
            _translator = translator;
        }
        public async Task Handle(string uid, Payload payload)
        {
            if (payload.Type == Type.Lang)
            {
                if (payload.Page != null)
                {
                    await ShowPage(uid, payload.Page.Value);
                    return;
                }

                if (payload.Id != null)
                {
                    await Gather(uid, payload.Id);
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

        private async Task Gather(string uid, string lang)
        {
            var user = await _dbContext.IncompleteUsers.FindAsync(uid);
            user.PreferredLanguage = lang;
            user.ListOffset = 0;
            user.Stage = Stage.GatheredLanguage;
            _dbContext.IncompleteUsers.Update(user);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
            
            var classesInDb = await _mediator.Send(new GetClassesQuery());
            var dict = new SortedSet<int>(classesInDb.Select(x => x.Year)).ToDictionary(x => x.ToString(), x => new Payload(Type.Year, x.ToString()).ToJson());
            
            var response = new SendRequest(uid, new Message(await _translator.TranslateString("year-selection", user.PreferredLanguage), _selector.GetSelector(dict, 0, Type.Year)));
            await _apiClient.Send(response);
        }
    }
}