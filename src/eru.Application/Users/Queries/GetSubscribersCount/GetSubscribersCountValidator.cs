﻿using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace eru.Application.Users.Queries.GetSubscribersCount
{
    public class GetSubscribersCountValidator : AbstractValidator<GetSubscribersCount>
    {
        private readonly IApplicationDbContext _context;
        public GetSubscribersCountValidator(IApplicationDbContext context)
        {
            _context = context;
            RuleFor(x => x.ClassId)
                .MustAsync(IsValidClassId);
        }

        private async Task<bool> IsValidClassId(string id, CancellationToken cancellationToken)
        {
            return id == null || await _context.Classes.AnyAsync(x => x.Id == id, cancellationToken);
        }
    }
}