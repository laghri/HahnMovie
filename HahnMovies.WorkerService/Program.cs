using HahnMovies.WorkerService;
using HahnMovies.Infrastructure;
using HahnMovies.Infrastructure.Data;
using MediatR;
using Hangfire;

var builder = Host.CreateApplicationBuilder(args);

// Register database and data services
builder.Services.RegisterDataServices(builder.Configuration);

// Add MediatR (if needed)
builder.Services.AddMediatR(typeof(AppDbContext).Assembly);

// Add Hangfire
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("Application")));
builder.Services.AddHangfireServer();

// Register services;

// Add Hosted Service (Worker)
builder.Services.AddHostedService<Worker>();

var host = builder.Build();

// Schedule jobs

host.Run();