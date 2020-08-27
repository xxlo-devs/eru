using System;
using System.Threading.Tasks;
using eru.Application.Substitutions.Commands;
using eru.WebApp.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;

namespace eru.WebApp.Controllers
{
    [Route("substitutions")]
    public class UploadController : Controller
    {
        private readonly IMediator _mediator;

        public UploadController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(UploadModel uploadModel)
        {
            await _mediator.Send(new UploadSubstitutionsCommand
            {
                UploadDateTime = DateTime.Now,
                IpAddress = HttpContext.Connection.RemoteIpAddress.ToString(), 
                Key = uploadModel.ApiKey,
                SubstitutionsDate = uploadModel.XmlModel.DateNode.GetDateTime(),
                Substitutions = uploadModel.XmlModel.DateNode.GetSubstitutionsDto()
            });
            return Ok();
        }
    }
}