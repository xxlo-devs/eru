using System.Linq;
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

            builder.ApiClientMock.Verify(x => x.Send(It.Is<SendRequest>(y =>
                y.Type == MessagingTypes.MessageTag
                && y.Tag == MessageTags.AccountUpdate
                && y.Recipient.Id == "sample-subscriber"
                && y.Message.Text == "A test message."
                && y.Message.QuickReplies.Count() == 1
                && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "cancel-button-text" && z.Payload == new Payload(PayloadType.Cancel).ToJson())
            )));

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
            
            builder.ApiClientMock.Verify(x => x.Send(It.Is<SendRequest>(y =>
                y.Type == MessagingTypes.MessageTag
                && y.Tag == MessageTags.ConfirmedEventUpdate
                && y.Recipient.Id == "sample-subscriber"
                && y.Message.Text == "new-substitutions-text"
                && y.Message.QuickReplies == null
            )), Times.Once);
            
            builder.ApiClientMock.Verify(x => x.Send(It.Is<SendRequest>(y =>
                y.Type == MessagingTypes.MessageTag
                && y.Tag == MessageTags.ConfirmedEventUpdate
                && y.Recipient.Id == "sample-subscriber"
                && y.Message.Text == "CANCELLATION | 1 | sample-subject | sample-teacher | sample-room | sample-note"
                && y.Message.QuickReplies == null
            )), Times.Once);
            
            builder.ApiClientMock.Verify(x => x.Send(It.Is<SendRequest>(y =>
                y.Type == MessagingTypes.MessageTag
                && y.Tag == MessageTags.ConfirmedEventUpdate
                && y.Recipient.Id == "sample-subscriber"
                && y.Message.Text == "SUBSTITUTION | sample-teacher-2 | 2 | sample-subject-2 | sample-teacher-3 | sample-room-2 | sample-note-2"
                && y.Message.QuickReplies == null
            )), Times.Once);
            
            builder.ApiClientMock.Verify(x => x.Send(It.Is<SendRequest>(y =>
                y.Type == MessagingTypes.MessageTag
                && y.Tag == MessageTags.ConfirmedEventUpdate
                && y.Recipient.Id == "sample-subscriber"
                && y.Message.Text == "closing-substitution-text"
                && y.Message.QuickReplies.Count() == 1
                && y.Message.QuickReplies.Any(z => z.ContentType == QuickReplyContentTypes.Text && z.Title == "cancel-button-text" && z.Payload == new Payload(PayloadType.Cancel).ToJson())
            )), Times.Once);
            
            builder.ApiClientMock.VerifyNoOtherCalls();
        }
    }
}