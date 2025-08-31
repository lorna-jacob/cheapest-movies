using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;
using Webjet.Api.Constants;
using Webjet.Api.Models;
using Webjet.Api.Models.Options;
using Webjet.Api.Services.Providers;
using Webjet.Api.Services.Providers.Dto;

namespace Webjet.Api.Services
{
    public class MoviesAggregatorService : IMovieAggregatorService
    {
        private readonly IEnumerable<IMovieProvider> _providers;
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheDuration;

        public MoviesAggregatorService(IEnumerable<IMovieProvider> providers, IMemoryCache cache, IOptions<CacheSettings> settings)
        {
            _providers = providers;
            _cache = cache;
            _cacheDuration = TimeSpan.FromMinutes(settings.Value.Minutes);
        }

        public async IAsyncEnumerable<ProviderStreamEvent> StreamMoviesAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            foreach (var provider in _providers)
            {
                var cacheKey = $"movies_{provider.ProviderName}";
                IEnumerable<Movie> movies = Enumerable.Empty<Movie>();

                try
                {
                    movies = await provider.GetMoviesAsync(cancellationToken);
                    _cache.Set(cacheKey, movies, _cacheDuration);
                }
                catch
                {
                    if (_cache.TryGetValue(cacheKey, out IEnumerable<Movie> cachedMovies))
                    {
                        movies = cachedMovies;
                    }
                }

                yield return new ProviderStreamEvent
                {
                    Provider = provider.ProviderName,
                    EventType = EventTypes.Movies,
                    Movies = movies
                };

                var detailTasks = movies.Select(async movie =>
                {
                    var cacheKeyDetail = $"movie_{provider.ProviderName}_{movie.Id}";
                    MovieDetail? detail = null;

                    try
                    {
                        detail = await provider.GetMovieDetailAsync(movie.Id, cancellationToken);
                        if (detail != null)
                            _cache.Set(cacheKeyDetail, detail, _cacheDuration);
                    }
                    catch
                    {
                        if (_cache.TryGetValue(cacheKeyDetail, out MovieDetail cachedDetail))
                        {
                            detail = cachedDetail;
                        }
                    }

                    if (detail != null)
                    {
                        return new ProviderStreamEvent
                        {
                            Provider = provider.ProviderName,
                            EventType = EventTypes.Detail,
                            Detail = detail
                        };
                    }
                    return null;
                });

                var detailResults = await Task.WhenAll(detailTasks);

                foreach (var ev in detailResults.Where(r => r != null))
                {
                    yield return ev!;
                }
            }
        }
    }
}