namespace HahnMovies.Application.Movies.jobs.ChangesMovie;

public class TmdbChangeResponse
{
    public int Page { get; set; }
    public int TotalPages { get; set; }
    public List<TmdbChangeResult> Results { get; set; }
}