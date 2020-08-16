using System.Diagnostics.CodeAnalysis;
using eru.Domain.Entity;
using MediatR;

namespace eru.Application.Substitutions.Notifications
{
    public class SendSubstitutionNotification : INotification
    {
        public SendSubstitutionNotification(Substitution substitution)
        {
            Substitution = substitution;
        }

        public Substitution Substitution { get; set; }
    }
}