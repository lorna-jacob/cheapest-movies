using Webjet.Api.Models;

namespace Webjet.Api.Services.Providers
{
    public interface IMovieProvider
    {
        string ProviderName { get; }
        Task<IEnumerable<Movie>> GetMoviesAsync(CancellationToken cancellationToken);
        Task<MovieDetail> GetMovieDetailAsync(string id, CancellationToken cancellationToken);
    }
}
