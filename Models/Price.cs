namespace FinalProject.Models
{
    public class Price
    {
        public int PriceID { get; set; }
        public string TicketType { get; set; } 
        public Decimal Cost { get; set; }
        public List<Schedule> Schedules { get; set; }
    }
}
