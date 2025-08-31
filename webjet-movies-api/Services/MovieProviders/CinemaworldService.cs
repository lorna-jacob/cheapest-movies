using Microsoft.Extensions.Options;
using Webjet.Api.Constants;
using Webjet.Api.Models;
using Webjet.Api.Models.Options;
using Webjet.Api.Services.Providers.Dto;

namespace Webjet.Api.Services.Providers
{
    public class CinemaworldService : IMovieProvider
    {
        private readonly HttpClient _http;

        public string ProviderName => ProviderNames.Cinemaworld;

        public CinemaworldService(IHttpClientFactory httpFactory, IOptions<WebjetSettings> settings)
        {
            _http = httpFactory.CreateClient(ProviderNames.Cinemaworld);
        }

        public async Task<IEnumerable<Movie>> GetMoviesAsync(CancellationToken cancellationToken)
        {
            var response = await _http.GetAsync("movies", cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<MoviesResponse>(cancellationToken: cancellationToken);

            return result?.Movies?.Select(m => new Movie
            {
                Id = m.Id,
                Title = m.Title,
                Year = int.TryParse(m.Year, out var year) ? year : 0,
                Type = m.Type
            }) ?? Enumerable.Empty<Movie>();
        }

        public async Task<MovieDetail> GetMovieDetailAsync(string id, CancellationToken cancellationToken)
        {
            var response = await _http.GetAsync($"movie/{id}", cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<MovieDetailResponse>(cancellationToken: cancellationToken);

            return new MovieDetail
            {
                Id = result!.Id,
                Title = result.Title,
                Year = int.TryParse(result.Year, out var year) ? year : 0,
                Rated = result.Rated,
                Released = DateTime.TryParse(result.Released, out var released) ? released : default,
                Runtime = result.Runtime,
                Genre = result.Genre,
                Director = result.Director,
                Writer = result.Writer,
                Actors = result.Actors,
                Plot = result.Plot,
                Language = result.Language,
                Country = result.Country,
                Metascore = double.TryParse(result.Metascore, out var metascore) ? metascore : 0,
                Rating = double.TryParse(result.Rating, out var rating) ? rating : 0,
                Votes = int.TryParse(result.Votes?.Replace(",", ""), out var votes) ? votes : 0,
                Price = decimal.TryParse(result.Price, out var price) ? price : 0
            };
        }
    }
}
