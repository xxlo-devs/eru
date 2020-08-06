using System.IO;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Application.XmlSubstitutions.Commands.NotifyClients;
using MediatR;

namespace eru.Application.XmlSubstitutions.Commands.UploadXmlSubstitutions
{
    public class UploadXmlSubstitutionsCommand : IRequest
    {
        public string FileName { get; set; }
        public Stream FileStream { get; set; }
        public string Key { get; set; }
        public string IpAddress { get; set; }

        public override string ToString()
        {
            return $"FileName( {FileName} ) - Key( {Key} ) - Ip( {IpAddress} )";
        }
    }
    
    public class UploadXmlSubstitutionsCommandHandler : IRequestHandler<UploadXmlSubstitutionsCommand, Unit>
    {
        private readonly IBackgroundJobClient _client;
        private readonly ISubstitutionsPlanXmlParser _parser;
        private readonly IMediator _mediator;

        public UploadXmlSubstitutionsCommandHandler(IBackgroundJobClient client, IMediator mediator, ISubstitutionsPlanXmlParser parser)
        {
            _client = client;
            _mediator = mediator;
            _parser = parser;
        }

        public async Task<Unit> Handle(UploadXmlSubstitutionsCommand request, CancellationToken cancellationToken)
        {
            var substitutionsPlan = await _parser.Parse(request.FileStream);
            _client.Enqueue(() => _mediator.Send(new NotifyClientsCommand(substitutionsPlan), cancellationToken));
            return Unit.Value;
        }
    }
}