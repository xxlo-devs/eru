using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Application.Substitutions.Notifications;
using eru.Domain.Entity;
using MediatR;

namespace eru.Application.Substitutions.Commands
{
    public class UploadSubstitutionsCommand : IRequest
    {
        public string Key { get; set; }
        public string IpAddress { get; set; }
        public SubstitutionsPlan SubstitutionsPlan { get; set; }

        public override string ToString()
        {
            return "{" + $"Ip Address: {IpAddress}, Key: {Key} " + "}";
        }
    }
    
    public class UploadSubstitutionsCommandHandler : IRequestHandler<UploadSubstitutionsCommand, Unit>
    {
        private readonly IMediator _mediator;
        private readonly IBackgroundExecutor _backgroundExecutor;

        public UploadSubstitutionsCommandHandler(IMediator mediator, IBackgroundExecutor backgroundExecutor)
        {
            _mediator = mediator;
            _backgroundExecutor = backgroundExecutor;
        }

        public Task<Unit> Handle(UploadSubstitutionsCommand request, CancellationToken cancellationToken)
        {
            foreach (var substitution in request.SubstitutionsPlan.Substitutions)
            {
                _backgroundExecutor.Enqueue(() => _mediator.Publish(new SendSubstitutionNotification(substitution), cancellationToken));
            }
            return Task.FromResult(Unit.Value);
        }
    }
}