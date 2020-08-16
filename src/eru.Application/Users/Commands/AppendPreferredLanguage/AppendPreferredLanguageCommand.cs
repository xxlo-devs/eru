using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Domain.Enums;
using MediatR;

namespace eru.Application.Users.Commands.AppendPreferredLanguage
{
    public class AppendPreferredLanguageCommand : IRequest<Unit>
    {
        public string UserId { get; set; }
        public string Platform { get; set; }
        public string PrefferedLanguage { get; set; }
    }

    public class AppendPrefferedLanguageCommandHandler : IRequestHandler<AppendPreferredLanguageCommand, Unit>
    {
        private readonly IApplicationDbContext _dbContext;

        public AppendPrefferedLanguageCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(AppendPreferredLanguageCommand command, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users.FindAsync(command.UserId, command.Platform);

            user.PreferredLanguage = command.PrefferedLanguage;
            user.Stage = Stage.GatheredLanguage;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
