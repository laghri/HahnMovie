namespace HahnMovies.Domain.Models;

public class MovieComment
{
    public int Id { get; init; }
    public int MovieId { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Comment { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    
    public Movie Movie { get; init; } = null!;
}