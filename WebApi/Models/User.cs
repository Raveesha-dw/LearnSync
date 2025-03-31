using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace WebApi.Models
{
    public class User : IdentityUser
    {
        [Required]
        public string FirstName{ get; set; }
        [Required]
        public string LastName{ get; set; }

        public DateTime CreatedDate{ get; set; } = DateTime.UtcNow;

    }
}
