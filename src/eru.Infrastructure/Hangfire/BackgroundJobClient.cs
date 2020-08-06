using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Hangfire;

namespace eru.Infrastructure.Hangfire
{
    public class BackgroundJobClient : Application.Common.Interfaces.IBackgroundJobClient
    {
        public string Enqueue(Expression<Func<Task>> methodToCall)
        {
            return BackgroundJob.Enqueue(methodToCall);
        }
    }
}