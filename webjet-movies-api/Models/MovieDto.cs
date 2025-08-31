namespace Webjet.Api.Models
{
    public class MovieDto
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int Year { get; set; }
        public decimal Price { get; set; }
        public string Provider { get; set; } = string.Empty;
        public bool IsFallback { get; set; }
        public DateTimeOffset RetrievedAt { get; set; }
    }
}