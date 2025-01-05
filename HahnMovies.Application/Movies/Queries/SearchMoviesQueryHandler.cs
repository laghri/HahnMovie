using HahnMovies.Application.Common;
using HahnMovies.Domain.Models;
using MediatR;

namespace HahnMovies.Application.Movies.Queries;

public class SearchMoviesQueryHandler(IMovieRepository movieRepository)
    : IRequestHandler<SearchMoviesQuery, IEnumerable<Movie>>
{
    public async Task<IEnumerable<Movie>> Handle(SearchMoviesQuery request, CancellationToken cancellationToken)
    {
        return await movieRepository.SearchMoviesAsync(request.Title, cancellationToken);
    }
}
