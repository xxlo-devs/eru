using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Classes.Commands.CreateClass;
using eru.Application.Classes.Commands.RemoveClass;
using eru.Application.Classes.Queries.GetClasses;
using eru.Application.Subscriptions.Queries.GetSubscribersCount;
using eru.WebApp.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eru.WebApp.Controllers
{
    [Route("admin")]
    public class AdminDashboardController : Controller
    {
        private readonly IMediator _mediator;

        public AdminDashboardController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("status")]
        public async Task<Status> GetStatus()
        {
            var classes = new Dictionary<string, int>();
            foreach (var @class in await _mediator.Send(new GetClassesQuery()))
            {
                classes[@class.Id] =
                    await _mediator.Send(new GetSubscribersCount(@class.Id), CancellationToken.None);
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
        public async Task AddClass(int year, string section)
        {
            await _mediator.Send(new CreateClassCommand {Year = year, Section = section});
        }
        
        [HttpDelete("class")]
        public async Task RemoveClass(string id)
        {
            await _mediator.Send(new RemoveClassCommand {Id = id});
        }
    }
}