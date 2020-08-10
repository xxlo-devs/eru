using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Substitutions.Commands.SendNotificationsAboutSubstitutions;
using eru.Domain.Entity;
using FluentAssertions;
using Xunit;

namespace eru.Application.Tests.Substitutions.Commands
{
    public class SendNotificationsAboutSubstitutionsCommandTests
    {
        [Fact]
        public void DoesToStringOnRequestWorksCorrectly()
        {
            var request = new SendNotificationsAboutSubstitutionsCommand()
            {
                IpAddress = "1.1.1.1",
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

            var output = request.ToString();

            output.Should().Be("Key( sample-key ) - Ip( 1.1.1.1 )");
        }
        [Fact]
        public async Task DoesHandleWorksCorrectly()
        {
            var fakeClient = new FakeBackgroundJobClient();
            var fakeMessageService = new FakeMessageService();
            var handler = new SendNotificationsAboutSubstitutionsCommandHandler(fakeClient, new []{fakeMessageService});
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

            await handler.Handle(request, CancellationToken.None);

            fakeClient.EnqueuedJobs.Should().ContainSingle("() => value(eru.Application.Substitutions.Commands.SendNotificationsAboutSubstitutions.SendNotificationsAboutSubstitutionsCommandHandler).PropagateSubstitutionsNotifications(value(eru.Application.Substitutions.Commands.SendNotificationsAboutSubstitutions.SendNotificationsAboutSubstitutionsCommandHandler+<>c__DisplayClass3_0).request.SubstitutionsPlan)");
        }

        [Fact]
        public async Task DoesPropagateSubstitutionsNotificationsWorksCorrectly()
        {
            var fakeClient = new FakeBackgroundJobClient();
            var fakeMessageService = new FakeMessageService();
            var handler = new SendNotificationsAboutSubstitutionsCommandHandler(fakeClient, new []{fakeMessageService});
            var substitutionsPlan = new SubstitutionsPlan
            {
                Substitutions = new[]
                {
                    new Substitution
                    {
                        Cancelled = true, Classes = new[]
                        {
                            new Class("IB1")
                        },
                        Groups = "Cała klasa", Lesson = 1, Subject = "j. Polski", Teacher = "Sample Teacher 1"
                    },
                    new Substitution
                    {
                        Cancelled = true, Classes = new[]
                        {
                            new Class("IIa2")
                        },
                        Groups = "Cała klasa", Lesson = 3, Subject = "matematyka", Teacher = "Sample Teacher 2"
                    },
                    new Substitution
                    {
                        Cancelled = true, Classes = new[]
                        {
                            new Class("IB1")
                        },
                        Groups = "Cała klasa", Lesson = 5, Subject = "informatyka", Teacher = "Sample Teacher 3"
                    }
                }
            };

            await handler.PropagateSubstitutionsNotifications(substitutionsPlan);

            fakeClient.EnqueuedJobs.Should().HaveCount(2).And.Contain(Enumerable.Repeat(
                "() => value(eru.Application.Substitutions.Commands.SendNotificationsAboutSubstitutions.SendNotificationsAboutSubstitutionsCommandHandler).SendSubstitutionsNotificationsToClass(value(eru.Application.Substitutions.Commands.SendNotificationsAboutSubstitutions.SendNotificationsAboutSubstitutionsCommandHandler+<>c__DisplayClass4_1).CS$<>8__locals1.substitutions.ToArray(), value(eru.Application.Substitutions.Commands.SendNotificationsAboutSubstitutions.SendNotificationsAboutSubstitutionsCommandHandler+<>c__DisplayClass4_1).CS$<>8__locals1.class, value(eru.Application.Substitutions.Commands.SendNotificationsAboutSubstitutions.SendNotificationsAboutSubstitutionsCommandHandler+<>c__DisplayClass4_1).messageService)",
                2));
        }

        [Fact]
        public async Task DoesSendSubstitutionsNotificationsToClassWorksCorrectly()
        {
            var fakeClient = new FakeBackgroundJobClient();
            var fakeMessageService = new FakeMessageService();
            var handler = new SendNotificationsAboutSubstitutionsCommandHandler(fakeClient, new []{fakeMessageService});

            await handler.SendSubstitutionsNotificationsToClass(new[]
            {
                new Substitution
                {
                    Cancelled = true, Classes = new[]
                    {
                        new Class("IB1")
                    },
                    Groups = "Cała klasa", Lesson = 5, Subject = "informatyka", Teacher = "Sample Teacher 3"
                },
                new Substitution
                {
                    Cancelled = true, Classes = new[]
                    {
                        new Class("IB1")
                    },
                    Groups = "Cała klasa", Lesson = 1, Subject = "j. Polski", Teacher = "Sample Teacher 1"
                },
            }, new Class("IB1"), fakeMessageService);

            fakeClient.EnqueuedJobs.Should()
                .HaveCount(fakeMessageService.StudentsCount * 2);
            fakeClient.EnqueuedJobs.All(x =>
                    x ==
                    "() => value(eru.Application.Substitutions.Commands.SendNotificationsAboutSubstitutions.SendNotificationsAboutSubstitutionsCommandHandler+<>c__DisplayClass5_2).CS$<>8__locals2.CS$<>8__locals1.messageService.SendSubstitutionNotification(value(eru.Application.Substitutions.Commands.SendNotificationsAboutSubstitutions.SendNotificationsAboutSubstitutionsCommandHandler+<>c__DisplayClass5_2).CS$<>8__locals2.studentId, value(eru.Application.Substitutions.Commands.SendNotificationsAboutSubstitutions.SendNotificationsAboutSubstitutionsCommandHandler+<>c__DisplayClass5_2).substitution)")
                .Should().BeTrue();
        }
    }
}