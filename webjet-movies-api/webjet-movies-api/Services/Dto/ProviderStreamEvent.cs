using Webjet.Api.Models;

namespace Webjet.Api.Services.Providers.Dto
{
    public class ProviderStreamEvent
    {
        public string Provider { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public IEnumerable<Movie>? Movies { get; set; }
        public MovieDetail? Detail { get; set; }
    }
}
