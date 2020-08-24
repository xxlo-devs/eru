using eru.Application.Common.Interfaces;
using eru.Application.Substitutions.Commands;
using eru.Domain.Entity;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Xunit;

namespace eru.Application.Tests.Substitutions.Commands
{
    public class UploadSubstitutionsCommandTests
    {
        public UploadSubstitutionsCommandTests()
        {
            ValidatorOptions.Global.LanguageManager.Enabled = false;
        }
        
        [Fact]
        public void IsToStringOnRequestWorkingCorrectly()
        {
            var request = new UploadSubstitutionsCommand
            {
                IpAddress = MockData.CorrectIpAddress,
                Key = MockData.CorrectUploadKey,
                SubstitutionsPlan = new SubstitutionsPlan
                {
                    Date = MockData.CorrectDate,
                    Substitutions = new []
                    {
                        new Substitution
                        {
                            Cancelled = false,
                            Classes = new []{new Class("IB1"), new Class("IIIc") },
                            Groups = "Cała klasa",
                            Lesson = 3,
                            Room = "204",
                            Subject = "j. Polski",
                            Substituting = "sample teacher 1",
                            Teacher = "sample teacher 2"
                        }, 
                    }
                }
            };

            var output = request.ToString();

            output.Should().Contain(request.IpAddress).And.Contain(request.Key);
        }

        [Fact]
        public async Task DoesUploadWorksCorrectly()
        {
            var request = new UploadSubstitutionsCommand
            {
                IpAddress = "198.51.100.1",
                Key = "sample-key",
                SubstitutionsPlan = new SubstitutionsPlan
                {
                    Date = MockData.CorrectDate,
                    Substitutions = new []
                    {
                        new Substitution
                        {
                            Cancelled = false,
                            Classes = new []{new Class("I a"), new Class("III c") },
                            Groups = "Cała klasa",
                            Lesson = 3,
                            Room = "204",
                            Subject = "j. Polski",
                            Substituting = "sample teacher 1",
                            Teacher = "sample teacher 2"
                        }, 
                        new Substitution
                        {
                            Cancelled = true,
                            Classes = new []{new Class("I a"), new Class("II b") },
                            Groups = "Cała klasa",
                            Lesson = 2,
                            Room = "304",
                            Subject = "matematyka",
                            Teacher = "sample teacher 3"
                        }, 
                    }
                }
            };
            var fakeDbContext = new FakeDbContext();
            var hangfireWrapper = new Mock<IHangfireWrapper>();
            var backgroundJobClient = new Mock<IBackgroundJobClient>();
            hangfireWrapper.Setup(x => x.BackgroundJobClient).Returns(backgroundJobClient.Object);
            var sampleClient = new Mock<IPlatformClient>();
            sampleClient.Setup(x => x.PlatformId).Returns("DebugMessageService");
            var handler = new UploadSubstitutionsCommandHandler(fakeDbContext, new []{sampleClient.Object}, hangfireWrapper.Object);

            await handler.Handle(request, CancellationToken.None);
            
            backgroundJobClient.Verify(x=>x.Create(It.Is<Job>(job => job.Method.Name == "SendMessage"), It.IsAny<IState>()));
        }

        [Fact]
        public async Task DoesValidatorAllowValidRequest()
        {
            var config = new ConfigurationBuilder().AddInMemoryCollection(new[] { new KeyValuePair<string, string>("UploadKey", "sample-key") }).Build();
            var validator = new UploadSubstitutionsCommandValidator(config, new FakeDbContext());

            var request = new UploadSubstitutionsCommand
            {
                IpAddress = MockData.CorrectIpAddress,
                Key = MockData.CorrectUploadKey,
                SubstitutionsPlan = new SubstitutionsPlan
                {
                    Date = MockData.CorrectDate,
                    Substitutions = new List<Substitution>
                    {
                        new Substitution
                        {
                            Cancelled = false,
                            Classes = new[] {new Class("I a"), new Class("III c")},
                            Groups = "Cała klasa",
                            Lesson = 3,
                            Room = "204",
                            Subject = "j. Polski",
                            Substituting = "sample teacher 1",
                            Teacher = "sample teacher 2"
                        }
                    }
                }
            };

            var result = await validator.ValidateAsync(request, CancellationToken.None);

            result.IsValid.Should().BeTrue();
            result.Errors.Should().HaveCount(0);
        }

