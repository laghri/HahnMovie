namespace HahnMovies.Application.Movies.Commands.Comments
{
    public record AddMovieCommentRequest
    {
        public string Username { get; init; } = string.Empty;
        
        public string Comment { get; init; } = string.Empty;
    }
}