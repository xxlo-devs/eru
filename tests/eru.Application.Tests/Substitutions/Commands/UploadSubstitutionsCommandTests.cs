using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.Configuration;
using Castle.DynamicProxy;
using eru.Application.Common.Interfaces;
using eru.Application.Substitutions.Commands;
using eru.Application.Substitutions.Notifications;
using eru.Domain.Entity;
using FluentAssertions;
using MediatR;
using Moq;
using Xunit;

namespace eru.Application.Tests.Substitutions.Commands
{
    public class UploadSubstitutionsCommandTests
    {
        [Fact]
        public async Task IsToStringOnRequestWorkingCorrectly()
        {
            var request = new UploadSubstitutionsCommand
            {
                IpAddress = "8.8.8.8",
                Key = "sample-key",
                SubstitutionsPlan = new SubstitutionsPlan
                {
                    Date = new DateTime(2010, 1,1),
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
                IpAddress = "8.8.8.8",
                Key = "sample-key",
                SubstitutionsPlan = new SubstitutionsPlan
                {
                    Date = new DateTime(2010, 1,1),
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
                        new Substitution
                        {
                            Cancelled = true,
                            Classes = new []{new Class("IB1"), new Class("IIC") },
                            Groups = "Cała klasa",
                            Lesson = 2,
                            Room = "304",
                            Subject = "matematyka",
                            Teacher = "sample teacher 3"
                        }, 
                    }
                }
            };
            var mediator = new Mock<IMediator>();
            var backgroundExecutor = new Mock<IBackgroundExecutor>();
            var handler = new UploadSubstitutionsCommandHandler(mediator.Object, backgroundExecutor.Object);

            await handler.Handle(request, CancellationToken.None);
            
            backgroundExecutor.Verify(x=>x.Enqueue(It.IsAny<Expression<Func<Task>>>()), Times.Exactly(2));
        }

        [Fact]
        public async Task DoesValidatorAllowValidRequest()
        {
            var config = new FakeConfiguration();
            var validator = new UploadSubstitutionsCommandValidator(config);

            var request = new UploadSubstitutionsCommand
            {
                IpAddress = "198.51.100.1",
                Key = "D6FFE16E-BF9D-4172-9869-F7EC182172A7",
                SubstitutionsPlan = new SubstitutionsPlan
                {
                    Date = DateTime.UtcNow.Date,
                    Substitutions = new List<Substitution>
                    {
                        new Substitution
                        {
                            Cancelled = false,
                            Classes = new[] {new Class("Ia"), new Class("IIIc")},
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
            var config = new FakeConfiguration();
            var validator = new UploadSubstitutionsCommandValidator(config);

            var request = new UploadSubstitutionsCommand
            {
                IpAddress = "198-51-100-1",
                Key = "D6FFE16E-BF9D-4172-9869-F7EC182172A7",
                SubstitutionsPlan = new SubstitutionsPlan
                {
                    Date = DateTime.UtcNow.Date,
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
            result.Errors.Should().HaveCount(1);
        }

        [Fact]
        public async Task DoesValidatorPreventUploadWithInvalidKey()
        {
            var config = new FakeConfiguration();
            var validator = new UploadSubstitutionsCommandValidator(config);

            var request = new UploadSubstitutionsCommand
            {
                IpAddress = "198.51.100.1",
                Key = "CF502808-4BAC-4D62-86DF-04BF0E040953",
                SubstitutionsPlan = new SubstitutionsPlan
                {
                    Date = DateTime.UtcNow.Date,
                    Substitutions = new List<Substitution>
                    {
                        new Substitution
                        {
                            Cancelled = false,
                            Classes = new[] {new Class("Ia"), new Class("IIIc")},
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
            result.Errors.Should().HaveCount(1);
        }

        [Fact]
        public async Task DoesValidatorPreventUploadOfInvalidSubstitutionsPlan()
        {
            var config = new FakeConfiguration();
            var validator = new UploadSubstitutionsCommandValidator(config);

            var request = new UploadSubstitutionsCommand
            {
                IpAddress = "198.51.100.1",
                Key = "D6FFE16E-BF9D-4172-9869-F7EC182172A7",
                SubstitutionsPlan = new SubstitutionsPlan
                {
                    Date = DateTime.UtcNow.Date,
                    Substitutions = new List<Substitution>()
                }
            };

            var result = await validator.ValidateAsync(request, CancellationToken.None);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
        }

    }
}