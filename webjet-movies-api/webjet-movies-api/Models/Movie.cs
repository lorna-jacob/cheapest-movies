namespace Webjet.Api.Models
{
    public class Movie
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int Year { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Poster { get; set; } = string.Empty;
    }
}
