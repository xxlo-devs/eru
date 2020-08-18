using Hangfire;

namespace eru.Application.Common.Interfaces
{
    public interface IHangfireWrapper
    {
        public IBackgroundJobClient BackgroundJobClient { get; }
    }
}