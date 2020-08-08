using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Domain.Entity;
using Microsoft.Extensions.Logging;

namespace eru.Infrastructure
{
    public class DebugMessageService : IMessageService
    {
        private readonly ILogger<DebugMessageService> _logger;

        public DebugMessageService(ILogger<DebugMessageService> logger)
        {
            _logger = logger;
        }

        public Task<IEnumerable<string>> GetIdsOfAllSubscribers()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<string>> GetIdsOfSubscribersInClass(Class targetClass)
        {
            await Task.Delay(5000);
            return new[]
            {
                Guid.NewGuid().ToString()
            };
        }

        public async Task SendSubstitutionNotification(string idOfTarget, Substitution substitution)
        {
            await Task.Delay(new Random().Next(1000, 15000));
            _logger.LogInformation($"{idOfTarget} : {substitution.Lesson} {substitution.Substituting} -> {substitution.Teacher}");
        }
    }
}