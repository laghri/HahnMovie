using HahnMovies.Domain.Models;

namespace HahnMovies.Application.Common;

public interface IMovieRepository
{
    Task BulkUpsertAsync(IEnumerable<Movie> movies, CancellationToken cancellationToken);
}