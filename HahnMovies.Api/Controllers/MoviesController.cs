using HahnMovies.Application.Movies.Commands.SyncMovies;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HahnMovies.Api.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("sync")]
        public async Task<IActionResult> SyncMovies(CancellationToken cancellationToken)
        {
            await _mediator.Send(new SyncMoviesCommand(), cancellationToken);
            return Ok("Sync completed successfully");
        }
    }
}