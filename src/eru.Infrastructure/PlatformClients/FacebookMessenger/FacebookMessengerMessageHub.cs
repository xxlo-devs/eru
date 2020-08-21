using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using eru.Application.Classes.Queries.GetClasses;
using eru.Application.Users.Commands.CancelSubscription;
using eru.Application.Users.Commands.CreateUser;
using eru.Application.Users.Queries.GetUser;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendAPI;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.Webhook.Messages;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDbContext;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Message = eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendAPI.Message;
using QuickReply = eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendAPI.QuickReply;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger
{
    public class FacebookMessengerMessageHub
    {
        private readonly FacebookMessengerRegistrationDbContext _localDbContext;
        private readonly IMediator _mediator;
        private readonly IStringLocalizer<FacebookMessengerMessageHub> _localizer;
        private readonly IConfiguration _configuration;

        public FacebookMessengerMessageHub(IConfiguration configuration, IMediator mediator, IStringLocalizer<FacebookMessengerMessageHub> localizer, FacebookMessengerRegistrationDbContext localDbContext)
        {
            _configuration = configuration;
            _mediator = mediator;
            _localizer = localizer;
            _localDbContext = localDbContext;
        }

        public async Task HandleIncomingMessage(Messaging message)
        {
            if (await _mediator.Send(new GetUserQuery {UserId = message.Sender.Id, Platform = FacebookMessengerPlatformClient.StaticPlatformId}) != null || await _localDbContext.IncompleteUsers.FindAsync(message.Sender.Id) != null)
            {
                if (message.Message.QuickReply.Payload != null)
                {
                    if (message.Message.QuickReply.Payload == ReplyPayloads.CancelPayload)
                    {
                        await CancelSubscription(message.Sender.Id);
                    }
                    else
                    {
                        await HandleKnownUserRequest(message.Sender.Id, message.Message.QuickReply.Payload);
                    }
                }
                else
                {
                    await UnsupportedCommand(message.Sender.Id);
                }
            }
            else
            {
                await HandleUnknownUserRequest(message.Sender.Id);
            }
        }

        private async Task CancelSubscription(string uid)
        {
            var globalUser = await _mediator.Send(new GetUserQuery {UserId = uid, Platform = FacebookMessengerPlatformClient.StaticPlatformId});
            var localUser = await _localDbContext.IncompleteUsers.FindAsync(uid);

            if (globalUser != null)
            {
                CultureInfo.CurrentCulture = new CultureInfo(globalUser.PreferredLanguage);
                CultureInfo.CurrentUICulture = new CultureInfo(globalUser.PreferredLanguage);
                await _mediator.Send(new CancelSubscriptionCommand {UserId = uid, Platform = FacebookMessengerPlatformClient.StaticPlatformId});
            }

            if (localUser != null)
            {
                CultureInfo.CurrentCulture = new CultureInfo(localUser.PreferredLanguage);
                CultureInfo.CurrentUICulture = new CultureInfo(localUser.PreferredLanguage);
                _localDbContext.IncompleteUsers.Remove(localUser);
                await _localDbContext.SaveChangesAsync();
            }

            await Send(new SendRequest(uid, new Message(_localizer["subscription-cancelled"])));
        }

        private async Task UnsupportedCommand(string uid)
        {
            var globalUser = await _mediator.Send(new GetUserQuery {UserId = uid, Platform = FacebookMessengerPlatformClient.StaticPlatformId});
            var localUser = await _localDbContext.IncompleteUsers.FindAsync(uid);

            if (globalUser != null)
            {
                CultureInfo.CurrentCulture = new CultureInfo(globalUser.PreferredLanguage);
                CultureInfo.CurrentUICulture = new CultureInfo(globalUser.PreferredLanguage);
                var response = new SendRequest(uid, new Message(_localizer["unsupported-command"], GetCancelButton()));

                await Send(response);
            }

            if (localUser != null)
            {
                switch (localUser.Stage)
                {
                    case Stage.Created: 
                        await Send(new SendRequest(uid, 
                            new Message(_localizer["unsupported-command"], GetLangSelector())));
                        break;

                    case Stage.GatheredLanguage:
                        CultureInfo.CurrentCulture = new CultureInfo(localUser.PreferredLanguage);
                        CultureInfo.CurrentUICulture = new CultureInfo(localUser.PreferredLanguage);
                        await Send(new SendRequest(uid,
                            new Message(_localizer["unsupported-command"], await GetClassSelector(localUser.ClassOffset))));
                        break;

                    case Stage.GatheredClass:
                        CultureInfo.CurrentCulture = new CultureInfo(localUser.PreferredLanguage);
                        CultureInfo.CurrentUICulture = new CultureInfo(localUser.PreferredLanguage);
                        await Send(new SendRequest(uid, 
                            new Message(_localizer["unsupported-command"], GetConfirmationSelector())));
                        break;
                }
            }
        }

        private async Task HandleKnownUserRequest(string uid, string payload)
        {
            var user = await _localDbContext.IncompleteUsers.FindAsync(uid);

            if (user.Stage == Stage.Created)
            {
                user.PreferredLanguage = payload;
                user.Stage = Stage.GatheredLanguage;
                _localDbContext.Update(user);
                await _localDbContext.SaveChangesAsync();

                CultureInfo.CurrentCulture = new CultureInfo(user.PreferredLanguage);
                CultureInfo.CurrentUICulture = new CultureInfo(user.PreferredLanguage);

                var reply = new SendRequest(uid, new Message(_localizer["class-selection"], await GetClassSelector(user.ClassOffset)));
                await Send(reply);
                return;
            }

            if (user.Stage == Stage.GatheredLanguage)
            {
                CultureInfo.CurrentCulture = new CultureInfo(user.PreferredLanguage);
                CultureInfo.CurrentUICulture = new CultureInfo(user.PreferredLanguage);

                if (payload == ReplyPayloads.PreviousPage)
                {
                    user.ClassOffset -= 10;
                    _localDbContext.Update(user);
                    await _localDbContext.SaveChangesAsync();

                    var reply = new SendRequest(uid, new Message(_localizer["class-selection"], await GetClassSelector(user.ClassOffset)));
                    await Send(reply);
                    return;
                }

                if (payload == ReplyPayloads.NextPage)
                {
                    user.ClassOffset += 10;
                    _localDbContext.Update(user);
                    await _localDbContext.SaveChangesAsync();

                    var reply = new SendRequest(uid, new Message(_localizer["class-selection"], await GetClassSelector(user.ClassOffset)));
                    await Send(reply);
                    return;
                }

                if (payload != ReplyPayloads.PreviousPage & payload != ReplyPayloads.NextPage)
                {
                    user.Class = payload;
                    user.Stage = Stage.GatheredClass;
                    _localDbContext.Update(user);
                    await _localDbContext.SaveChangesAsync();

                    var reply = new SendRequest(uid, new Message(_localizer["confirmation"], GetConfirmationSelector()));
                    await Send(reply);
                    return;
                }
            }

            if (user.Stage == Stage.GatheredClass)
            {
                if (payload == ReplyPayloads.SubscribePayload)
                {
                    CultureInfo.CurrentCulture = new CultureInfo(user.PreferredLanguage);
                    CultureInfo.CurrentUICulture = new CultureInfo(user.PreferredLanguage);

                    var command = new CreateUserCommand
                    {
                        Id = user.Id,
                        Platform = user.Platform,
                        Class = user.Class,
                        PreferredLanguage = user.Class
                    };

                    await _mediator.Send(command);

                    _localDbContext.Remove(user);
                    await _localDbContext.SaveChangesAsync();

                    var response = new SendRequest(uid, new Message(_localizer["congratulations"], GetCancelButton()));
                    await Send(response);
                    return;
                }
            }
        }

        private async Task HandleUnknownUserRequest(string uid)
        {
            var user = new IncompleteUser
            {
                Id = uid,
                Platform = FacebookMessengerPlatformClient.StaticPlatformId,
                Stage = Stage.Created
            };

            await _localDbContext.AddAsync(user);
            await _localDbContext.SaveChangesAsync();

            var reply = new SendRequest(user.Id, new Message(_localizer["greeting"], GetLangSelector()));

            await Send(reply);
        }

        private IEnumerable<QuickReply> GetCancelButton() => new[] { new QuickReply(_localizer["cancel-button"], ReplyPayloads.CancelPayload) };

        private IEnumerable<QuickReply> GetLangSelector() => new []
        {
            new QuickReply(_localizer["cancel-button"], ReplyPayloads.CancelPayload),
            new QuickReply("English", ReplyPayloads.English),
            new QuickReply("Polski", ReplyPayloads.Polish)
        };

        private IEnumerable<QuickReply> GetConfirmationSelector() => new []
        {
            new QuickReply(_localizer["subscribe-button"], ReplyPayloads.SubscribePayload),
            new QuickReply(_localizer["cancel-button"], ReplyPayloads.CancelPayload)
        };

        private async Task<IEnumerable<QuickReply>> GetClassSelector(int offset)
        {
            var classes = await _mediator.Send(new GetClassesQuery());
            var classNames = classes
                .Skip(offset)
                .Take(10)
                .Select(x => x.Name);

            var replies = new List<Models.SendAPI.QuickReply>();

            if (offset > 0)
            {
                replies.Add(new QuickReply(_localizer["previous"], ReplyPayloads.PreviousPage));
            }

            foreach (var x in classNames)
            {
                replies.Add(new QuickReply(x, x));
            }

            if (classNames.Count() == 10)
            {
                if (classes.Count() - offset - 10 > 0)
                {
                    replies.Add(new QuickReply(_localizer["next"], ReplyPayloads.NextPage));
                }
            }

            replies.Add(new QuickReply(_localizer["cancel-button"], ReplyPayloads.CancelPayload));

            return replies;
        }

        private async Task Send(SendRequest req)
        {
            throw new NotImplementedException();
        }
    }
}