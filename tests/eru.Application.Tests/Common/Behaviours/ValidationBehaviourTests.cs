using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Behaviours;
using FluentAssertions;
using FluentValidation;
using Xunit;
using ValidationException = eru.Application.Common.Exceptions.ValidationException;

namespace eru.Application.Tests.Common.Behaviours
{
    public class ValidationBehaviourTests
    {
        [Fact]
        public Task DoesValidationBehaviourWorksCorrectlyWhenNoValidators()
        {
            var validators = new IValidator<SampleRequest>[]{};
            var behaviour = new ValidationBehaviour<SampleRequest, SampleResponse>(validators);
            var request = new SampleRequest
            {
                Version = "v2.0",
                IsWorking = true
            };

            Action a = () => behaviour.Handle(request, CancellationToken.None,
                () => Task.FromResult(new SampleResponse {HasWorked = true})).GetAwaiter().GetResult();

            a.Should().NotThrow();
            return Task.CompletedTask;
        }

        [Fact]
        public Task DoesValidationBehaviourWorksCorrectlyWhenAllValidatorsPass()
        {
            var validators = new IValidator<SampleRequest>[]{new SampleValidator()};
            var behaviour = new ValidationBehaviour<SampleRequest, SampleResponse>(validators);
            var request = new SampleRequest
            {
                Version = "v2.0",
                IsWorking = true
            };

            Action a = () => behaviour.Handle(request, CancellationToken.None,
                () => Task.FromResult(new SampleResponse {HasWorked = true})).GetAwaiter().GetResult();

            a.Should().NotThrow();
            return Task.CompletedTask;
        }

        [Fact]
        public Task DoesValidationBehaviourWorksCorrectlyWhenValidatorDoesNotPass()
        {
            var validators = new IValidator<SampleRequest>[]{new SampleValidator(), };
            var behaviour = new ValidationBehaviour<SampleRequest, SampleResponse>(validators);
            var request = new SampleRequest
            {
                IsWorking = true
            };

            Action a = () => behaviour.Handle(request, CancellationToken.None,null).GetAwaiter().GetResult();

            a.Should()
                .Throw<ValidationException>()
                .Which
                .Errors.Should().ContainKey("Version").And.Subject.Values.Should().ContainSingle(x=>x.Contains("'Version' must not be empty."));
            return Task.CompletedTask;
        }
    }
}