using System;
using System.Linq;
using System.Threading;
using eru.Application.Common.Interfaces;
using eru.Application.Notifications.Commands;
using eru.Domain.Entity;
using FluentAssertions;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Moq;
using Xunit;

namespace eru.Application.Tests.Notifications.Commands
{
    public class SendGlobalNotificationHandlerTests
    {
        //There could be more tests...
        [Fact]
        public void ShouldCorrectlySendNotificationToAllSubscribers()
        {
            var context = new FakeDbContext();
            var mockPlatformClient = new Mock<IPlatformClient>();
            mockPlatformClient.Setup(x => x.PlatformId).Returns("DebugMessageService");
            var platformClients = new[]
            {
                mockPlatformClient.Object
            };
            var backgroundJobClient = new Mock<IBackgroundJobClient>();
            var handler = new SendGlobalNotificationHandler(platformClients, context, backgroundJobClient.Object);
            var request = new SendGlobalNotification("test");

            handler.Handle(request, CancellationToken.None);
            
            backgroundJobClient.Verify(x => x.Create(
                It.Is<Job>(job => job.Method.Name == "SendMessage" && ReferenceEquals(job.Args[0], "sample-subscriber") && ReferenceEquals(job.Args[1], "test")),
                It.IsAny<EnqueuedState>()));
        }

        [Fact]
        public async void DoesValidatorPreventsFromSendingEmptyNotification()
        {
            var request = new SendGlobalNotification("");
            var validator = new SendGlobalNotificationValidator();

            var result = await validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public async void DoesValidatorPreventsFromSendingTooLongNotification()
        {
            var request = new SendGlobalNotification(Enumerable.Repeat("A", 700).Aggregate((s1, s2) => s1+s2));
            var validator = new SendGlobalNotificationValidator();

            var result = await validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
        }
    }
}