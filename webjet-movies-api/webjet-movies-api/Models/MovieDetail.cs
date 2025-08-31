namespace Webjet.Api.Models
{
    public class MovieDetail : Movie
    {
        public string Rated { get; set; } = string.Empty;
        public DateTime Released { get; set; }
        public string Runtime { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
        public string Director { get; set; } = string.Empty;
        public string Writer { get; set; } = string.Empty;
        public string Actors { get; set; } = string.Empty;
        public string Plot { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public double Metascore { get; set; }
        public double Rating { get; set; }
        public int Votes { get; set; }
        public decimal Price { get; set; }
    }
}
