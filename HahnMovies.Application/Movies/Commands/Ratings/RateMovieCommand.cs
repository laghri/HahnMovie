using MediatR;

namespace HahnMovies.Application.Movies.Commands.Ratings;

public class RateMovieCommand : IRequest
{
    public int MovieId { get; init; }
    
    public int Rating { get; init; }
}