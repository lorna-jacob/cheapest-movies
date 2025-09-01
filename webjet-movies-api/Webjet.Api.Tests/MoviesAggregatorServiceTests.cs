using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using Webjet.Api.Models;
using Webjet.Api.Models.Options;
using Webjet.Api.Services;
using Webjet.Api.Services.Providers;
using Webjet.Api.Services.Providers.Dto;

namespace Webjet.Api.Tests
{
    public class MoviesAggregatorServiceTests
    {
        [Fact]
        public async Task StreamMoviesAsync_YieldsMoviesAndDetails()
        {
            var provider = new Mock<IMovieProvider>();
            provider.Setup(p => p.ProviderName).Returns("FakeProvider");
            provider.Setup(p => p.GetMoviesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Movie>
            {
                new Movie { Id = "f001", Title = "Fake Movie", Year = 2020 }
            });
            provider.Setup(p => p.GetMovieDetailAsync("f001", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MovieDetail
            {
                Id = "f001",
                Title = "Fake Movie",
                Year = 2020,
                Price = 42m
            });

            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var cacheSettings = Options.Create(new CacheSettings
            {
                Minutes = 1
            });

            var sut = new MoviesAggregatorService(new[] { provider.Object }, memoryCache, cacheSettings);

            var seen = new List<ProviderStreamEvent>();
            await foreach (var ev in sut.StreamMoviesAsync(CancellationToken.None))
            {
                seen.Add(ev);
            }

            Assert.Contains(seen, e => e.EventType == "movies");
            Assert.Contains(seen, e => e.EventType == "detail" && e.Detail?.Price == 42m);
        }
    }
}