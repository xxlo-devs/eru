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
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendAPI.Properties;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendAPI.Static;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.Webhook.Messaging;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDbContext;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Message = eru.Infrastructure.PlatformClients.FacebookMessenger.Models.Webhook.Messaging.Message;

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
            if (await _mediator.Send(new GetUserQuery {UserId = message.Sender.Id, Platform = FacebookMessengerPlatformClient.StaticPlatformId}) != null)
            {
                await HandleRegistratedUserRequest(message);
                return;
            }

            if (await _localDbContext.IncompleteUsers.FindAsync(message.Sender.Id) != null)
            {
                await HandleUserBeingRegistratedRequest(message);
                return;
            }

            await HandleNewUserRequest(message);
            return;
        }

        private async Task HandleRegistratedUserRequest(Messaging message)
        {
            var user = await _mediator.Send(new GetUserQuery {UserId = message.Sender.Id, Platform = FacebookMessengerPlatformClient.StaticPlatformId});
            CultureInfo.CurrentCulture = new CultureInfo(user.PreferredLanguage);
            CultureInfo.CurrentUICulture = new CultureInfo(user.PreferredLanguage);

            if (message.Message.QuickReply.Payload == ReplyPayloads.CancelPayload)
            {
                await CancelSubscription(message.Sender.Id);
            }

            var response = new Request
            {
                Type = MessagingTypes.Response,
                Recipient = new Recipient(user.Id),
                Message = new Models.SendAPI.Message
                {
                    Text = _localizer["unsupported-command"],
                    QuickReplies = new Models.SendAPI.QuickReply[]
                    {
                        new Models.SendAPI.QuickReply
                        {
                            ContentType = QuickReplyContentTypes.Text,
                            Title = _localizer["cancel-button"],
                            Payload = ReplyPayloads.CancelPayload
                        }
                    }
                }
            };

            await Send(response);
        }

        private async Task HandleUserBeingRegistratedRequest(Messaging message)
        {
            var user = await _localDbContext.IncompleteUsers.FindAsync(message.Sender.Id);

            if (user.Stage == Stage.Created)
            {
                await GatherLanguage(user.Id, message.Message);
                return;
            }

            if (user.Stage == Stage.GatheredLanguage)
            {
                await GatherClass(user.Id, message.Message);
                return;
            }

            if (user.Stage == Stage.GatheredClass)
            {
                await ConfirmSubscription(user.Id, message.Message);
                return;
            }
        }

        private async Task HandleNewUserRequest(Messaging message)
        {
            var user = new IncompleteUser
            {
                Id = message.Sender.Id,
                Platform = FacebookMessengerPlatformClient.StaticPlatformId,
                Stage = Stage.Created
            };

            await _localDbContext.AddAsync(user);
            await _localDbContext.SaveChangesAsync();

            var reply = new Request
            {
                Type = MessagingTypes.Response,
                Recipient = new Recipient(user.Id),
                Message = new Models.SendAPI.Message
                {
                    Text = _localizer["greeting"],
                    QuickReplies = new Models.SendAPI.QuickReply[]
                    {
                        new Models.SendAPI.QuickReply
                        {
                            ContentType = QuickReplyContentTypes.Text,
                            Title = _localizer["cancel-button"],
                            Payload = ReplyPayloads.CancelPayload
                        },
                        new Models.SendAPI.QuickReply
                        {
                            ContentType = QuickReplyContentTypes.Text,
                            Title = "English",
                            Payload = ReplyPayloads.LanguagePayload("en")
                        },
                        new Models.SendAPI.QuickReply
                        {
                            ContentType = QuickReplyContentTypes.Text,
                            Title = "Polski",
                            Payload = ReplyPayloads.LanguagePayload("pl")
                        }
                    }
                }
            };

            await Send(reply);
        }

        private async Task ConfirmSubscription(string uid, Message msg)
        {
            if (msg.QuickReply.Payload == ReplyPayloads.SubscribePayload)
            {
                var tempUser = await _localDbContext.IncompleteUsers.FindAsync(uid);
                var command = new CreateUserCommand
                {
                    Id = tempUser.Id,
                    Platform = tempUser.Platform,
                    Class = tempUser.Platform,
                    PreferredLanguage = tempUser.PreferredLanguage
                };

                await _mediator.Send(command);

                var reply = new Request
                {
                    Type = MessagingTypes.Response,
                    Recipient = new Recipient(uid),
                    Message = new Models.SendAPI.Message
                    {
                        Text = _localizer["congratulations"]
                    }
                };

                await Send(reply);
                return;
            }

            if (msg.QuickReply.Payload == ReplyPayloads.CancelPayload)
            {
                await CancelSubscription(uid);
                return;
            }

            var response = new Request
            {
                Type = MessagingTypes.Response,
                Recipient = new Recipient(uid),
                Message = new Models.SendAPI.Message
                {
                    Text = _localizer["unsupported-command"],
                    QuickReplies = new Models.SendAPI.QuickReply[]
                    {
                        new Models.SendAPI.QuickReply
                        {
                            ContentType = QuickReplyContentTypes.Text,
                            Title = _localizer["subscribe-button"],
                            Payload = ReplyPayloads.SubscribePayload
                        },
                        new Models.SendAPI.QuickReply
                        {
                            ContentType = QuickReplyContentTypes.Text,
                            Title = _localizer["cancel-button"],
                            Payload = ReplyPayloads.CancelPayload
                        }
                    }
                }
            };

            await Send(response);
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

            var reply = new Request
            {
                Type = MessagingTypes.Response,
                Recipient = new Recipient(uid),
                Message = new Models.SendAPI.Message
                {
                    Text = _localizer["subscription-cancelled"]
                }
            };

            await Send(reply);
        }

        private async Task GatherLanguage(string uid, Message msg)
        {
            if (msg.QuickReply.Payload != null)
            {
                var user = await _localDbContext.IncompleteUsers.FindAsync(uid);

                if (msg.QuickReply.Payload == ReplyPayloads.CancelPayload)
                {
                    await CancelSubscription(uid);
                    return;
                }

                if (msg.QuickReply.Payload == ReplyPayloads.LanguagePayload("en"))
                {
                    user.PreferredLanguage = "en";
                    user.Stage = Stage.GatheredLanguage;
                    CultureInfo.CurrentCulture = new CultureInfo(user.PreferredLanguage);
                    CultureInfo.CurrentUICulture = new CultureInfo(user.PreferredLanguage);
                }

                if (msg.QuickReply.Payload == ReplyPayloads.LanguagePayload("pl"))
                {
                    user.PreferredLanguage = "pl";
                    user.Stage = Stage.GatheredLanguage;
                    CultureInfo.CurrentCulture = new CultureInfo(user.PreferredLanguage);
                    CultureInfo.CurrentUICulture = new CultureInfo(user.PreferredLanguage);
                }

                _localDbContext.IncompleteUsers.Update(user);
                await _localDbContext.SaveChangesAsync();

                var quickReplies = await GetClasses(0);

                if (quickReplies.Count == 10)
                {
                    quickReplies.Add(new Models.SendAPI.QuickReply
                    {
                        ContentType = "QuickReplyContentTypes.Text",
                        Title = _localizer["next"],
                        Payload = ReplyPayloads.NextPage
                    });
                }

                quickReplies.Add(new Models.SendAPI.QuickReply
                {
                    ContentType = QuickReplyContentTypes.Text,
                    Title = _localizer["cancel-button"],
                    Payload = ReplyPayloads.CancelPayload
                });

                var reply = new Request
                {
                    Type = MessagingTypes.Response,
                    Recipient = new Recipient(uid),
                    Message = new Models.SendAPI.Message
                    {
                        Text = _localizer["class-selection"],
                        QuickReplies = quickReplies
                    }
                };

                await Send(reply);
            }
            else
            {
                var response = new Request
                {
                    Type = MessagingTypes.Response,
                    Recipient = new Recipient(uid),
                    Message = new Models.SendAPI.Message
                    {
                        Text = _localizer["unsupported-command"],
                        QuickReplies = new Models.SendAPI.QuickReply[]
                        {
                            new Models.SendAPI.QuickReply
                            {
                                ContentType = QuickReplyContentTypes.Text,
                                Title = _localizer["cancel-button"],
                                Payload = ReplyPayloads.CancelPayload
                            }
                        }
                    }
                };

                await Send(response);
            }
        }

        private async Task GatherClass(string uid, Message msg)
        {
            if (msg.QuickReply.Payload != null)
            {
                var user = await _localDbContext.IncompleteUsers.FindAsync(uid);

                if (msg.QuickReply.Payload == ReplyPayloads.CancelPayload)
                {
                    await CancelSubscription(uid);
                    return;
                }

                if (msg.QuickReply.Payload == ReplyPayloads.NextPage)
                {
                    user.ClassOffset = user.ClassOffset + 10;
                    var buttons = await GetClasses(user.ClassOffset);
                    
                    if (buttons.Count == 10)
                    {
                        buttons.Add(new Models.SendAPI.QuickReply
                        {
                            ContentType = QuickReplyContentTypes.Text,
                            Title = _localizer["next"],
                            Payload = ReplyPayloads.NextPage
                        });
                    }

                    buttons.Add(new Models.SendAPI.QuickReply
                    {
                        ContentType = QuickReplyContentTypes.Text,
                        Title = _localizer["previous"],
                        Payload = ReplyPayloads.PreviousPage
                    });

                    buttons.Add(new Models.SendAPI.QuickReply
                    {
                        ContentType = QuickReplyContentTypes.Text,
                        Title = _localizer["cancel-button"],
                        Payload = ReplyPayloads.CancelPayload
                    });

                    var reply = new Request
                    {
                        Type = MessagingTypes.Response,
                        Recipient = new Recipient(uid),
                        Message = new Models.SendAPI.Message
                        {
                            Text = _localizer["class-selection"],
                            QuickReplies = buttons
                        }
                    };

                    await Send(reply);
                    return;
                }

                if (msg.QuickReply.Payload == ReplyPayloads.PreviousPage)
                {
                    user.ClassOffset = user.ClassOffset - 10;

                    var buttons = await GetClasses(user.ClassOffset);

                    buttons.Add(new Models.SendAPI.QuickReply
                    {
                        ContentType = QuickReplyContentTypes.Text,
                        Title = _localizer["previous"],
                        Payload = ReplyPayloads.NextPage
                    });

                    buttons.Add(new Models.SendAPI.QuickReply
                    {
                        ContentType = QuickReplyContentTypes.Text,
                        Title = _localizer["cancel-button"],
                        Payload = ReplyPayloads.CancelPayload
                    });

                    if (user.ClassOffset >= 10)
                    {
                        buttons.Add(new Models.SendAPI.QuickReply
                        {
                            ContentType = QuickReplyContentTypes.Text,
                            Title = _localizer["previous"],
                            Payload = ReplyPayloads.PreviousPage
                        });
                    }

                    var reply = new Request
                    {
                        Type = MessagingTypes.Response,
                        Recipient = new Recipient(uid),
                        Message = new Models.SendAPI.Message
                        {
                            Text = _localizer["class-selection"],
                            QuickReplies = buttons
                        }
                    };

                    await Send(reply);
                    return;
                }

                user.Class = msg.QuickReply.Payload;
                user.Stage = Stage.GatheredClass;
                _localDbContext.IncompleteUsers.Update(user);
                await _localDbContext.SaveChangesAsync();

                var response = new Request
                {
                    Type = MessagingTypes.Response,
                    Recipient = new Recipient(uid),
                    Message = new Models.SendAPI.Message
                    {
                        Text = _localizer["confirmation"],
                        QuickReplies = new Models.SendAPI.QuickReply[]
                        {
                            new Models.SendAPI.QuickReply
                            {
                                ContentType = QuickReplyContentTypes.Text,
                                Title = _localizer["subscribe-button"],
                                Payload = ReplyPayloads.SubscribePayload
                            },
                            new Models.SendAPI.QuickReply
                            {
                                ContentType = QuickReplyContentTypes.Text,
                                Title = _localizer["cancel-button"],
                                Payload = ReplyPayloads.CancelPayload
                            }
                        }
                    }
                };

                await Send(response);
            }
            else
            {
                var response = new Request
                {
                    Type = MessagingTypes.Response,
                    Recipient = new Recipient(uid),
                    Message = new Models.SendAPI.Message
                    {
                        Text = _localizer["unsupported-command"],
                        QuickReplies = new Models.SendAPI.QuickReply[]
                        {
                            new Models.SendAPI.QuickReply
                            {
                                ContentType = QuickReplyContentTypes.Text,
                                Title = _localizer["cancel-button"],
                                Payload = ReplyPayloads.CancelPayload
                            }
                        }
                    }
                };
            }
        }

        private async Task<List<Models.SendAPI.QuickReply>> GetClasses(int offset)
        {
            var classes = await _mediator.Send(new GetClassesQuery());
            var classNames = classes
                .Skip(offset)
                .Take(10)
                .Select(x => x.Name);

            var replies = new List<Models.SendAPI.QuickReply>();

            foreach (var x in classNames)
            {
                replies.Add(new Models.SendAPI.QuickReply
                {
                    ContentType = QuickReplyContentTypes.Text,
                    Title = x,
                    Payload = x,
                });
            }

            return replies;
        }

        private async Task Send(Request req)
        {
            throw new NotImplementedException();
        }
    }
}