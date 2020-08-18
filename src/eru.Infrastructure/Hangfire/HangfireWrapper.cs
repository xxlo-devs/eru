using eru.Application.Common.Interfaces;
using Hangfire;

namespace eru.Infrastructure.Hangfire
{
    public class HangfireWrapper : IHangfireWrapper
    {
        public IBackgroundJobClient BackgroundJobClient => new BackgroundJobClient(JobStorage.Current);
    }
}