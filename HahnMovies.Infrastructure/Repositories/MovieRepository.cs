using HahnMovies.Application.Common;
using HahnMovies.Domain.Models;
using HahnMovies.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HahnMovies.Infrastructure.Repositories;

public class MovieRepository(AppDbContext context, ILogger<MovieRepository> logger) : IMovieRepository
{
    public async Task BulkUpsertAsync(IEnumerable<Movie> movies, CancellationToken cancellationToken)
    {
        try
        {
            foreach (var batch in movies.Chunk(1000))
            {
                foreach (var movie in batch)
                {
                    var existingMovie = await context.Movies
                        .Where(m => m.Id == movie.Id)
                        .FirstOrDefaultAsync(cancellationToken);

                    if (existingMovie != null)
                    {
                        continue;
                    }

                    await context.Movies.AddAsync(movie, cancellationToken);
                }

                // Save changes for the current batch
                await context.SaveChangesAsync(cancellationToken);
                logger.LogInformation("Processed batch of {Count} movies", batch.Length);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during bulk upsert");
            throw;
        }
    }
}
