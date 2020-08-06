using eru.Application.XmlSubstitutions.Commands.UploadXmlSubstitutions;
using eru.WebApp.UploadXmlSubstitutions.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace eru.WebApp.UploadXmlSubstitutions.Controllers
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
        public IActionResult Upload(UploadModel uploadModel)
        {
            _mediator.Send(new UploadXmlSubstitutionsCommand
            {
                FileName = uploadModel.File.FileName, 
                FileStream = uploadModel.File.OpenReadStream(), 
                IpAddress = HttpContext.Connection.RemoteIpAddress.ToString(), 
                Key = uploadModel.ApiKey
            });
            return Ok();
        }
    }
}