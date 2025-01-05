namespace HahnMovies.Application.Movies.jobs.ChangesMovie;

public class TmdbChangesMovieSyncJobConfiguration
{
    public const string SettingsIdentifier = "TmdbChangesMovieSyncJob";

    public string Cron { get; init; } = string.Empty;
   
}