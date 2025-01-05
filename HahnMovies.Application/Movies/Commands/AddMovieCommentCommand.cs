using MediatR;

namespace HahnMovies.Application.Movies.Commands;

public class AddMovieCommentCommand : IRequest
{
    public int MovieId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
}