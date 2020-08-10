using System.Collections.Generic;
using System.Threading.Tasks;
using eru.Application.Substitutions.Commands.SendNotificationsAboutSubstitutions;
using eru.Domain.Entity;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace eru.Application.Tests.Substitutions.Commands
{
    public class SendNotificationsAboutSubstitutionsCommandValidatorTests
    {
        [Fact]
        public async Task DoesValidatorPreventFromSendingNotificationsWhenBadIpAddress()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>("UploadKey", "sample-key")
                })
                .Build();
            var validator = new SendNotificationsAboutSubstitutionsCommandValidator(configuration);
            var request = new SendNotificationsAboutSubstitutionsCommand
            {
                IpAddress = "not-ip-address",
                Key = "sample-key",
                SubstitutionsPlan = new SubstitutionsPlan
                {
                    Substitutions = new[]
                    {
                        new Substitution
                        {
                            Cancelled = true, Classes = new[]
                            {
                                new Class("IB1")
                            },
                            Groups = "Cała klasa", Lesson = 1, Subject = "j. Polski", Teacher = "Sample Teacher"
                        }
                    }
                }
            };

            var result = await validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(x => x.ErrorMessage == "'Ip Address' is not in the correct format.");
        }

        [Fact]
        public async Task DoesValidatorPreventFromSendingNotificationsWhenBadKey()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>("UploadKey", "sample-key")
                })
                .Build();
            var validator = new SendNotificationsAboutSubstitutionsCommandValidator(configuration);
            var request = new SendNotificationsAboutSubstitutionsCommand
            {
                IpAddress = "8.8.8.8",
                Key = "bad-key",
                SubstitutionsPlan = new SubstitutionsPlan
                {
                    Substitutions = new[]
                    {
                        new Substitution
                        {
                            Cancelled = true, Classes = new[]
                            {
                                new Class("IB1")
                            },
                            Groups = "Cała klasa", Lesson = 1, Subject = "j. Polski", Teacher = "Sample Teacher"
                        }
                    }
                }
            };

            var result = await validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should()
                .ContainSingle(x => x.ErrorMessage == "The specified condition was not met for 'Key'.");
        }

        [Fact]
        public async Task DoesValidatorPreventFromSendingNotificationsWhenBadSubstitutionsPlan()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>("UploadKey", "sample-key")
                })
                .Build();
            var validator = new SendNotificationsAboutSubstitutionsCommandValidator(configuration);
            var request = new SendNotificationsAboutSubstitutionsCommand
            {
                IpAddress = "8.8.8.8",
                Key = "sample-key",
                SubstitutionsPlan = new SubstitutionsPlan()
            };

            var result = await validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should()
                .ContainSingle(x =>
                    x.ErrorMessage == "The specified condition was not met for 'Substitutions Plan'.");
        }

        [Fact]
        public async Task DoesValidatorPreventFromSendingNotificationsWhenNoIpAddress()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>("UploadKey", "sample-key")
                })
                .Build();
            var validator = new SendNotificationsAboutSubstitutionsCommandValidator(configuration);
            var request = new SendNotificationsAboutSubstitutionsCommand
            {
                Key = "sample-key",
                SubstitutionsPlan = new SubstitutionsPlan
                {
                    Substitutions = new[]
                    {
                        new Substitution
                        {
                            Cancelled = true, Classes = new[]
                            {
                                new Class("IB1")
                            },
                            Groups = "Cała klasa", Lesson = 1, Subject = "j. Polski", Teacher = "Sample Teacher"
                        }
                    }
                }
            };

            var result = await validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(x => x.ErrorMessage == "'Ip Address' must not be empty.");
        }

        [Fact]
        public async Task DoesValidatorPreventFromSendingNotificationsWhenNoKey()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>("UploadKey", "sample-key")
                })
                .Build();
            var validator = new SendNotificationsAboutSubstitutionsCommandValidator(configuration);
            var request = new SendNotificationsAboutSubstitutionsCommand
            {
                IpAddress = "8.8.8.8",
                SubstitutionsPlan = new SubstitutionsPlan
                {
                    Substitutions = new[]
                    {
                        new Substitution
                        {
                            Cancelled = true, Classes = new[]
                            {
                                new Class("IB1")
                            },
                            Groups = "Cała klasa", Lesson = 1, Subject = "j. Polski", Teacher = "Sample Teacher"
                        }
                    }
                }
            };

            var result = await validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should()
                .Contain(x => x.ErrorMessage == "'Key' must not be empty.")
                .And
                .Contain(x => x.ErrorMessage == "The specified condition was not met for 'Key'.")
                .And
                .HaveCount(2);
        }

        [Fact]
        public async Task DoesValidatorPreventFromSendingNotificationsWhenNoSubstitutionsPlan()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>("UploadKey", "sample-key")
                })
                .Build();
            var validator = new SendNotificationsAboutSubstitutionsCommandValidator(configuration);
            var request = new SendNotificationsAboutSubstitutionsCommand
            {
                IpAddress = "8.8.8.8",
                Key = "sample-key"
            };

            var result = await validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should()
                .Contain(x => x.ErrorMessage == "'Substitutions Plan' must not be empty.")
                .And
                .Contain(x => x.ErrorMessage == "The specified condition was not met for 'Substitutions Plan'.")
                .And
                .HaveCount(2);
        }

        [Fact]
        public async Task DoesValidatorAllowCorrectRequest()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>("UploadKey", "sample-key")
                })
                .Build();
            var validator = new SendNotificationsAboutSubstitutionsCommandValidator(configuration);
            var request = new SendNotificationsAboutSubstitutionsCommand
            {
                IpAddress = "8.8.8.8",
                Key = "sample-key",
                SubstitutionsPlan = new SubstitutionsPlan
                {
                    Substitutions = new[]
                    {
                        new Substitution
                        {
                            Cancelled = true, Classes = new[]
                            {
                                new Class("IB1")
                            },
                            Groups = "Cała klasa", Lesson = 1, Subject = "j. Polski", Teacher = "Sample Teacher"
                        }
                    }
                }
            };

            var result = await validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }
    }
}