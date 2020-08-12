using System.Threading.Tasks;
using eru.Application.Substitutions.Commands;
using eru.WebApp.Areas.UploadSubstitutions.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;

namespace eru.WebApp.Areas.UploadSubstitutions.Controllers
{
    [Route("substitutions")]
    [Area("UploadSubstitutions")]
    public class UploadController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IHtmlLocalizer<UploadController> _localizer;

        public UploadController(IMediator mediator, IHtmlLocalizer<UploadController> localizer)
        {
            _mediator = mediator;
            _localizer = localizer;
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(UploadModel uploadModel)
        {
            await _mediator.Send(new UploadSubstitutionsCommand
            {
                SubstitutionsPlan = uploadModel.XmlModel.ToSubstitutionsPlan(),
                IpAddress = HttpContext.Connection.RemoteIpAddress.ToString(), 
                Key = uploadModel.ApiKey
            });
            return Ok();
        }
    }
}