using HahnMovies.Application.Common;
using HahnMovies.Domain.Models;
using HahnMovies.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HahnMovies.Infrastructure.Repositories
{
    public class MovieCommentRepository(AppDbContext context) : IMovieCommentRepository
    {
        public async Task AddCommentAsync(MovieComment movieComment, CancellationToken cancellationToken)
        {
            await context.MovieComments.AddAsync(movieComment, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<MovieComment>> GetCommentsByMovieIdAsync(int movieId, CancellationToken cancellationToken)
        {
            return await context.MovieComments
                .Where(c => c.MovieId == movieId)
                .ToListAsync(cancellationToken);
        }
    }
}