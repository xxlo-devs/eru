using System;
using System.Linq;
using System.Text.Unicode;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Classes.Queries.GetClasses;
using eru.Application.Common.Interfaces;
using eru.Application.Subscriptions.Queries.GetSubscriber;
using eru.Domain.Entity;
using eru.Infrastructure.PlatformClients.FacebookMessenger;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Selector;
using eru.Infrastructure.PlatformClients.FacebookMessenger.SendAPIClient;
using MediatR;
using Microsoft.EntityFrameworkCore.Internal;
using Moq;
using Xunit;

namespace eru.Infrastructure.Tests.PlatformClients.FacebookMessenger.PlatformClient
{
    public class PlatformClientTests
    {
        [Fact]
        public async void ShouldSendGenericMessageCorrectly()
        {
            var builder = new PlatformClientBuilder();
            
            await builder.PlatformClient.SendMessage("sample-subscriber", "A test message.");
            builder.ApiClientMock.Verify(x => x.Send(It.IsAny<SendRequest>()), Times.Once);
        }

        [Fact]
        public async void ShouldSendSubstitutionsCorrectly()
        {
            var builder = new PlatformClientBuilder();
            
            await builder.PlatformClient.SendMessage("sample-subscriber", new[]
            {
                new Substitution{Teacher = "sample-teacher", Lesson = 1, Subject = "sample-subject", Classes = new[] {new Class(1, "a"), new Class(1, "b") }, Groups = "sample-group", Note = "sample-note", Room = "sample-room", Cancelled = true}, 
                new Substitution{Teacher = "sample-teacher-2", Lesson = 2, Subject = "sample-subject-2", Classes = new[] {new Class(1, "a")}, Groups = "sample-group-2", Note = "sample-note-2", Room = "sample-room-2", Substituting = "sample-teacher-3"}
            });
            
            builder.ApiClientMock.Verify(x => x.Send(It.IsAny<SendRequest>()), Times.Exactly(4));
        }
    }
}