namespace FinalProject.Models
{
    public enum ReviewStatus
    {
        Approved,
        NeedsReview,
        Rejected
    }
    public class Review
    {
        public int ReviewID { get; set; }
        public int Rating { get; set; }
        public string? Description { get; set; }
        public ReviewStatus ReviewStatus { get; set; }  
        // Foreign key for the many-to-one relationship with AppUser

        // Navigation property for the many-to-one relationship with AppUser
        public AppUser AppUser { get; set; }

        // Foreign key for the many-to-one relationship with Movies

        // Navigation property for the many-to-one relationship with Movies
        public Movie Movie { get; set; }
    }

}
