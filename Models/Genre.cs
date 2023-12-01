namespace FinalProject.Models
{
    public class Genre
    {
        public int GenreID { get; set; }
        public string GenreTitle { get; set; }
        public List<Movie> Movies { get; set; }
    }
}
