using HahnMovies.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HahnMovies.Infrastructure.Data.Config;

public class MoviesConfig : IEntityTypeConfiguration<Movie>
{
  
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            
            builder.HasKey(m => m.Id);
            
            builder.Property(m => m.Id)
                .ValueGeneratedNever();
            
            builder.Property(m => m.Title)
                .IsRequired(false)
                .HasMaxLength(200);

            builder.Property(m => m.PosterPath)
                .HasMaxLength(100)
                .IsRequired(false);
        }
    }
