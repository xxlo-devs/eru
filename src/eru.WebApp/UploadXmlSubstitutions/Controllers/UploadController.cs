using System.Threading.Tasks;
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
        public async Task<IActionResult> Upload(UploadModel uploadModel)
        {
            await _mediator.Send(new UploadXmlSubstitutionsCommand
            {
                SubstitutionsPlan = uploadModel.XmlModel.ToSubstitutionsPlan(),
                IpAddress = HttpContext.Connection.RemoteIpAddress.ToString(), 
                Key = uploadModel.ApiKey
            });
            return Ok();
        }
    }
}