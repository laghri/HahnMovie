using HahnMovies.Application.Movies.Commands.Comments;
using HahnMovies.Application.Movies.Commands.Ratings;
using HahnMovies.Application.Movies.Commands.SyncMovies;
using HahnMovies.Application.Movies.Queries;
using HahnMovies.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HahnMovies.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieController(IMediator mediator) : ControllerBase
    {
        [HttpPost("sync")]
        public async Task<IActionResult> SyncMovies(CancellationToken cancellationToken)
        {
            await mediator.Send(new SyncMoviesCommand(), cancellationToken);
            return Ok("Sync completed successfully");
        }

        [HttpGet]
        public async Task<IActionResult> SearchMovies([FromQuery] string title, CancellationToken cancellationToken)
        {
            var query = new SearchMoviesQuery { Title = title };
            var movies = await mediator.Send(query, cancellationToken);

            var moviesList = movies as Movie[] ?? movies.ToArray();
            return Ok(!moviesList.Any() ? new List<Movie>() : moviesList);
        }

        [HttpPost("{movieId}/rate")]
        public async Task<IActionResult> RateMovie(int movieId, [FromBody] int rating)
        {
            if (rating < 1 || rating > 10)
            {
                return BadRequest("Rating must be between 1 and 10.");
            }

            var command = new RateMovieCommand
            {
                MovieId = movieId,
                Rating = rating
            };

            await mediator.Send(command);

            return Ok();
        }

        [HttpPost("{movieId}/comment")]
        public async Task<IActionResult> CommentOnMovie(int movieId, [FromBody] AddMovieCommentRequest request)
        {
            var command = new AddMovieCommentCommand
            {
                MovieId = movieId,
                Username = request.Username,
                Comment = request.Comment
            };

            await mediator.Send(command);

            return Ok(new { message = "Comment added successfully" });
        }
    }
}