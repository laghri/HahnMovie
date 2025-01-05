using HahnMovies.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HahnMovies.Infrastructure.Data.Config
{
    public class MovieCommentConfig : IEntityTypeConfiguration<MovieComment>
    {
        public void Configure(EntityTypeBuilder<MovieComment> builder)
        {
            builder.HasKey(mc => mc.Id);
            builder.Property(mc => mc.Id)
                .ValueGeneratedOnAdd();

            builder.Property(mc => mc.MovieId)
                .IsRequired();

            builder.Property(mc => mc.Username)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(mc => mc.Comment)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(mc => mc.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(mc => mc.Movie)
                .WithMany(m => m.MovieComments)
                .HasForeignKey(mc => mc.MovieId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
