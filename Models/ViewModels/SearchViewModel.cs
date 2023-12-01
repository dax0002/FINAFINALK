using System;
using System.ComponentModel.DataAnnotations;

namespace FinalProject.Models
{
    public enum SearchType { GreaterThan, LessThan }

    public class SearchViewModel
	{
        [Display(Name = "Search by Title:")]
        public String SearchTitle { get; set; }

        [Display(Name = "Search by Tagline")]
        public String SearchTagline { get; set; }

        [Display(Name = "Search by Genre")]
        public Int32 SelectedGenre { get; set; }

        // Before & after? 
        [Display(Name = "Search by Release Year:")]
        //[DataType(DataType.Date)]
        //public DateTime?
        public Int32 SearchReleaseYear { get; set; }

        [Display(Name = "Search by Rating")]
        public String SearchRating { get; set; }

        // Specify <> 1.0-5.0 return avg rating 
        [Display(Name = "Search by Reviews:")]
        public String SearchReview { get; set; }

        [Display(Name = "Search Type:")]
        public SearchType SearchType { get; set; }

        [Display(Name = "Search by Actors")]
        public String SelectedActors { get; set; }
		
	}
}

