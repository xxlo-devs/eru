using System.Threading.Tasks;
using eru.Application.Classes.Commands.CreateClass;
using eru.Application.Classes.Commands.RemoveClass;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eru.WebApp.Controllers.Admin
{
    [Authorize]
    [Route("/admin/class")]
    public class ClassesController : Controller
    {
        private readonly IMediator _mediator;

        public ClassesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task AddClass(int year, string section)
        {
            await _mediator.Send(new CreateClassCommand(year, section));
        }
        [HttpDelete]
        public async Task RemoveClass(string id)
        {
            await _mediator.Send(new RemoveClassCommand(id));
        }
    }
}