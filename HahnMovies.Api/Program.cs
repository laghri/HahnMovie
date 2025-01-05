using HahnMovies.Application.Common;
using HahnMovies.Application.Movies.Commands;
using HahnMovies.Application.Movies.Commands.SyncMovies;
using HahnMovies.Application.Movies.Queries;
using HahnMovies.Infrastructure;
using HahnMovies.Infrastructure.Data;
using HahnMovies.Infrastructure.Repositories;
using HahnMovies.Infrastructure.Services;
using Hangfire;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.RegisterDataServices(builder.Configuration);
builder.Services.AddMediatR(typeof(SyncMoviesCommandHandler).Assembly);
builder.Services.AddMediatR(typeof(RateMovieCommandHandler).Assembly);
builder.Services.AddMediatR(typeof(AddMovieCommentCommandHandler).Assembly);
builder.Services.AddMediatR(typeof(SearchMoviesQueryHandler));
builder.Services.AddControllers(); 
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<IMovieCommentRepository, MovieCommentRepository>();
builder.Services.AddHttpClient<ITmdbService, TmdbService>();

builder.Services.AddHangfire(config =>
{
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("Application"));
});

builder.Services.AddHangfireServer();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.UseHangfireDashboard("/hangfire");
app.Run();

