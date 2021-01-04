using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace FederatedIPAPIConsumer.Models
{
    public class RegisterFormVM
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName {get; set;}
        [Display(Name ="Middle Name")]
        public string MiddleName { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}