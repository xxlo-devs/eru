using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Classes.Queries.GetClasses;
using eru.Application.Common.Interfaces;
using eru.Application.Subscriptions.Queries.GetSubscriber;
using eru.Domain.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace eru.PlatformClients.FacebookMessenger.Tests
{
    public static class MockBuilder
    {
        public static IApplicationCultures BuildFakeCultures()
        {
            var mock = new Mock<IApplicationCultures>();
            mock
                .Setup(x => x.AvailableCultures)
                .Returns(new[]
                {
                    new Language("English", string.Empty, new CultureInfo("en")),
                    new Language("Polish", string.Empty, new CultureInfo("pl"))
                });

            mock
                .Setup(x => x.DefaultCulture)
                .Returns(new Language("English", string.Empty, new CultureInfo("en")));

            mock.Setup(x => x.FindCulture(It.IsAny<string>())).Returns((string name) => name switch
            {
                "en" => new Language("English", string.Empty, new CultureInfo("en")),
                "pl" => new Language("Polish", string.Empty, new CultureInfo("pl")),
                _ => null
            });

            return mock.Object;
        }

        public static IConfiguration BuildFakeConfiguration()
        {
            return new ConfigurationBuilder().AddInMemoryCollection(new[]
            {
                new KeyValuePair<string, string>("PlatformClients:FacebookMessenger:VerifyToken", "sample-verify-token"),
                new KeyValuePair<string, string>("PlatformClients:FacebookMessenger:AccessToken", "sample-access-token")
            }).Build();
        }
        
        public static Mock<IMediator> BuildMediatorMock()
        {
            var mediatorMock = new Mock<IMediator>();

            mediatorMock.Setup(x => x.Send(It.IsAny<GetSubscriberQuery>(), It.IsAny<CancellationToken>())).Returns(
                (GetSubscriberQuery query, CancellationToken cancellationToken) =>
                {
                    if (query.Id == "sample-subscriber" && query.Platform == FacebookMessengerPlatformClient.PId)
                        return Task.FromResult(new Subscriber
                        {
                            Id = "sample-subscriber", Platform = FacebookMessengerPlatformClient.PId,
                            PreferredLanguage = "en", Class = "sample-class"
                        });
                    else
                        return Task.FromResult<Subscriber>(null);
                });
            
            mediatorMock.Setup(x => x.Send(It.IsAny<GetClassesQuery>(), It.IsAny<CancellationToken>())).Returns(
                Task.FromResult(new[]
                {
                    new ClassDto {Id = "sample-class-1a", Year = 1, Section = "a"},
                    new ClassDto {Id = "sample-class-1b", Year = 1, Section = "b"},
                    new ClassDto {Id = "sample-class-1c", Year = 1, Section = "c"},
                    new ClassDto {Id = "sample-class-1d", Year = 1, Section = "d"},
                    new ClassDto {Id = "sample-class-1e", Year = 1, Section = "e"},
                    new ClassDto {Id = "sample-class-1f", Year = 1, Section = "f"},
                    new ClassDto {Id = "sample-class-1g", Year = 1, Section = "g"},
                    new ClassDto {Id = "sample-class-1h", Year = 1, Section = "h"},
                    new ClassDto {Id = "sample-class-1i", Year = 1, Section = "i"},
                    new ClassDto {Id = "sample-class-1j", Year = 1, Section = "j"},
                    new ClassDto {Id = "sample-class-1k", Year = 1, Section = "k"},
                    new ClassDto {Id = "sample-class-1l", Year = 1, Section = "l"},
                    new ClassDto {Id = "sample-class-2a", Year = 2, Section = "a"},
                    new ClassDto {Id = "sample-class-3a", Year = 3, Section = "a"},
                    new ClassDto {Id = "sample-class-4a", Year = 4, Section = "a"},
                    new ClassDto {Id = "sample-class-5a", Year = 5, Section = "a"},
                    new ClassDto {Id = "sample-class-6a", Year = 6, Section = "a"},
                    new ClassDto {Id = "sample-class-7a", Year = 7, Section = "a"},
                    new ClassDto {Id = "sample-class-8a", Year = 8, Section = "a"},
                    new ClassDto {Id = "sample-class-9a", Year = 9, Section = "a"},
                    new ClassDto {Id = "sample-class-10a", Year = 10, Section = "a"},
                    new ClassDto {Id = "sample-class-11a", Year = 11, Section = "a"},
                    new ClassDto {Id = "sample-class-12a", Year = 12, Section = "a"}
                }.AsEnumerable()));

            return mediatorMock;
        }
              
        public static ITranslator<FacebookMessengerPlatformClient> BuildFakeTranslator()
        {
            var translatorMock = new Mock<ITranslator<FacebookMessengerPlatformClient>>();
            
            translatorMock
                .Setup(x => x.TranslateString("cancel-button", It.Is<Language>(y => y.Culture.Name == "en")))
                .Returns(Task.FromResult("cancel-button-text"));
            translatorMock
                .Setup(x => x.TranslateString("next-page", It.Is<Language>(y => y.Culture.Name == "en")))
                .Returns(Task.FromResult("->"));
            translatorMock
                .Setup(x => x.TranslateString("previous-page", It.Is<Language>(y => y.Culture.Name == "en")))
                .Returns(Task.FromResult("<-"));
            translatorMock
                .Setup(x => x.TranslateString("subscribe-button", It.Is<Language>(y => y.Culture.Name == "en")))
                .Returns(Task.FromResult("subscribe-button-text"));
            
            translatorMock
                .Setup(x => x.TranslateString("greeting", It.Is<Language>(y => y.Culture.Name == "en")))
                .Returns(Task.FromResult("greeting-text"));
            translatorMock
                .Setup(x => x.TranslateString("year-selection", It.Is<Language>(y => y.Culture.Name == "en")))
                .Returns(Task.FromResult("year-selection-text"));
            translatorMock
                .Setup(x => x.TranslateString("class-selection", It.Is<Language>(y => y.Culture.Name == "en")))
                .Returns(Task.FromResult("class-selection-text"));
            
            translatorMock
                .Setup(x => x.TranslateString("confirmation", It.Is<Language>(y => y.Culture.Name == "en")))
                .Returns(Task.FromResult("confirmation-text"));
            translatorMock
                .Setup(x => x.TranslateString("congratulations", It.Is<Language>(y => y.Culture.Name == "en")))
                .Returns(Task.FromResult("congratulations-text"));
            
            translatorMock
                .Setup(x => x.TranslateString("subscription-cancelled", It.Is<Language>(y => y.Culture.Name == "en")))
                .Returns(Task.FromResult("subscription-cancelled-text"));
            translatorMock
                .Setup(x => x.TranslateString("unsupported-command", It.Is<Language>(y => y.Culture.Name == "en")))
                .Returns(Task.FromResult("unsupported-command-text"));

            translatorMock
                .Setup(x => x.TranslateString("new-substitutions", It.Is<Language>(y => y.Culture.Name == "en")))
                .Returns(Task.FromResult("new-substitutions-text"));
            translatorMock
                .Setup(x => x.TranslateString("substitution", It.Is<Language>(y => y.Culture.Name == "en")))
                .Returns(Task.FromResult("SUBSTITUTION | {0} | {1} | {2} | {3} | {4} | {5}"));
            translatorMock.
                Setup(x => x.TranslateString("cancellation", It.Is<Language>(y => y.Culture.Name == "en")))
                .Returns(Task.FromResult("CANCELLATION | {0} | {1} | {2} | {3} | {4}"));
            translatorMock
                .Setup(x => x.TranslateString("closing-substitutions", It.Is<Language>(y => y.Culture.Name == "en")))
                .Returns(Task.FromResult("closing-substitutions-text"));
            
            return translatorMock.Object;
        }

        public static ILogger<T> BuildFakeLogger<T>()
        {
            return new Mock<ILogger<T>>().Object;
        }
    }
}