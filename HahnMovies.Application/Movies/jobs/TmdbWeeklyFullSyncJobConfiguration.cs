namespace HahnMovies.Application.Movies.jobs;

public class TmdbWeeklyFullSyncJobConfiguration
{
    public const string SettingsIdentifier = "TmdbWeeklyFullSyncJob";

    public string Cron { get; init; } = string.Empty;
}