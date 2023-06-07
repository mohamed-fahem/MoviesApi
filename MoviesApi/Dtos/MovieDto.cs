using System.ComponentModel.DataAnnotations;

namespace MoviesApi.Dtos
{
    public class MovieDto
    {

        public string Title { get; set; }
        public int Year { get; set; }
        public double Rate { get; set; }
        
        public string Storeline { get; set; }
        public IFormFile? Poster { get; set; }

        public byte GenreId { get; set; }
    }
}
