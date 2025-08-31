using Webjet.Api.Services.Providers.Dto;

namespace Webjet.Api.Services
{
    public interface IMovieAggregatorService
    {
        IAsyncEnumerable<ProviderStreamEvent> StreamMoviesAsync(CancellationToken cancellationToken);
    }
}
