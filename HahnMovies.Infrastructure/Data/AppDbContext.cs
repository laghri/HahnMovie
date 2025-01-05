using HahnMovies.Application.Common.Interfaces;
using HahnMovies.Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HahnMovies.Infrastructure.Data;


public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<MovieComment> MovieComments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}