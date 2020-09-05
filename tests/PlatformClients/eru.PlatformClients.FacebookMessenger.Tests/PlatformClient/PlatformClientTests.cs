using eru.Domain.Entity;
using eru.PlatformClients.FacebookMessenger.Models.SendApi;
using Moq;
using Xunit;

namespace eru.PlatformClients.FacebookMessenger.Tests.PlatformClient
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