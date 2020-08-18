using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Classes.Commands.CreateClass;
using eru.Application.Classes.Commands.RemoveClass;
using eru.Application.Classes.Queries.GetClasses;
using eru.Application.Users.Queries.GetIdsOfSubscribersInClass;
using eru.Application.Users.Queries.GetSubscribersCount;
using eru.WebApp.Areas.AdminDashboard.Model;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eru.WebApp.Areas.AdminDashboard.Controllers
{
    [Route("admin")]
    [Area("AdminDashboard")]
    public class AdminDashboardController : Controller
    {
        private readonly IMediator _mediator;

        public AdminDashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public IActionResult Dashboard()
            => View();
        
        [HttpGet("status")]
        public async Task<Status> GetStatus()
        {
            var classes = new Dictionary<string, int>();
            foreach (var @class in await _mediator.Send(new GetClassesQuery()))
            {
                classes[@class.Name] =
                    await _mediator.Send(new GetSubscribersCount(@class.Name), CancellationToken.None);
            }
            var status = new Status
            {
                Uptime = DateTime.Now - System.Diagnostics.Process.GetCurrentProcess().StartTime,
                Subscribers = await _mediator.Send(new GetSubscribersCount(), CancellationToken.None),
                Classes = classes
            };
            return status;
        }

        [HttpPost("class")]
        public async Task AddClass(string name)
        {
            await _mediator.Send(new CreateClassCommand {Name = name});
        }
        
        [HttpDelete("class")]
        public async Task RemoveClass(string name)
        {
            await _mediator.Send(new RemoveClassCommand {Name = name});
        }
    }
}