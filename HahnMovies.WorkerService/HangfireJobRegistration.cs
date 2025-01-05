using HahnMovies.Application.Movies.jobs;
using HahnMovies.Application.Movies.Jobs;
using HahnMovies.Application.Movies.jobs.ChangesMovie;
using HahnMovies.Application.Movies.jobs.WeeklyFullSync;
using Hangfire;
using Hangfire.SqlServer;

namespace HahnMovies.WorkerService
{
    internal static class HangfireJobRegistration
    {
        internal static void RegisterGlobalRecurringJobs(IConfiguration configuration)
        {
            var tmdbWeeklyFullSyncCron = configuration[$"TmdbWeeklyFullSyncJob:Cron"];
            var tmdbChangesSyncCron = configuration[$"TmdbChangesMovieSyncJob:Cron"];
            var hangfireConnectionString = configuration.GetConnectionString("Application");

            JobStorage.Current = new SqlServerStorage(hangfireConnectionString);

            RecurringJob.AddOrUpdate<TmdbFullSyncJob>(nameof(TmdbFullSyncJob),
                job => job.ExecuteAsync(default),
                tmdbWeeklyFullSyncCron,
                new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });

            RecurringJob.AddOrUpdate<TmdbChangesMovieSyncJob>(
                nameof(TmdbChangesMovieSyncJob),
                job => job.ExecuteAsync(default),
                tmdbChangesSyncCron,
                new RecurringJobOptions { TimeZone = TimeZoneInfo.Local });
        }
    }
}