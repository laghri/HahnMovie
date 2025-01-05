using HahnMovies.Application.Movies.jobs;
using HahnMovies.Application.Movies.jobs.ChangesMovie;
using HahnMovies.Application.Movies.jobs.WeeklyFullSync;
using HahnMovies.WorkerService;
using HahnMovies.Infrastructure;
using HahnMovies.Infrastructure.Data;
using MediatR;
using Hangfire;

var builder = Host.CreateApplicationBuilder(args);

// Register database and data services
builder.Services.RegisterDataServices(builder.Configuration);

builder.Services.AddMediatR(typeof(AppDbContext).Assembly);

// Add Hangfire
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("Application")));
builder.Services.AddHangfireServer();
builder.Services.Configure<TmdbWeeklyFullSyncJobConfiguration>(
    builder.Configuration.GetSection(TmdbWeeklyFullSyncJobConfiguration.SettingsIdentifier));
builder.Services.Configure<TmdbChangesMovieSyncJobConfiguration>(
    builder.Configuration.GetSection(TmdbChangesMovieSyncJobConfiguration.SettingsIdentifier));
HangfireJobRegistration.RegisterGlobalRecurringJobs(builder.Configuration);

// Register services;

// Add Hosted Service (Worker)
builder.Services.AddHostedService<Worker>();

var host = builder.Build();

// Schedule jobs

host.Run();