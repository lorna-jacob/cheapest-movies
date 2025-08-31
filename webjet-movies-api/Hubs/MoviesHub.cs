using Microsoft.AspNetCore.SignalR;
using System.Runtime.CompilerServices;
using Webjet.Api.Services;
using Webjet.Api.Services.Providers.Dto;

namespace Webjet.Api.Hubs
{
    public class MoviesHub : Hub
    {
        private readonly IMovieAggregatorService _stream;

        public MoviesHub(IMovieAggregatorService stream)
        {
            _stream = stream;
        }

        public async IAsyncEnumerable<ProviderStreamEvent> StreamMovies([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            await foreach (var ev in _stream.StreamMoviesAsync(cancellationToken))
            {
                yield return ev;
            }
        }
    }
}
