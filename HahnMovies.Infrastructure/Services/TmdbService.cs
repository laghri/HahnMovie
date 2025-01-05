using System.IO.Compression;
using System.Net.Http.Headers;
using System.Text.Json;
using HahnMovies.Application.Common;
using HahnMovies.Application.Movies.jobs.ChangesMovie;
using HahnMovies.Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HahnMovies.Infrastructure.Services;

public class TmdbService : ITmdbService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly ILogger<TmdbService> _logger;

    public TmdbService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<TmdbService> logger)
    {
        _httpClient = httpClient;
        _apiKey = configuration["Tmdb:ApiKey"]!;
        _logger = logger;

        _httpClient.BaseAddress = new Uri("https://api.themoviedb.org/3/");
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", configuration["Tmdb:AccessToken"]);
    }

    public async Task<IEnumerable<int>> GetAllMovieIdsAsync(CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow;
        var exportUrl = $"http://files.tmdb.org/p/exports/movie_ids_{today:MM_dd_yyyy}.json.gz";

        using var response = await _httpClient.GetAsync(exportUrl, cancellationToken);
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        await using var gzipStream = new GZipStream(stream, CompressionMode.Decompress);
        using var reader = new StreamReader(gzipStream);

        var movieIds = new List<int>();
        while (await reader.ReadLineAsync(cancellationToken) is { } line)
        {
            var movieData = JsonSerializer.Deserialize<JsonElement>(line);
            movieIds.Add(movieData.GetProperty("id").GetInt32());
        }

        return movieIds;
    }

    public async Task<IEnumerable<Movie>> GetMovieDetailsAsync(
        IEnumerable<int> movieIds,
        CancellationToken cancellationToken)
    {
        var tasks = movieIds.Select(id => GetSingleMovieAsync(id, cancellationToken));
        var movieDetails = await Task.WhenAll(tasks);
        return movieDetails.Where(movie => movie != null)!;
    }

    public async Task<IEnumerable<int>> GetUpdatedMovieIdsAsync(CancellationToken cancellationToken)
    {
        var updatedMovies = new List<int>();
        var page = 1;
        var continueFetching = true;
            
        while (continueFetching)
        {
            var response = await _httpClient.GetAsync(
                $"https://api.themoviedb.org/3/movie/changes?page={page}&api_key=YOUR_API_KEY", cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error fetching movie changes from TMDB.");
            }
            
            var data = await JsonSerializer.DeserializeAsync<TmdbChangeResponse>(
                await response.Content.ReadAsStreamAsync(cancellationToken), cancellationToken: cancellationToken);

            if (data?.Results != null)
            {
                updatedMovies.AddRange(data.Results.Select(r => r.Id));
            }
            
            if (data != null) continueFetching = data.Page < data.TotalPages;
            page++;
        }

        return updatedMovies;
    }
    
    private async Task<Movie?> GetSingleMovieAsync(int movieId, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(
            $"movie/{movieId}?api_key={_apiKey}",
            cancellationToken);

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var movieData = JsonSerializer.Deserialize<JsonElement>(content);

        if (movieData.TryGetProperty("title", out var titleElement) &&
            movieData.TryGetProperty("release_date", out var releaseDateElement) &&
            movieData.TryGetProperty("vote_average", out var voteAverageElement) &&
            movieData.TryGetProperty("poster_path", out var posterPathElement))
        {
            if (string.IsNullOrWhiteSpace(releaseDateElement.GetString()))
            {
                _logger.LogWarning(
                    $"Invalid or missing release_date for movie ID {movieId}. Setting release date to default.");
                return Movie.Create(
                    movieId,
                    titleElement.GetString()!,
                    DateTime.MinValue,
                    voteAverageElement.GetDouble(),
                    posterPathElement.GetString()!
                );
            }

            if (DateTime.TryParse(releaseDateElement.GetString(), out var releaseDate))
            {
                return Movie.Create(
                    movieId,
                    titleElement.GetString()!,
                    releaseDate,
                    voteAverageElement.GetDouble(),
                    posterPathElement.GetString()!
                );
            }

            _logger.LogWarning($"Invalid release_date format for movie ID {movieId}. Skipping movie.");
            return null!;
        }

        _logger.LogError($"Missing required properties for movie ID {movieId}");
        return null;
    }
}