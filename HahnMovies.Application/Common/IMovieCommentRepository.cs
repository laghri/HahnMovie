using HahnMovies.Domain.Models;

namespace HahnMovies.Application.Common;

public interface IMovieCommentRepository
{
    Task AddCommentAsync(MovieComment movieComment, CancellationToken cancellationToken);
    
    Task<List<MovieComment>> GetCommentsByMovieIdAsync(int movieId, CancellationToken cancellationToken);
}