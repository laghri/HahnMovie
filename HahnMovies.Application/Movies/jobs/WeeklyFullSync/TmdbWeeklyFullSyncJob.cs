using HahnMovies.Application.Common;
using HahnMovies.Domain.Models;
using Microsoft.Extensions.Logging;

namespace HahnMovies.Application.Movies.jobs.WeeklyFullSync
{
    public class TmdbFullSyncJob(
        ITmdbService tmdbService,
        IMovieRepository movieRepository,
        ILogger<TmdbFullSyncJob> logger)
    {
        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting the TMDB weekly full-sync job.");

            try
            {
                var movieIds = await tmdbService.GetAllMovieIdsAsync(cancellationToken);
                var enumerable = movieIds as int[] ?? movieIds.ToArray();
                logger.LogInformation("Fetched {Count} movie IDs from TMDB.", enumerable.Count());

                const int batchSize = 100;

                foreach (var batch in enumerable.Chunk(batchSize))
                {
                    var movies = await tmdbService.GetMovieDetailsAsync(batch, cancellationToken);

                    var moviesList = movies as Movie[] ?? movies.ToArray();
                    await movieRepository.AddMovieAsync(moviesList, cancellationToken);
                    logger.LogInformation("Upserted {Count} movies into the database.", moviesList.Count());
                }

                logger.LogInformation("TMDB weekly full-sync job completed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred during the TMDB full-sync job.");
            }
        }
    }
}