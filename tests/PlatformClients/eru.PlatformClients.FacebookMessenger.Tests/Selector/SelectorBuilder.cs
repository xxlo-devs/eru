using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Classes.Queries.GetClasses;
using eru.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using Moq;

namespace eru.PlatformClients.FacebookMessenger.Tests.Selector
{
    internal class SelectorBuilder
    {
        public SelectorBuilder()
        {
            MediatorMock = new Mock<IMediator>();
            TranslatorMock = new Mock<ITranslator<FacebookMessengerPlatformClient>>();
            FakeConfigurationBuilder = new ConfigurationBuilder();
            InjectTranslations();
        }

        public void InjectCultures()
        {
            FakeConfigurationBuilder.AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>("CultureSettings:AvailableCultures:0", "en"),
                    new KeyValuePair<string, string>("CultureSettings:AvailableCultures:1", "pl"),
                    new KeyValuePair<string, string>("CultureSettings:AvailableCultures:2", "bg"),
                    new KeyValuePair<string, string>("CultureSettings:AvailableCultures:3", "hr"),
                    new KeyValuePair<string, string>("CultureSettings:AvailableCultures:4", "cs"),
                    new KeyValuePair<string, string>("CultureSettings:AvailableCultures:5", "da"),
                    new KeyValuePair<string, string>("CultureSettings:AvailableCultures:6", "et"),
                    new KeyValuePair<string, string>("CultureSettings:AvailableCultures:7", "fi"),
                    new KeyValuePair<string, string>("CultureSettings:AvailableCultures:8", "fr"),
                    new KeyValuePair<string, string>("CultureSettings:AvailableCultures:9", "el"),
                    new KeyValuePair<string, string>("CultureSettings:AvailableCultures:10", "es"),
                    new KeyValuePair<string, string>("CultureSettings:AvailableCultures:11", "ga"),
                    new KeyValuePair<string, string>("CultureSettings:AvailableCultures:12", "lt"),
                    new KeyValuePair<string, string>("CultureSettings:AvailableCultures:13", "lv"),
                    new KeyValuePair<string, string>("CultureSettings:AvailableCultures:14", "nl"),
                    new KeyValuePair<string, string>("CultureSettings:AvailableCultures:15", "nb"),
                    new KeyValuePair<string, string>("CultureSettings:AvailableCultures:16", "de"),
                    new KeyValuePair<string, string>("CultureSettings:AvailableCultures:17", "pt"),
                    new KeyValuePair<string, string>("CultureSettings:AvailableCultures:18", "ro"),
                    new KeyValuePair<string, string>("CultureSettings:AvailableCultures:19", "sk"),
                    new KeyValuePair<string, string>("CultureSettings:AvailableCultures:20", "sv"),
                    new KeyValuePair<string, string>("CultureSettings:DefaultCulture", "en"),
                }
            );
        }

        public void InjectYears()
        {
            MediatorMock.Setup(x => x.Send(It.IsAny<GetClassesQuery>(), It.IsAny<CancellationToken>())).Returns(
                (GetClassesQuery query, CancellationToken cancellationToken) =>
                {
                    return Task.FromResult(new[]
                    {
                        new ClassDto {Id = "sample-class-1", Year = 1, Section = "a"},
                        new ClassDto {Id = "sample-class-2", Year = 2, Section = "a"},
                        new ClassDto {Id = "sample-class-3", Year = 3, Section = "a"},
                        new ClassDto {Id = "sample-class-4", Year = 4, Section = "a"},
                        new ClassDto {Id = "sample-class-5", Year = 5, Section = "a"},
                        new ClassDto {Id = "sample-class-6", Year = 6, Section = "a"},
                        new ClassDto {Id = "sample-class-7", Year = 7, Section = "a"},
                        new ClassDto {Id = "sample-class-8", Year = 8, Section = "a"},
                        new ClassDto {Id = "sample-class-9", Year = 9, Section = "a"},
                        new ClassDto {Id = "sample-class-10", Year = 10, Section = "a"},
                        new ClassDto {Id = "sample-class-11", Year = 11, Section = "a"},
                        new ClassDto {Id = "sample-class-12", Year = 12, Section = "a"},
                        new ClassDto {Id = "sample-class-13", Year = 11, Section = "b"},
                        new ClassDto {Id = "sample-class-14", Year = 11, Section = "c"},
                    }.AsEnumerable());
                });
        }

        public void InjectClasses()
        {
            MediatorMock.Setup(x => x.Send(It.IsAny<GetClassesQuery>(), It.IsAny<CancellationToken>())).Returns(
                (GetClassesQuery query, CancellationToken cancellationToken) =>
                {
                    return Task.FromResult(new[]
                    {
                        new ClassDto {Id = "sample-class-1", Year = 1, Section = "a1"},
                        new ClassDto {Id = "sample-class-2", Year = 1, Section = "b1"},
                        new ClassDto {Id = "sample-class-3", Year = 1, Section = "c1"},
                        new ClassDto {Id = "sample-class-4", Year = 1, Section = "d1"},
                        new ClassDto {Id = "sample-class-5", Year = 1, Section = "e1"},
                        new ClassDto {Id = "sample-class-6", Year = 1, Section = "f1"},
                        new ClassDto {Id = "sample-class-7", Year = 1, Section = "a2"},
                        new ClassDto {Id = "sample-class-8", Year = 1, Section = "b2"},
                        new ClassDto {Id = "sample-class-9", Year = 1, Section = "c2"},
                        new ClassDto {Id = "sample-class-10", Year = 1, Section = "d2"},
                        new ClassDto {Id = "sample-class-11", Year = 1, Section = "e2"},
                        new ClassDto {Id = "sample-class-12", Year = 1, Section = "f2"},
                        new ClassDto {Id = "sample-class-12", Year = 2, Section = "2a"},
                        new ClassDto {Id = "sample-class-12", Year = 2, Section = "2b"},
                    }.AsEnumerable());
                });
        }

        private void InjectTranslations()
        {
            
            TranslatorMock.Setup(x => x.TranslateString("cancel-button", "en")).Returns(Task.FromResult("Cancel"));
            TranslatorMock.Setup(x => x.TranslateString("next-page", "en")).Returns(Task.FromResult("->"));
            TranslatorMock.Setup(x => x.TranslateString("previous-page", "en")).Returns(Task.FromResult("<-"));
            TranslatorMock.Setup(x => x.TranslateString("subscribe-button", "en")).Returns(Task.FromResult("Subscribe"));
        }
        
        public Mock<IMediator> MediatorMock { get; set; }
        public Mock<ITranslator<FacebookMessengerPlatformClient>> TranslatorMock { get; set; }
        public IConfigurationBuilder FakeConfigurationBuilder { get; set; }
    }
}