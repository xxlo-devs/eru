using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Classes.Commands.CreateClass;
using eru.Application.Classes.Commands.RemoveClass;
using eru.Application.Classes.Queries.GetClasses;
using eru.Application.Subscriptions.Queries.GetSubscribersCount;
using eru.Domain.Entity;
using eru.WebApp.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

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
            var classes = new List<ClassInfo>();
            foreach (var @class in await _mediator.Send(new GetClassesQuery()))
            {
                classes.Add(new ClassInfo()
                {
                    Id = @class.Id,
                    Name = @class.ToString(),
                    SubscribersCount = await _mediator.Send(new GetSubscribersCount(@class.Id), CancellationToken.None)
                });
            }
            var status = new Status
            {
                Uptime = DateTime.Now - System.Diagnostics.Process.GetCurrentProcess().StartTime,
                Subscribers = await _mediator.Send(new GetSubscribersCount(), CancellationToken.None),
                Classes = classes
            };
            return status;
        }

        [HttpGet("/logout")]
        public async Task<RedirectResult> Logout()
        {
            await HttpContext.RequestServices.GetService<SignInManager<User>>().SignOutAsync();
            return RedirectPermanent("/login");
        }

        [HttpPost("class")]
        public async Task AddClass(int year, string section)
        {
            await _mediator.Send(new CreateClassCommand(year, section));
        }
        
        [HttpDelete("class")]
        public async Task RemoveClass(string id)
        {
            await _mediator.Send(new RemoveClassCommand(id));
        }
    }
}