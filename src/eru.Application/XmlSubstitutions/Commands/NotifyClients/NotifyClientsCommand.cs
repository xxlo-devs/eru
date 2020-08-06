using System.Threading;
using System.Threading.Tasks;
using eru.Domain.Entity;
using MediatR;
using Microsoft.Extensions.Logging;

namespace eru.Application.XmlSubstitutions.Commands.NotifyClients
{
    public class NotifyClientsCommand : IRequest
    {
        public NotifyClientsCommand(SubstitutionsPlan substitutionsPlan)
        {
            SubstitutionsPlan = substitutionsPlan;
        }
        
        public SubstitutionsPlan SubstitutionsPlan { get; set; }
    }
    
    public class NotifyClientsCommandHandler : IRequestHandler<NotifyClientsCommand, Unit>
    {
        public async Task<Unit> Handle(NotifyClientsCommand request, CancellationToken cancellationToken)
        {
            return Unit.Value;
        }
    }
}