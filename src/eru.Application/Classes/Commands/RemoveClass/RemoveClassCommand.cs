using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using MediatR;

namespace eru.Application.Classes.Commands.RemoveClass
{
    public class RemoveClassCommand : IRequest
    {
        public RemoveClassCommand(string id)
        {
            Id = id;
        }
        public string Id { get; set; }
    }
    
    public class RemoveClassCommandHandler : IRequestHandler<RemoveClassCommand, Unit>
    {
        private readonly IApplicationDbContext _context;

        public RemoveClassCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(RemoveClassCommand request, CancellationToken cancellationToken)
        {
            var obj = await _context.Classes.FindAsync(request.Id);
            _context.Classes.Remove(obj);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}