using System.Diagnostics.CodeAnalysis;
using eru.Domain.Entity;
using MediatR;

namespace eru.Application.Substitutions.Notifications
{
    public class SendSubstitutionNotification : INotification
    {
        [ExcludeFromCodeCoverage]
        public SendSubstitutionNotification(Substitution substitution)
        {
            Substitution = substitution;
        }

        [ExcludeFromCodeCoverage]
        public Substitution Substitution { get; set; }
    }
}