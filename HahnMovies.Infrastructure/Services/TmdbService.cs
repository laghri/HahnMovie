using System.IO.Compression;
using System.Net.Http.Headers;
using System.Text.Json;
using HahnMovies.Application.Common;
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
            return await Task.WhenAll(tasks);
        }

        private async Task<Movie> GetSingleMovieAsync(int movieId, CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(
                $"movie/{movieId}?api_key={_apiKey}",
                cancellationToken);
    
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var movieData = JsonSerializer.Deserialize<JsonElement>(content);

            // Use TryGetProperty to check for the existence of properties
            if (movieData.TryGetProperty("title", out var titleElement) &&
                movieData.TryGetProperty("release_date", out var releaseDateElement) &&
                movieData.TryGetProperty("vote_average", out var voteAverageElement) &&
                movieData.TryGetProperty("poster_path", out var posterPathElement))
                return Movie.Create(
                    movieId,
                    titleElement.GetString()!,
                    DateTime.Parse(releaseDateElement.GetString()!),
                    voteAverageElement.GetDouble(),
                    posterPathElement.GetString()!
                );
            _logger.LogError($"Missing required properties for movie ID {movieId}");
            throw new KeyNotFoundException($"Required properties are missing for movie ID {movieId}");

        }

    }