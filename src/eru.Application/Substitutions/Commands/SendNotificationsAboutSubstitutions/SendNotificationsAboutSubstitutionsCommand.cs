using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Common.Interfaces;
using eru.Domain.Entity;
using MediatR;

namespace eru.Application.Substitutions.Commands.SendNotificationsAboutSubstitutions
{
    public class SendNotificationsAboutSubstitutionsCommand : IRequest
    {
        public SubstitutionsPlan SubstitutionsPlan { get; set; }
        public string Key { get; set; }
        public string IpAddress { get; set; }

        public override string ToString()
        {
            return $"Key( {Key} ) - Ip( {IpAddress} )";
        }
    }
    
    public class SendNotificationsAboutSubstitutionsCommandHandler : IRequestHandler<SendNotificationsAboutSubstitutionsCommand, Unit>
    {
        private readonly IBackgroundJobClient _client;
        private readonly IMediator _mediator;
        private readonly IEnumerable<IMessageService> _messageServices;

        public SendNotificationsAboutSubstitutionsCommandHandler(IBackgroundJobClient client, IMediator mediator, IEnumerable<IMessageService> messageServices)
        {
            _client = client;
            _mediator = mediator;
            _messageServices = messageServices;
        }

        public Task<Unit> Handle(SendNotificationsAboutSubstitutionsCommand request, CancellationToken cancellationToken)
        {
            _client.Enqueue(() => PropagateSubstitutionsNotifications(request.SubstitutionsPlan));
            return Task.FromResult(Unit.Value);
        }

        public Task PropagateSubstitutionsNotifications(SubstitutionsPlan plan)
        {
            var messagesQueue = new Dictionary<Class, List<Substitution>>();
            foreach (var substitution in plan.Substitutions)
            {
                foreach (var @class in substitution.Classes)
                {
                    if(messagesQueue.ContainsKey(@class))
                        messagesQueue[@class].Add(substitution);
                    else
                        messagesQueue.Add(@class, new List<Substitution> {substitution});
                }
            }
            foreach (var (@class, substitutions) in messagesQueue)
            {
                foreach (var messageService in _messageServices)
                {
                    _client.Enqueue(() => SendSubstitutionsNotificationsToClass(substitutions.ToArray(), @class, messageService));
                }
            }

            return Task.CompletedTask;
        }

        public async Task SendSubstitutionsNotificationsToClass(Substitution[] substitutions, Class @class, IMessageService messageService)
        {
            var studentsIds = await messageService.GetIdsOfSubscribersInClass(@class);
            foreach (var studentId in studentsIds)
            {
                foreach (var substitution in substitutions)
                {
                    _client.Enqueue(() => messageService.SendSubstitutionNotification(studentId, substitution));
                }
            }
        }
    }
}