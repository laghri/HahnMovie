using HahnMovies.Application.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HahnMovies.Application.Movies.Commands.SyncMovies;

public class SyncMoviesCommandHandler(
    ITmdbService tmdbService,
    IMovieRepository movieRepository,
    ILogger<SyncMoviesCommandHandler> logger)
    : IRequestHandler<SyncMoviesCommand, Unit>
{
    public async Task<Unit> Handle(SyncMoviesCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting full movie sync");

        var movieIds = await tmdbService.GetAllMovieIdsAsync(cancellationToken);
            
        const int batchSize = 100;
        var enumerable = movieIds as int[] ?? movieIds.ToArray();
        for (var i = 0; i < enumerable.Count(); i += batchSize)
        {
            var batchIds = enumerable.Skip(i).Take(batchSize);
            var movies = await tmdbService.GetMovieDetailsAsync(batchIds, cancellationToken);
                
            await movieRepository.BulkUpsertAsync(movies, cancellationToken);
                
            logger.LogInformation("Processed {Count} movies", i + batchSize);
        }

        return Unit.Value;
    }
}