using HahnMovies.Application.Common;
using HahnMovies.Domain.Models;
using Microsoft.Extensions.Logging;

namespace HahnMovies.Application.Movies.jobs.ChangesMovie;

    public class TmdbChangesMovieSyncJob(
        ITmdbService tmdbService,
        IMovieRepository movieRepository,
        ILogger<TmdbChangesMovieSyncJob> logger)
    {
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting the TMDB daily partial-sync job.");

            try
            {
                var updatedMovieIds = await tmdbService.GetUpdatedMovieIdsAsync(cancellationToken);
                var enumerable = updatedMovieIds as int[] ?? updatedMovieIds.ToArray();
                logger.LogInformation("Fetched {Count} updated movie IDs from TMDB.", enumerable.Count());

                const int batchSize = 100;
                
                foreach (var batch in enumerable.Chunk(batchSize))
                {
                    var updatedMovies = await tmdbService.GetMovieDetailsAsync(batch, cancellationToken);
                    
                    var movies = updatedMovies as Movie[] ?? updatedMovies.ToArray();
                    await movieRepository.BulkUpsertAsync(movies, cancellationToken);
                    logger.LogInformation("Upserted {Count} updated movies into the database.", movies.Count());
                }

                logger.LogInformation("TMDB daily partial-sync job completed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred during the TMDB partial-sync job.");
            }
        }
    }

