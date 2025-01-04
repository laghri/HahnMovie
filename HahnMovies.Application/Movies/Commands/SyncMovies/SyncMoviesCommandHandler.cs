using HahnMovies.Application.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HahnMovies.Application.Movies.Commands.SyncMovies;

public class SyncMoviesCommandHandler : IRequestHandler<SyncMoviesCommand, Unit>
{
    private readonly ITmdbService _tmdbService;
    private readonly IMovieRepository _movieRepository;
    private readonly ILogger<SyncMoviesCommandHandler> _logger;

    public SyncMoviesCommandHandler(
        ITmdbService tmdbService,
        IMovieRepository movieRepository,
        ILogger<SyncMoviesCommandHandler> logger)
    {
        _tmdbService = tmdbService;
        _movieRepository = movieRepository;
        _logger = logger;
    }

    public async Task<Unit> Handle(SyncMoviesCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting full movie sync");

        var movieIds = await _tmdbService.GetAllMovieIdsAsync(cancellationToken);
            
        const int batchSize = 100;
        for (var i = 0; i < movieIds.Count(); i += batchSize)
        {
            var batchIds = movieIds.Skip(i).Take(batchSize);
            var movies = await _tmdbService.GetMovieDetailsAsync(batchIds, cancellationToken);
                
            await _movieRepository.BulkUpsertAsync(movies, cancellationToken);
                
            _logger.LogInformation("Processed {Count} movies", i + batchSize);
        }

        return Unit.Value;
    }
}