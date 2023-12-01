using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

//TODO: Make this namespace match your project name
namespace FinalProject.Models
{
    public class AppUser : IdentityUser
    {
        //TODO: Add custom user fields - first name is included as an example
        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name is required")]
        public String FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last Name is required")]
        public String LastName { get; set; }

        [Display(Name = "Date Of Birth")]
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}")]
        public DateTime? DateofBirth { get; set; }

        [Display(Name = "Street")]
        public String? Street { get; set; }

        [Display(Name = "City")]
        public String? City { get; set; }

        [Display(Name = "State")]
        public String? State { get; set; }

        [Display(Name = "Zip")]
        public String? Zip { get; set; }

        [Display(Name = "Popcorn Points")]
        public Int32? PopcornPoints { get; set; }

        [Display(Name = "Fired")]
        public Boolean Fired { get; set; }

        //Navigational Properties
        public List<Review> Reviews { get; set; }
        public List<Transaction> Transactions { get; set; }
        public AppUser()
        {
            if (Reviews == null)
            {
                Reviews = new List<Review>();
            }
            if (Transactions == null)
            {
                Transactions = new List<Transaction>();
            }

        }

    }
}
