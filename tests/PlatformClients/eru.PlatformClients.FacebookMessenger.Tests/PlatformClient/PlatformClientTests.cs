using eru.Domain.Entity;
using eru.PlatformClients.FacebookMessenger.ReplyPayload;
using eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests;
using eru.PlatformClients.FacebookMessenger.SendAPIClient.Requests.Static;
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

            builder.ApiClientMock.Verify(x => x.Send(It.Is<SendRequest>(y => y.IsEquivalentTo(
                new SendRequest("sample-subscriber", new Message("A test message.", new[]
                {
                    new QuickReply("cancel-button-text", new Payload(PayloadType.Cancel).ToJson()) 
                }), MessageTags.AccountUpdate)
            ))));
            builder.ApiClientMock.VerifyNoOtherCalls();
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
            
            builder.ApiClientMock.Verify(
                x => x.Send(It.Is<SendRequest>(y => y.IsEquivalentTo(new SendRequest("sample-subscriber",
                    new Message("new-substitutions-text"), MessageTags.ConfirmedEventUpdate)))), Times.Once);
            
            builder.ApiClientMock.Verify(
                x => x.Send(It.Is<SendRequest>(y => y.IsEquivalentTo(new SendRequest("sample-subscriber",
                    new Message("CANCELLATION | 1 | sample-subject | sample-teacher | sample-room | sample-note"),
                    MessageTags.ConfirmedEventUpdate)))), Times.Once);
            
            builder.ApiClientMock.Verify(
                x => x.Send(It.Is<SendRequest>(y => y.IsEquivalentTo(new SendRequest("sample-subscriber",
                    new Message(
                        "SUBSTITUTION | sample-teacher-2 | 2 | sample-subject-2 | sample-teacher-3 | sample-room-2 | sample-note-2"),
                    MessageTags.ConfirmedEventUpdate)))), Times.Once);
            
            builder.ApiClientMock.Verify(
                x => x.Send(It.Is<SendRequest>(y => y.IsEquivalentTo(new SendRequest("sample-subscriber",
                    new Message("closing-substitutions-text",
                        new[] {new QuickReply("cancel-button-text", new Payload(PayloadType.Cancel).ToJson())}),
                    MessageTags.ConfirmedEventUpdate)))), Times.Once);
            
            builder.ApiClientMock.VerifyNoOtherCalls();
        }
    }
}