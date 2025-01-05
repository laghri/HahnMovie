using HahnMovies.Application.Common;
using MediatR;

namespace HahnMovies.Application.Movies.Commands.Ratings;

public class RateMovieCommandHandler(IMovieRepository movieRepository) : IRequestHandler<RateMovieCommand>
{
    public async Task<Unit> Handle(RateMovieCommand request, CancellationToken cancellationToken)
    {
        var movie = await movieRepository.GetMovieByIdAsync(request.MovieId);
        if (movie == null)
        {
            throw new KeyNotFoundException($"Movie with ID {request.MovieId} not found.");
        }
        
        movie.RateMovie(request.Rating);
        
        await movieRepository.UpdateMovieAsync(movie);

        return Unit.Value;
    }
}