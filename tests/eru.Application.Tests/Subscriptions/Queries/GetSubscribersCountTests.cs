using System.Threading;
using eru.Application.Subscriptions.Queries.GetSubscribersCount;
using FluentAssertions;
using Xunit;

namespace eru.Application.Tests.Subscriptions.Queries
{
    public class GetSubscribersCountTests
    {
        [Fact]
        public async void ShouldGetSubscribersCountCorrectly()
        {
            var db = new FakeDbContext();
            var handler = new GetSubscribersCountHandler(db);

            var request = new GetSubscribersCount();

            var result = await handler.Handle(request, CancellationToken.None);
            result.Should().Be(1);
        }

        [Fact]
        public async void ShouldGetSubscribersCountFromClassCorrectly()
        {
            var db = new FakeDbContext();
            var validator = new GetSubscribersCountHandler(db);

            var request = new GetSubscribersCount
            {
                ClassId = MockData.ExistingClassId
            };

            var result = await validator.Handle(request, CancellationToken.None);

            result.Should().Be(1);
        }

        [Fact]
        public async void DoesValidatorAllowCorrectRequestWithoutClassId()
        {
            var db = new FakeDbContext();
            var validator = new GetSubscribersCountValidator(db);

            var request = new GetSubscribersCount();

            var result = await validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }
        
        [Fact]
        public async void DoesValidatorAllowCorrectRequestWithClassId()
        {
            var db = new FakeDbContext();
            var validator = new GetSubscribersCountValidator(db);

            var request = new GetSubscribersCount
            {
                ClassId = MockData.ExistingClassId
            };

            var result = await validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async void DoesValidatorPreventFromGettingCountFromNonExistingClass()
        {
            var db = new FakeDbContext();
            var validator = new GetSubscribersCountValidator(db);
            
            var request = new GetSubscribersCount
            {
                ClassId = "non-existing-class"
            };

            var result = await validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1).And.ContainSingle(x => x.ErrorCode == "AsyncPredicateValidator" & x.ErrorMessage == "Mentioned class must already exist.");
        }
    }
}