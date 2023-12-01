using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FinalProject.Models
{
    //NOTE: This is the view model used to allow the user to login
    //The user only needs the email and password to login
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    //NOTE: This is the view model used to register a user
    //When the user registers, they only need to specify the
    //properties listed in this model
    public class RegisterViewModel
    {
        //NOTE: Here is the property for email
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        //NOTE: Here is the property for phone number
        [Required(ErrorMessage = "Phone number is required")]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [Display(Name = "First Name")]
        public String FirstName { get; set; }

//ADDED 11/27 HAVE NOT RESEEDED OR RESCAFFOLD 
        [Required(ErrorMessage = "Last name is required.")]
        [Display(Name = "Last Name")]
        public String LastName { get; set; }

        //ADDED 11/27
        [Required(ErrorMessage = "Date of birth is required.")]
        [Display(Name = "Date of Birth")]
        [ValidateAge(13)]
        public DateTime DateOfBirth { get; set; }

        //ADDED 11/27
        [Required(ErrorMessage = "Street is required.")]
        [Display(Name = "Street")]
        public string Street { get; set; }

        //ADDED 11/27
        [Required(ErrorMessage = "City is required.")]
        [Display(Name = "City")]
        public string City { get; set; }

        //ADDED 11/27
        [Required(ErrorMessage = "State is required.")]
        [Display(Name = "State")]
        public string State { get; set; }

        //ADDED 11/27
        [Required(ErrorMessage = "Zip is required.")]
        [Display(Name = "Zip")]
        public string Zip { get; set; }

        //NOTE: Here is the logic for putting in a password
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }


    public class ValidateAgeAttribute : ValidationAttribute
    {
        private readonly int _minimumAge;

        public ValidateAgeAttribute(int minimumAge)
        {
            _minimumAge = minimumAge;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime dateOfBirth)
            {
                var age = DateTime.Now.Year - dateOfBirth.Year;

                if (DateTime.Now < dateOfBirth.AddYears(age))
                {
                    age--;
                }

                if (age < _minimumAge)
                {
                    return new ValidationResult($"You must be at least {_minimumAge} years old to make an account.");
                }
            }

            return ValidationResult.Success;
        }

    }

    // DO I INHERIT??????
    public class AccountEditViewModel : RegisterViewModel
    {
        [Required(ErrorMessage = "Date of birth is required.")]
        [Display(Name = "Date of Birth")]
        [ValidateAge(13)]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Street is required.")]
        [Display(Name = "Street")]
        public string Street { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [Display(Name = "City")]
        public string City { get; set; }

        [Required(ErrorMessage = "State is required.")]
        [Display(Name = "State")]
        public string State { get; set; }

        [Required(ErrorMessage = "Zip is required.")]
        [Display(Name = "Zip")]
        public string Zip { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }

    //NOTE: This is the view model used to allow the user to 
    //change their password
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    //NOTE: This is the view model used to display basic user information
    //on the index page
    public class IndexViewModel
    {
        public bool HasPassword { get; set; }
        public String UserName { get; set; }
        public String Email { get; set; }
        public String UserID { get; set; }
        public String Phone { get; set; }
        public DateTime DateofBirth { get; set; }
        public String Street { get; set; }
        public String City { get; set; }
        public String State { get; set; }
        public String Zip { get; set; }
    }
}
