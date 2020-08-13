using System;
using System.Threading;
using eru.Application.Users.Queries.GetIdsOfAllSubscribersInPlatform;
using System.Threading.Tasks;
using eru.Domain.Entity;
using eru.Domain.Enums;
using FluentAssertions;
using Xunit;
using Xunit.Sdk;
using System.Collections.Generic;

namespace eru.Application.Tests.Users.Queries
{
    public class GetIdsOfAllSubscribersInPlatformQueryTests
    {
        [Fact]
        public async Task ShouldReturnAllUsersFromGivenPlatformCorrectly()
        {
            var context = new FakeDbContext();
            var handler = new GetIdsOfAllSubscribersInPlatformQueryHandler(context);
            var request = new GetIdsOfAllSubscribersInPlatformQuery
            {
                Platform = Platform.DebugMessageService
            };

            var expected = new List<string>
            {
                new string("98DFFEBA-BEB4-4D76-8C89-78857D7B7A2F"),
                new string("195CC4D0-80F5-4745-86AC-7FCD3BAF209B"),
                new string("7124C49B-B04A-468F-A946-40025B19FF91"),
                new string("380AE765-803D-4174-A370-1038B7D53CD6"),
                new string("FCDEE5DA-F755-45F9-B8BB-D7C7C303F70B")
            };

            var actual = await handler.Handle(request, CancellationToken.None);

            actual.Should().HaveCount(5).And.BeEquivalentTo(expected);
        }
    }
}