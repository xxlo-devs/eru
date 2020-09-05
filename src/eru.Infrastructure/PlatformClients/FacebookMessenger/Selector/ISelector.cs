using System.Collections.Generic;
using System.Threading.Tasks;
using eru.Application.Classes.Queries.GetClasses;
using eru.Infrastructure.PlatformClients.FacebookMessenger.Models.SendApi;

namespace eru.Infrastructure.PlatformClients.FacebookMessenger.Selector
{
    public interface ISelector
    {
        public Task<IEnumerable<QuickReply>> GetLangSelector(int page);
        public Task<IEnumerable<QuickReply>> GetYearSelector(int page, string preferredLanguage);
        public Task<IEnumerable<QuickReply>> GetClassSelector(int page, int year, string preferredLanguage);

        public Task<IEnumerable<QuickReply>> GetConfirmationSelector(string preferredLanguage);
        public Task<IEnumerable<QuickReply>> GetCancelSelector(string preferredLanguage);
    }
}