using System.Threading.Tasks;
using eru.Application.Substitutions.Queries.GetLatestSubstitution;
using eru.Domain.Entity;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace eru.WebApp.Pages
{
    public class Substitutions : PageModel
    {
        private readonly IMediator _mediator;

        public Substitutions(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [BindProperty]
        public SubstitutionsRecord SubstitutionsRecord { get; set; }

        public async Task OnGet()
        {
            SubstitutionsRecord = await _mediator.Send(new GetLatestSubstitution());
        }
    }
}