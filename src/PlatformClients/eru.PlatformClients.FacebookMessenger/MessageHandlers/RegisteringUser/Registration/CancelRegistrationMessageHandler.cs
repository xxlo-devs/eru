﻿using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.PlatformClients.FacebookMessenger.SendAPIClient;
using Hangfire.Logging;
using Microsoft.Extensions.Logging;

namespace eru.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.CancelRegistration
{
    public class CancelRegistrationMessageHandler : RegistrationMessageHandler<CancelRegistrationMessageHandler>
    {
        // private readonly IRegistrationDbContext _dbContext;
        // private readonly ISendApiClient _apiClient;
        // private readonly ITranslator<FacebookMessengerPlatformClient> _translator;
        // private readonly ILogger<CancelRegistrationMessageHandler> _logger;
        //
        // public CancelRegistrationMessageHandler(IRegistrationDbContext dbContext, ISendApiClient apiClient, ITranslator<FacebookMessengerPlatformClient> translator, ILogger<CancelRegistrationMessageHandler> logger)
        // {
        //     _dbContext = dbContext;
        //     _apiClient = apiClient;
        //     _translator = translator;
        //     _logger = logger;
        // }
        // public async Task Handle(string uid)
        // {
        //     _logger.LogTrace($"eru.PltaformClients.FacebookMessenger: CancelRegistrationMessageHandler.Handle got a request (uid: {uid})");
        //     var user = await _dbContext.IncompleteUsers.FindAsync(uid);
        //     
        //     _dbContext.IncompleteUsers.Remove(user);
        //     await _dbContext.SaveChangesAsync(CancellationToken.None);
        //
        //     var response = new SendRequest(uid, new Message(await _translator.TranslateString("subscription-cancelled", user.PreferredLanguage)));
        //     await _apiClient.Send(response);
        //     _logger.LogInformation($"eru.PltaformClients.FacebookMessenger: CancelRegistrationMessageHandler.Handle has successfully cancelled registration for user (uid: {uid})");
        // }
        public override async Task GatherBase()
        {
            throw new System.NotImplementedException();
        }

        public override async Task ShowInstructionBase()
        {
            throw new System.NotImplementedException();
        }

        public override async Task ShowUnsupportedCommandBase()
        {
            throw new System.NotImplementedException();
        }
    }
}