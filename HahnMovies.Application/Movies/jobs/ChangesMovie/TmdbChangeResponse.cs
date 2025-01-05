namespace HahnMovies.Application.Movies.Jobs.ChangesMovie
{
    public record TmdbChangeResponse
    {
        public int Page { get; init; }
        
        public int TotalPages { get; init; }
        
        public List<TmdbChangeResult> Results { get; init; } = null!;
    }
}