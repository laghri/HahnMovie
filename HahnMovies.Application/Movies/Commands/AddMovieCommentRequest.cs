namespace HahnMovies.Application.Movies.Commands;

public class AddMovieCommentRequest
{
    public string Username { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
}