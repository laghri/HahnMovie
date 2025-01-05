using System.Globalization;
using HahnMovies.Application.Common;
using HahnMovies.Domain.Models;
using HahnMovies.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HahnMovies.Infrastructure.Repositories;

public class MovieRepository(AppDbContext context, ILogger<MovieRepository> logger) : IMovieRepository
{
    public async Task<Movie?> GetMovieByIdAsync(int movieId)
    {
        return await context.Movies.FindAsync(movieId);
    }

    public async Task UpdateMovieAsync(Movie movie)
    {
        context.Movies.Update(movie);
        await context.SaveChangesAsync();
    }

    public async Task AddMovieAsync(IEnumerable<Movie> movies, CancellationToken cancellationToken)
    {
        try
        {
            foreach (var batch in movies.Chunk(1000))
            {
                var movieIds = batch.Select(m => m.Id).ToList();
                var existingMovies = await context.Movies
                    .Where(m => movieIds.Contains(m.Id))
                    .ToDictionaryAsync(m => m.Id, cancellationToken);

                foreach (var movie in batch)
                {
                    if (!DateTime.TryParse(movie.ReleaseDate.ToString(CultureInfo.InvariantCulture), out _))
                    {
                        logger.LogWarning("Invalid ReleaseDate for Movie Id {MovieId}. Skipping movie.", movie.Id);
                        continue;
                    }

                    if (!DateTime.TryParse(movie.LastUpdated.ToString(CultureInfo.InvariantCulture), out _))
                    {
                        logger.LogWarning("Invalid LastUpdated for Movie Id {MovieId}. Skipping movie.", movie.Id);
                        continue;
                    }

                    if (existingMovies.TryGetValue(movie.Id, out var existingMovie))
                    {
                        existingMovie.Update(movie.Title, movie.ReleaseDate, movie.VoteAverage, movie.PosterPath);
                    }
                    else
                    {
                        await context.Movies.AddAsync(movie, cancellationToken);
                    }
                }

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

    public async Task<IEnumerable<Movie>> SearchMoviesAsync(string title, CancellationToken cancellationToken)
    {
        return await context.Movies
            .Where(m => EF.Functions.Like(m.Title, $"%{title}%"))
            .ToListAsync(cancellationToken);
    }
}