namespace FinalProject.Models
{
    public class Rating
    {
        public int RatingID { get; set; }
        public string MPAARating { get; set; }
        public List<Movie> Movies { get; set; }
    }
}
