using FluentValidation;

namespace eru.Application.Notifications.Commands
{
    public class SendGlobalNotificationValidator : AbstractValidator<SendGlobalNotification>
    {
        public SendGlobalNotificationValidator()
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content cannot be empty.")
                .MaximumLength(600).WithMessage("Content must have length up to 600 characters.");
        }
    }
}