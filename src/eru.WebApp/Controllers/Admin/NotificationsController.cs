using System.Threading.Tasks;
using eru.Application.Notifications.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eru.WebApp.Controllers.Admin
{
    [Authorize]
    [Route("admin/notification")]
    public class NotificationsController : Controller
    {
        private readonly IMediator _mediator;

        public NotificationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> SendNotification([FromBody] string content)
        {
            await _mediator.Send(new SendGlobalNotification(content));
            return Ok();
        }
    }
}