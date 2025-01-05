using HahnMovies.Domain.Models;

namespace HahnMovies.Application.Common;

public interface ITmdbService
{
    Task<IEnumerable<int>> GetAllMovieIdsAsync(CancellationToken cancellationToken);
    Task<IEnumerable<Movie>> GetMovieDetailsAsync(IEnumerable<int> movieIds, CancellationToken cancellationToken);
    Task<IEnumerable<int>> GetUpdatedMovieIdsAsync(CancellationToken cancellationToken);
    
}