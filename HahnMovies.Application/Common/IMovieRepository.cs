using HahnMovies.Domain.Models;

namespace HahnMovies.Application.Common;

public interface IMovieRepository
{
    Task<Movie?> GetMovieByIdAsync(int movieId);
    Task UpdateMovieAsync(Movie movie);
    Task AddMovieAsync(IEnumerable<Movie> movies, CancellationToken cancellationToken);
    
    Task<IEnumerable<Movie>> SearchMoviesAsync(string title, CancellationToken cancellationToken);
}