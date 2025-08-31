namespace Webjet.Api.Services.Providers.Dto
{
    public class MovieSummary
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Year { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Poster { get; set; } = string.Empty;
    }

    public class MoviesResponse
    {
        public IEnumerable<MovieSummary> Movies { get; set; } = Enumerable.Empty<MovieSummary>();
    }
}
