using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eru.Application.Common.Behaviours
{
    public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IStopwatch _stopwatch;
        private readonly ILogger _logger;

        public PerformanceBehaviour(IStopwatch stopwatch, ILogger<TRequest> logger)
        {
            _stopwatch = stopwatch;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            _stopwatch.Start();
            var response = await next();
            _stopwatch.Stop();
            var elapsedMilliseconds = _stopwatch.ElapsedMilliseconds;
            if (elapsedMilliseconds > 500) //TODO It should be set in config
            {
                var requestName = typeof(TRequest).Name;
                _logger.LogWarning("eru Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@Request}",
                    requestName, elapsedMilliseconds, request);
            }

            return response;
        }
    }
}