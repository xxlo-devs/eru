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
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Substitutions.Commands.UploadSubstitutions;
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
                Substitutions = new []
                {
                    new SubstitutionDto
                    {
                        Cancelled = false,
                        ClassesNames = new []{"IB1", "IIIc" },
                        Groups = "Cała klasa",
                        Lesson = 3,
                        Room = "204",
                        Subject = "j. Polski",
                        Substituting = "sample teacher 1",
                        Absent = "sample teacher 2"
                    }, 
                },
                SubstitutionsDate = MockData.CorrectDate,
                UploadDateTime = MockData.CorrectDate
            };

            var output = request.ToString();

            output.Should().Contain(request.IpAddress).And.Contain(request.Key);
        }

        [Fact]
        public async Task DoesUploadWorksCorrectly()
        {
            var request = new UploadSubstitutionsCommand
            {
                IpAddress = MockData.CorrectIpAddress,
                Key = MockData.CorrectUploadKey,
                Substitutions = new []
                {
                    new SubstitutionDto
                    {
                        Cancelled = false,
                        ClassesNames = new []{"I a", "III c" },
                        Groups = "Cała klasa",
                        Lesson = 3,
                        Room = "204",
                        Subject = "j. Polski",
                        Substituting = "sample teacher 1",
                        Absent = "sample teacher 2"
                    }, 
                    new SubstitutionDto
                    {
                        Cancelled = true,
                        ClassesNames = new []{"I a", "II b" },
                        Groups = "Cała klasa",
                        Lesson = 2,
                        Room = "304",
                        Subject = "matematyka",
                        Absent = "sample teacher 3"
                    }, 
                },
                UploadDateTime = MockData.CorrectDate,
                SubstitutionsDate = MockData.CorrectDate
            };
            var fakeDbContext = new FakeDbContext();
            var backgroundJobClient = new Mock<IBackgroundJobClient>();
            var sampleClient = new Mock<IPlatformClient>();
            sampleClient.Setup(x => x.PlatformId).Returns("DebugMessageService");

            var classParser = new Mock<IClassesParser>();
            classParser.Setup(classparser => classparser.Parse(It.IsAny<IEnumerable<string>>())).Returns(
                (IEnumerable<string> x) =>
                {
                    if(x.Contains("I a") & x.Contains("III c")) return new[] {new Class(1, "a"), new Class(3, "c")};
                    else return new[] {new Class(1, "a"), new Class(2, "b")};
                });
            
            var handler = new UploadSubstitutionsCommandHandler(fakeDbContext, new []{sampleClient.Object}, backgroundJobClient.Object, classParser.Object);

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
                Substitutions = new []
                {
                    new SubstitutionDto
                    {
                        Cancelled = false,
                        ClassesNames = new[] {"I a", "III c"},
                        Groups = "Cała klasa",
                        Lesson = 3,
                        Room = "204",
                        Subject = "j. Polski",
                        Substituting = "sample teacher 1",
                        Absent = "sample teacher 2"
                    }
                },
                UploadDateTime = MockData.CorrectDate,
                SubstitutionsDate = MockData.CorrectDate
            };

            var result = await validator.ValidateAsync(request, CancellationToken.None);

            result.IsValid.Should().BeTrue();
            result.Errors.Should().HaveCount(0);
        }

        [Fact]
        public async Task DoesValidatorPreventUploadWithInvalidIPAddress()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>("UploadKey", "sample-key")
                }).Build();
            var validator = new UploadSubstitutionsCommandValidator(config, new FakeDbContext());

            var request = new UploadSubstitutionsCommand
            {
                IpAddress = "198-51-100-1",
                Key = MockData.CorrectUploadKey,
                UploadDateTime = MockData.CorrectDate,
                SubstitutionsDate = MockData.CorrectDate,
                Substitutions = new []
                {
                    new SubstitutionDto
                    {
                        Cancelled = false,
                        ClassesNames = new[] {"I a", "III c"},
                        Groups = "Cała klasa",
                        Lesson = 3,
                        Room = "204",
                        Subject = "j. Polski",
                        Substituting = "sample teacher 1",
                        Absent = "sample teacher 2"
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
                SubstitutionsDate = MockData.CorrectDate,
                UploadDateTime = MockData.CorrectDate,
                Key = "invalid-key",
                Substitutions = new []
                {
                    new SubstitutionDto
                    {
                        Cancelled = false,
                        ClassesNames = new[] {"I a", "III c"},
                        Groups = "Cała klasa",
                        Lesson = 3,
                        Room = "204",
                        Subject = "j. Polski",
                        Substituting = "sample teacher 1",
                        Absent = "sample teacher 2"
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
                SubstitutionsDate = MockData.CorrectDate,
                UploadDateTime = MockData.CorrectDate,
                Substitutions = ArraySegment<SubstitutionDto>.Empty
            };

            var result = await validator.ValidateAsync(request, CancellationToken.None);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1).And.ContainSingle(x=>x.ErrorCode == "NotEmptyValidator" && x.ErrorMessage == "Substitutions cannot be empty.");
        }

        [Fact]
        public async Task DoesNewlyCreatedClassesAreNotDuplicated()
        {
            var request = new UploadSubstitutionsCommand
            {
                IpAddress = MockData.CorrectIpAddress,
                Key = MockData.CorrectUploadKey,
                Substitutions = new []
                {
                    new SubstitutionDto
                    {
                        Cancelled = false,
                        ClassesNames = new []{"II a", "III c" },
                        Groups = "Cała klasa",
                        Lesson = 3,
                        Room = "204",
                        Subject = "j. Polski",
                        Substituting = "sample teacher 1",
                        Absent = "sample teacher 2"
                    }, 
                    new SubstitutionDto
                    {
                        Cancelled = true,
                        ClassesNames = new []{"II a", "II b" },
                        Groups = "Cała klasa",
                        Lesson = 2,
                        Room = "304",
                        Subject = "matematyka",
                        Absent = "sample teacher 3"
                    }, 
                },
                UploadDateTime = MockData.CorrectDate,
                SubstitutionsDate = MockData.CorrectDate
            };
            var fakeDbContext = new FakeDbContext();
            var backgroundJobClient = new Mock<IBackgroundJobClient>();
            var sampleClient = new Mock<IPlatformClient>();
            sampleClient.Setup(x => x.PlatformId).Returns("DebugMessageService");

            var classParser = new Mock<IClassesParser>();
            classParser.Setup(classesParser => classesParser.Parse(It.IsAny<IEnumerable<string>>())).Returns(
                (IEnumerable<string> x) =>
                {
                    var array = x.ToArray();
                    if(array.Contains("II a") & array.Contains("III c"))
                        return new[] {new Class(2, "a"), new Class(3, "c")};
                    else 
                        return new[] {new Class(2, "a"), new Class(2, "b")};
                });
            
            var handler = new UploadSubstitutionsCommandHandler(fakeDbContext, new []{sampleClient.Object}, backgroundJobClient.Object, classParser.Object);

            await handler.Handle(request, CancellationToken.None);

            fakeDbContext.Classes.Should()
                .HaveCount(4)
                .And
                .Contain(new[]
                {
                    new Class(1, "a"), 
                    new Class(2, "a"),
                    new Class(3, "c"),
                    new Class(2, "b"),
                });
        }

    }
}