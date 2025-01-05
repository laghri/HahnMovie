using MediatR;

namespace HahnMovies.Application.Movies.Commands;

public class RateMovieCommand : IRequest
{
    public int MovieId { get; init; }
    public int Rating { get; init; }
}