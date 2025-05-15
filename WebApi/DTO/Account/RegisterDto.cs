using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace WebApi.DTO.Account
{
    public class RegisterDto
    {
        [Required]
        [StringLength(20, MinimumLength =3, ErrorMessage ="First Name should have more than 03 characters")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Last Name should have more than 03 characters")]
        public string LastName { get; set; }
       // public string Address { get; set; }
       // [Required]
       // public string Role { get; set; }

        [Required]
        [RegularExpression("^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$", ErrorMessage ="Invalid Email Address")]
        public string Email { get; set; }

        [Required]
        [StringLength(12, MinimumLength = 8, ErrorMessage ="Password should contain atleast 8 characters")]
        public string Password { get; set; }
    }
}
