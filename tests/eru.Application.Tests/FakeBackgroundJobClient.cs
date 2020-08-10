using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;

namespace eru.Application.Tests
{
    public class FakeBackgroundJobClient : IBackgroundJobClient
    {
        public Queue<string> EnqueuedJobs { get; } = new Queue<string>();
        public string Enqueue(Expression<Func<Task>> methodToCall)
        {
            EnqueuedJobs.Enqueue(methodToCall.ToString());
            return string.Empty;
        }
    }
}