using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TRMDesktopUI.Library.Models
{
    public class CreateUserModel
    {
        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        [Required]
        public string LastName { get; set; }

        [DisplayName("Email Address")]
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        public string Password { get; set; }

        [DisplayName("Confirm Password")]
        [Required]
        [Compare(nameof(Password), ErrorMessage = "The passwords must match")]
        public string ConfirmPassword { get; set; }
    }
}