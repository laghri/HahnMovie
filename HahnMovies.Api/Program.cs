using HahnMovies.Application.Common;
using HahnMovies.Application.Movies.Commands.SyncMovies;
using HahnMovies.Infrastructure;
using HahnMovies.Infrastructure.Data;
using HahnMovies.Infrastructure.Repositories;
using HahnMovies.Infrastructure.Services;
using Hangfire;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.RegisterDataServices(builder.Configuration);
builder.Services.AddMediatR(typeof(SyncMoviesCommandHandler).Assembly);
builder.Services.AddControllers(); 
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
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

