using HahnMovies.Application.Common;
using HahnMovies.Domain.Models;
using MediatR;

namespace HahnMovies.Application.Movies.Commands.Comments;

public class AddMovieCommentCommandHandler(IMovieCommentRepository movieCommentRepository)
    : IRequestHandler<AddMovieCommentCommand>
{
    public async Task<Unit> Handle(AddMovieCommentCommand request, CancellationToken cancellationToken)
    {
        var movieComment = new MovieComment
        {
            MovieId = request.MovieId,
            Username = request.Username,
            Comment = request.Comment,
            CreatedAt = DateTime.UtcNow
        };

        await movieCommentRepository.AddCommentAsync(movieComment, cancellationToken);

        return Unit.Value;
    }
}