        [Fact]
        public async Task DoesValidatorPreventUploadWithInvalidIPAddress()
        {
            var config = new ConfigurationBuilder().AddInMemoryCollection(new[] { new KeyValuePair<string, string>("UploadKey", "sample-key") }).Build();
            var validator = new UploadSubstitutionsCommandValidator(config, new FakeDbContext());

            var request = new UploadSubstitutionsCommand
            {
                IpAddress = "198-51-100-1",
                Key = MockData.CorrectUploadKey,
                SubstitutionsPlan = new SubstitutionsPlan
                {
                    Date = MockData.CorrectDate,
                    Substitutions = new List<Substitution>
                    {
                        new Substitution
                        {
                            Cancelled = false,
                            Classes = new[] {new Class("I a"), new Class("III c")},
                            Groups = "Cała klasa",
                            Lesson = 3,
                            Room = "204",
                            Subject = "j. Polski",
                            Substituting = "sample teacher 1",
                            Teacher = "sample teacher 2"
                        }
                    }
                }
            };

            var result = await validator.ValidateAsync(request, CancellationToken.None);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1).And.ContainSingle(x=>x.ErrorCode == "PredicateValidator" && x.ErrorMessage == "IpAddress must be a valid ip address.");
        }

        [Fact]
        public async Task DoesValidatorPreventUploadWithInvalidKey()
        {
            var config = new ConfigurationBuilder().AddInMemoryCollection(new[] { new KeyValuePair<string, string>("UploadKey", "sample-key") }).Build();
            var validator = new UploadSubstitutionsCommandValidator(config, new FakeDbContext());

            var request = new UploadSubstitutionsCommand
            {
                IpAddress = MockData.CorrectIpAddress,
                Key = "invalid-key",
                SubstitutionsPlan = new SubstitutionsPlan
                {
                    Date = MockData.CorrectDate,
                    Substitutions = new List<Substitution>
                    {
                        new Substitution
                        {
                            Cancelled = false,
                            Classes = new[] {new Class("I a"), new Class("III c")},
                            Groups = "Cała klasa",
                            Lesson = 3,
                            Room = "204",
                            Subject = "j. Polski",
                            Substituting = "sample teacher 1",
                            Teacher = "sample teacher 2"
                        }
                    }
                }
            };

            var result = await validator.ValidateAsync(request, CancellationToken.None);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1).And.ContainSingle(x=>x.ErrorCode == "PredicateValidator" && x.ErrorMessage == "Key must be a correct key from configuration.");
        }

        [Fact]
        public async Task DoesValidatorPreventUploadOfInvalidSubstitutionsPlan()
        {
            var config = new ConfigurationBuilder().AddInMemoryCollection(new[] { new KeyValuePair<string, string>("UploadKey", "sample-key") }).Build();
            var validator = new UploadSubstitutionsCommandValidator(config, new FakeDbContext());

            var request = new UploadSubstitutionsCommand
            {
                IpAddress = MockData.CorrectIpAddress,
                Key = MockData.CorrectUploadKey,
                SubstitutionsPlan = new SubstitutionsPlan
                {
                    Date = MockData.CorrectDate,
                    Substitutions = new List<Substitution>()
                }
            };

            var result = await validator.ValidateAsync(request, CancellationToken.None);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1).And.ContainSingle(x=>x.ErrorCode == "PredicateValidator" && x.ErrorMessage == "SubstitutionsPlan must have at least one substitution.");
        }

    }
}