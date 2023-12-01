using FinalProject.Models;
using System.ComponentModel.DataAnnotations;

namespace FinalProject.Models
{
    public enum TransactionStatus
    {
        Purchased,
        Cancelled
    }
    public class Transaction
    {
        private const Decimal TAX_RATE = 0.0825m;
        public int TransactionID { get; set; }

        [Display(Name = "Transaction Number")]
        public int TransactionNumber { get; set; }

        [Display(Name = "Transaction Date")]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime TransactionDate { get; set; }

        [Display(Name = "Order Subtotal")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public decimal Subtotal 
        {
            get { return TransactionDetails.Sum(rd => rd.TicketPrice); }
        }

        [Display(Name = "Order Tax")]
        [DisplayFormat(DataFormatString ="{0:C}")]
        public Decimal OrderTax
        {
            get { return Subtotal * TAX_RATE; }
        }

        [Display(Name = "Order Total")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Decimal OrderTotal
        {
            get { return Subtotal + OrderTax; }
        }
        public TransactionStatus TransactionStatus { get; set; }   
        // Other transaction properties
        public Boolean GiftTicket { get; set; }
        public String? Recipient { get; set; }
        // Navigation property for many-to-many relationship
        public AppUser User { get; set; }
        public List<TransactionDetail> TransactionDetails { get; set; }
        public Transaction()
        {
            if (TransactionDetails == null)
            {
                TransactionDetails = new List<TransactionDetail>();
            }
        }
    }
}
