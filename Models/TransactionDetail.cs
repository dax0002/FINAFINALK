using FinalProject.Controllers;
using FinalProject.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProject.Models
{
    public enum PaymentMethod
    {
        CashorCard,
        PopcornPoints
    }
    public class TransactionDetail
    {
        public int TransactionDetailID { get; set; }
        public int TransactionNumber { get; set; }
        public string Seat { get; set; }
        public PaymentMethod PaymentMethod { get; set; }

        [NotMapped] // This property won't be mapped to the database
        public int SelectedMovieID { get; set; }
        public decimal TicketPrice
        {
            get {
                if (Schedule != null)
                {
                    return Schedule.ticketprice;
                }
                return 0;
            }
        }
        public Int32 PointChange
        {
            get
            {
                // Assuming there is a valid relationship between TransactionDetails, Schedule, and Price
                // Calculate popcorn points based on the associated price
                if (Schedule != null && Schedule.Price != null)
                {
                    decimal price = Schedule.Price.Cost; // Accessing the Price property from the related Schedule
                                                           // Assuming each dollar spent equals one popcorn point
                    int popcornPoints = (int)price;

                    return popcornPoints;
                }

                return 0; // Default value if the relationship is not set or data is missing
            }
        }


        public Transaction Transaction { get; set; }   
        public Schedule Schedule { get; set; } 

    }
}
