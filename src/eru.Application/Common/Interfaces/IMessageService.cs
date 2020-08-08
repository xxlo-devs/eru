using System.Collections.Generic;
using System.Threading.Tasks;
using eru.Domain.Entity;

namespace eru.Application.Common.Interfaces
{
    public interface IMessageService
    {
        Task<IEnumerable<string>> GetIdsOfAllSubscribers();
        Task<IEnumerable<string>> GetIdsOfSubscribersInClass(Class targetClass);
        Task SendSubstitutionNotification(string idOfTarget, Substitution substitution);
    }
}