﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Domain.Entity;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace eru.Application.Substitutions.Commands
{
    public class UploadSubstitutionsCommandHandler : IRequestHandler<UploadSubstitutionsCommand, Unit>
    {
        private readonly IApplicationDbContext _context;
        private readonly IClassesParser _classesParser;
        private readonly IHangfireWrapper _hangfireWrapper;
        private readonly IEnumerable<IPlatformClient> _clients;

        public UploadSubstitutionsCommandHandler(IApplicationDbContext context, IEnumerable<IPlatformClient> clients, IHangfireWrapper hangfireWrapper, IClassesParser classesParser)
        {
            _context = context;
            _clients = clients;
            _hangfireWrapper = hangfireWrapper;
            _classesParser = classesParser;
        }

        public async Task<Unit> Handle(UploadSubstitutionsCommand request, CancellationToken cancellationToken)
        {
            var data = new HashSet<Substitution>();
            foreach (var substitution in request.Substitutions)
            {
                var classes = await _classesParser.Parse(substitution.ClassesNames);
                var trackedClasses = new List<Class>();
                foreach (var @class in classes)
                {
                    var tmpClass = await _context.Classes.FirstOrDefaultAsync(x => x.Year == @class.Year && x.Section == @class.Section, cancellationToken);
                    if (tmpClass == null)
                    {
                        var entity = await _context.Classes.AddAsync(@class, cancellationToken);
                        trackedClasses.Add(entity.Entity);
                    }
                    else
                    {
                        trackedClasses.Add(tmpClass);
                    }
                }

                data.Add(new Substitution
                {
                    Cancelled = substitution.Cancelled,
                    Classes = trackedClasses,
                    Groups = substitution.Groups,
                    Lesson = substitution.Lesson,
                    Note = substitution.Note,
                    Room = substitution.Room,
                    Subject = substitution.Subject,
                    Substituting = substitution.Substituting,
                    Teacher = substitution.Absent
                });
            }
            await _context.SaveChangesAsync(cancellationToken);
            
            var temp = _context.Classes.ToDictionary(x => x.Id, x=>new HashSet<Substitution>());
            foreach (var substitution in data)
            {
                foreach (var @class in substitution.Classes)
                {
                    temp[@class.Id].Add(substitution);
                }
            }
            foreach (var client in _clients)
            {
                foreach (var @class in temp.Keys)
                {
                    var ids = _context
                        .Users
                        .Where(x => x.Platform == client.PlatformId && x.Class == @class.ToString());
                    foreach (var id in ids)
                    {
                        _hangfireWrapper.BackgroundJobClient.Enqueue(() =>
                            client.SendMessage(id.Id, temp[@class].AsEnumerable()));
                    }
                }
            }
            return Unit.Value;
        }
    }
}