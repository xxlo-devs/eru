using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
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
        private readonly IApplicationDbContext _context;
        private readonly IBackgroundExecutor _backgroundExecutor;
        private readonly IEnumerable<IPlatformClient> _clients;

        public UploadSubstitutionsCommandHandler(IApplicationDbContext context, IBackgroundExecutor backgroundExecutor, IEnumerable<IPlatformClient> clients)
        {
            _context = context;
            _backgroundExecutor = backgroundExecutor;
            _clients = clients;
        }

        public Task<Unit> Handle(UploadSubstitutionsCommand request, CancellationToken cancellationToken)
        {
            var data = _context.Classes.ToDictionary(x => x.Name, x=>new HashSet<Substitution>());
            foreach (var substitution in request.SubstitutionsPlan.Substitutions)
            {
                foreach (var @class in substitution.Classes)
                {
                    data[@class.Name].Add(substitution);
                }
            }
            foreach (var client in _clients)
            {
                foreach (var @class in data.Keys)
                {
                    var ids = _context
                        .Users
                        .Where(x => x.Platform == client.PlatformId && x.Class == @class.ToString());
                    foreach (var id in ids)
                    {
                        _backgroundExecutor.Enqueue(() => client.SendMessage(id.Id, data[@class].AsEnumerable()));
                    }
                }
            }
            return Task.FromResult(Unit.Value);
        }
    }
}