using MediatR;

namespace HahnMovies.Application.Movies.Commands.Comments;

public class AddMovieCommentCommand : IRequest
{
    public int MovieId { get; init; }
    
    public string Username { get; init; } = string.Empty;
    
    public string Comment { get; init; } = string.Empty;
}