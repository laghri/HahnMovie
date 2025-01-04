namespace HahnMovies.Domain.Models;

public class Movie
{
 public int Id { get; init; }
 public string Title { get; private set; }
 public DateTime ReleaseDate { get; private set; }
 public double VoteAverage { get; private set; }
 public string PosterPath { get; private set; }
 public DateTime LastUpdated { get; private set; }

 private Movie() { }

 public static Movie Create(
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
   VoteAverage = voteAverage,
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
}