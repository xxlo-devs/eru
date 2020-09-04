using MediatR;

namespace eru.Application.Notifications.Commands
{
    public class SendGlobalNotification : IRequest
    {
        public SendGlobalNotification(string content)
        {
            Content = content;
        }
        public string Content { get; set; }
    }
}