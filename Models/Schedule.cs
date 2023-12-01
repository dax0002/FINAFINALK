using FinalProject.Seeding;
using System.ComponentModel.DataAnnotations;

namespace FinalProject.Models
{
    public enum Theater
    {
        [Display(Name = "Theater One")] One,
        [Display(Name = "Theater Two")] Two
    }
    public enum Status
    {
        [Display(Name = "Cancelled")] Cancelled,
        [Display(Name = "Running")] Running,
        [Display(Name = "Finished")] Finished,
        [Display(Name = "Planned")] Planned
    }
    public class Schedule
    {
        public int ScheduleID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime
        {
            get { return StartTime.AddMinutes(Movie.Runtime); }
        } // Calculated from movie's runtime
        public Theater Theater { get; set; }
        public Status Status { get; set; } 
        public Boolean SpecialEvent { get; set; }
        public decimal ticketprice
        {
            get { return Price.Cost; }
        }

        // Navigation property for the many-to-one relationship with Movies
        public Movie Movie { get; set; }
        public Price Price { get; set; }
        public List<TransactionDetail> TransactionDetails { get; set; }
        public Schedule()
        {
            TransactionDetails ??= new List<TransactionDetail>();
        }
    }

}
