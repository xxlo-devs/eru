using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Domain.Entity;
using MediatR;

namespace eru.Application.XmlSubstitutions.Commands.UploadXmlSubstitutions
{
    public class UploadXmlSubstitutionsCommand : IRequest
    {
        public SubstitutionsPlan SubstitutionsPlan { get; set; }
        public string Key { get; set; }
        public string IpAddress { get; set; }

        public override string ToString()
        {
            return $"Key( {Key} ) - Ip( {IpAddress} )";
        }
    }
    
    public class UploadXmlSubstitutionsCommandHandler : IRequestHandler<UploadXmlSubstitutionsCommand, Unit>
    {
        private readonly IBackgroundJobClient _client;
        private readonly IMediator _mediator;

        public UploadXmlSubstitutionsCommandHandler(IBackgroundJobClient client, IMediator mediator)
        {
            _client = client;
            _mediator = mediator;
        }

        public Task<Unit> Handle(UploadXmlSubstitutionsCommand request, CancellationToken cancellationToken)
        {
            //Here notification should be sent to all interested students
            return Task.FromResult(Unit.Value);
        }
    }
}