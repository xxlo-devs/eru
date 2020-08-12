using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using Hangfire;

namespace eru.Infrastructure.Hangfire
{
    public class HangfireExecutror : IBackgroundExecutor
    {
        public string Enqueue(Expression<Func<Task>> method) => BackgroundJob.Enqueue(method);
    }
}