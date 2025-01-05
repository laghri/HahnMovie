namespace HahnMovies.Domain.Models;

public class Movie
{
 public int Id { get; init; }
 public string Title { get; private set; }
 public DateTime ReleaseDate { get; private set; }
 public double VoteAverage { get; private set; }
 public string PosterPath { get; private set; }
 public DateTime LastUpdated { get; private set; }
 public int HahnVoteCount { get; private set; } = 0; 
 public double HahnVoteAverage { get; private set; } = 0.0;
 
 public ICollection<MovieComment> MovieComments { get; set; } = new List<MovieComment>();

 private Movie() { }

 public static Movie? Create(
  int id, 
  string title, 
  DateTime releaseDate, 
  double voteAverage, 
  string posterPath)
 {
  return new Movie
  {
   Id = id,
   Title = title,
   ReleaseDate = releaseDate,
   VoteAverage = voteAverage,  // TMDB vote average, for reference only
   PosterPath = posterPath,
   LastUpdated = DateTime.UtcNow
  };
 }

 public void Update(
  string title, 
  DateTime releaseDate, 
  double voteAverage, 
  string posterPath)
 {
  Title = title;
  ReleaseDate = releaseDate;
  VoteAverage = voteAverage;
  PosterPath = posterPath;
  LastUpdated = DateTime.UtcNow;
 }
 
 public void RateMovie(int rating)
 {
  if (rating < 1 || rating > 10)
  {
   throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 1 and 10.");
  }

  HahnVoteCount++;
  HahnVoteAverage = ((HahnVoteAverage * (HahnVoteCount - 1)) + rating) / HahnVoteCount;
 }
}
