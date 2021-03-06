﻿using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Domain.Entity;
using MediatR;

namespace eru.Application.Classes.Commands.CreateClass
{
    public class CreateClassCommand : IRequest
    {
        public CreateClassCommand(int year, string section)
        {
            Year = year;
            Section = section;
        }
        public int Year { get; set; }
        public string Section { get; set; }
    }
    
    public class CreateClassCommandHandler : IRequestHandler<CreateClassCommand, Unit>
    {
        private readonly IApplicationDbContext _context;

        public CreateClassCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(CreateClassCommand request, CancellationToken cancellationToken)
        {
            await _context.Classes.AddAsync(new Class(request.Year, request.Section), cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}