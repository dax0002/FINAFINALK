using FinalProject.Seeding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FinalProject.Models
{
    public class Movie
    {
        public int MovieID { get; set; }
        [Display(Name = "Movie Title")]
        public string MovieTitle { get; set; }
        public string Description { get; set; }
        public string Tagline { get; set; }
        [Display(Name = "Release Year")]
        public Int32 ReleaseYear { get; set; }
        public Int32 Runtime { get; set; }
        public string Actors { get; set; }

        // Navigation property for the many-to-one relationship with Genre
        public Genre Genre { get; set; }
        public Rating Rating { get; set; } 

        // Navigation property for the one-to-many relationship with Schedule
        public List<Schedule> Schedules { get; set; }
        public List<Review> Reviews { get; set; }

        public Movie()
        {
            Reviews ??= new List<Review>();
            Schedules ??= new List<Schedule>();
        }
    }


}
