﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using eru.Application.Classes.Queries.GetClasses;
using eru.Application.Subscriptions.Queries.GetSubscribersCount;
using eru.WebApp.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eru.WebApp.Controllers.Admin
{
    [Authorize]
    [Route("admin/status")]
    public class StatusController : Controller
    {
        private readonly IMediator _mediator;

        public StatusController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpGet]
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
    }
}