using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Classes.Queries.GetClasses;
using eru.Infrastructure.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUser.Paging;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.DbContext;
using eru.Infrastructure.PlatformClients.FacebookMessenger.RegistrationDb.Enums;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Selector;
using eru.Infrastructure.PlatformClients.FacebookMessenger.SendAPIClient;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace eru.Infrastructure.Tests.PlatformClients.FacebookMessenger.MessageHandlers.RegisteringUserTests
{
    internal class PagingHandlerBuilder
    {
        public PagingHandlerBuilder()
        {
            SetupMediator();
            SetupConfig();
            FakeDbContext = new FakeRegistrationDb();
            Selector = new Infrastructure.PlatformClients.FacebookMessenger.Selector.Selector();
            ApiClientMock = new Mock<ISendApiClient>();
            Handler = new PageingMessageHandler(FakeDbContext, Selector, ApiClientMock.Object, MediatorMock.Object, FakeConfiguration);
        }
        

        private void SetupConfig()
        {
            FakeConfiguration = new ConfigurationBuilder().AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>("CultureSettings:AvailableCultures:0", "en"),
                    new KeyValuePair<string, string>("CultureSettings:AvailableCultures:1", "pl"),
                    new KeyValuePair<string, string>("CultureSettings:AvailableCultures:2", "de"),
                    new KeyValuePair<string, string>("CultureSettings:AvailableCultures:3", "it"),
                    new KeyValuePair<string, string>("CultureSettings:AvailableCultures:4", "fr"),
                    new KeyValuePair<string, string>("CultureSettings:AvailableCultures:5", "es"),
                    new KeyValuePair<string, string>("CultureSettings:AvailableCultures:6", "cs"),
                    new KeyValuePair<string, string>("CultureSettings:AvailableCultures:7", "sk"),
                    new KeyValuePair<string, string>("CultureSettings:AvailableCultures:8", "hu"),
                    new KeyValuePair<string, string>("CultureSettings:AvailableCultures:9", "el"),
                    new KeyValuePair<string, string>("CultureSettings:AvailableCultures:10", "lt"),
                    new KeyValuePair<string, string>("CultureSettings:AvailableCultures:11", "da"),
                    new KeyValuePair<string, string>("CultureSettings:DefaultCulture", "pl"), 
                }
            ).Build();
        }
        private void SetupMediator()
        {
            MediatorMock = new Mock<IMediator>();
            MediatorMock.Setup(x => x.Send(It.IsAny<GetClassesQuery>(), It.IsAny<CancellationToken>())).Returns(
                (GetClassesQuery Queryable, CancellationToken cancellationToken) =>
                {
                    return Task.FromResult(new[]
                    {
                        new ClassDto {Id = "sample-class-1a1", Year = 1, Section = "a1"},
                        new ClassDto {Id = "sample-class-1b1", Year = 1, Section = "b1"},
                        new ClassDto {Id = "sample-class-1c1", Year = 1, Section = "c1"},
                        new ClassDto {Id = "sample-class-1d1", Year = 1, Section = "d1"},
                        new ClassDto {Id = "sample-class-1e1", Year = 1, Section = "e1"},
                        new ClassDto {Id = "sample-class-1a2", Year = 1, Section = "a2"},
                        new ClassDto {Id = "sample-class-1b2", Year = 1, Section = "b2"},
                        new ClassDto {Id = "sample-class-1c2", Year = 1, Section = "c2"},
                        new ClassDto {Id = "sample-class-1d2", Year = 1, Section = "d2"},
                        new ClassDto {Id = "sample-class-1e2", Year = 1, Section = "e2"},
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
                        new ClassDto {Id = "sample-class-12a", Year = 12, Section = "a"},
                        new ClassDto {Id = "sample-class-12b", Year = 12, Section = "b"}
                    }.AsEnumerable());
                });
        }
        
        public Mock<IMediator> MediatorMock { get; set; }
        public IConfiguration FakeConfiguration { get; set; }
        public IRegistrationDbContext FakeDbContext { get; set; }
        public Mock<ISendApiClient> ApiClientMock { get; set; }
        public ISelector Selector { get; set; }
        public IPagingMessageHandler Handler { get; set; }
    }
    
    public class PagingHandlerTests
    {
        [Fact]
        public async void ShouldHandleLanguagePageUp()
        {
            var builder = new PagingHandlerBuilder();
            await builder.Handler.ShowNextPage("sample-registering-user");

            builder.FakeDbContext.IncompleteUsers.Should().ContainSingle(x =>
                x.Id == "sample-registering-user" && x.Platform == "FacebookMessenger" && x.Stage == Stage.Created &&
                x.ListOffset == 10);
            builder.ApiClientMock.Verify(x => x.Send(It.IsAny<SendRequest>()), Times.Once);
        }
        
        [Fact]
        public async void ShouldHandleLanguagePageDown()
        {
            var builder = new PagingHandlerBuilder();
            await builder.Handler.ShowPreviousPage("language-paging-test-user");
            
            builder.FakeDbContext.IncompleteUsers.Should().ContainSingle(x =>
                x.Id == "language-paging-test-user" && x.Platform == "FacebookMessenger" && x.Stage == Stage.Created &&
                x.ListOffset == 0);
            builder.ApiClientMock.Verify(x => x.Send(It.IsAny<SendRequest>()), Times.Once);
        }

        [Fact]
        public async void ShouldHandleYearPageUp()
        {
            var builder = new PagingHandlerBuilder();
            await builder.Handler.ShowNextPage("sample-registering-user-with-lang");
            
            builder.FakeDbContext.IncompleteUsers.Should().ContainSingle(x =>
                x.Id == "sample-registering-user-with-lang" && x.Platform == "FacebookMessenger" && x.Stage == Stage.GatheredLanguage &&
                x.ListOffset == 10);
            builder.ApiClientMock.Verify(x => x.Send(It.IsAny<SendRequest>()), Times.Once);
        }
        
        [Fact]
        public async void ShouldHandleYearPageDown()
        {
            var builder = new PagingHandlerBuilder();
            await builder.Handler.ShowPreviousPage("year-paging-test-user");
            
            builder.FakeDbContext.IncompleteUsers.Should().ContainSingle(x =>
                x.Id == "year-paging-test-user" && x.Platform == "FacebookMessenger" && x.Stage == Stage.GatheredLanguage &&
                x.ListOffset == 0);
            builder.ApiClientMock.Verify(x => x.Send(It.IsAny<SendRequest>()), Times.Once);
        }

        [Fact]
        public async void ShouldHandleClassPageUp()
        {
            var builder = new PagingHandlerBuilder();
            await builder.Handler.ShowNextPage("sample-registering-user-with-year");
            
            builder.FakeDbContext.IncompleteUsers.Should().ContainSingle(x =>
                x.Id == "sample-registering-user-with-year" && x.Platform == "FacebookMessenger" && x.Stage == Stage.GatheredYear &&
                x.ListOffset == 10);
            builder.ApiClientMock.Verify(x => x.Send(It.IsAny<SendRequest>()), Times.Once);
        }
        
        [Fact]
        public async void ShouldHandleClassPageDown()
        {
            var builder = new PagingHandlerBuilder();
            await builder.Handler.ShowPreviousPage("class-paging-test-user");
            
            builder.FakeDbContext.IncompleteUsers.Should().ContainSingle(x =>
                x.Id == "class-paging-test-user" && x.Platform == "FacebookMessenger" && x.Stage == Stage.GatheredYear &&
                x.ListOffset == 0);
            builder.ApiClientMock.Verify(x => x.Send(It.IsAny<SendRequest>()), Times.Once);
        }
    }
}