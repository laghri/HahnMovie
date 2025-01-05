using HahnMovies.Domain.Models;
using MediatR;

namespace HahnMovies.Application.Movies.Queries;

public class SearchMoviesQuery : IRequest<IEnumerable<Movie>>
{
    public string Title { get; init; } = string.Empty;
}