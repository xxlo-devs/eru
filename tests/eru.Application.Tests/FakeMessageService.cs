using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Domain.Entity;

namespace eru.Application.Tests
{
    public class FakeMessageService : IMessageService
    {
        public int StudentsCount { get; } = 10;
        
        [ExcludeFromCodeCoverage]
        public async Task<IEnumerable<string>> GetIdsOfAllSubscribers()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetIdsOfSubscribersInClass(Class targetClass)
        {
            return Task.FromResult(Enumerable.Range(1,StudentsCount).Select(x=>x.ToString()));
        }
        
        [ExcludeFromCodeCoverage]
        public Task SendSubstitutionNotification(string idOfTarget, Substitution substitution)
        {
            throw new NotImplementedException();
        }
    }
}