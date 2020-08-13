using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eru.Application.Users.Commands.AppendYear
{
    public class AppendYearCommand : IRequest
    {
        public string UserId { get; set; }
        public Year Year { get; set; }
        public Platform Platform { get; set; }
    }

    public class AppendYearCommandHandler : IRequestHandler<AppendYearCommand>
    {
        private readonly IApplicationDbContext _dbContext;

        public AppendYearCommandHandler(IApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Unit> Handle(AppendYearCommand command, CancellationToken cancellationToken)
        {
            var user = await _dbContext.Users
                .Where(x => x.Id == command.UserId & x.Platform == command.Platform)
                .FirstOrDefaultAsync();

            user.Year = command.Year;
            user.Stage = Stage.GatheredYear;

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
