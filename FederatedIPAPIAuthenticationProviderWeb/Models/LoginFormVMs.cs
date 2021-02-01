using FederatedAuthNAuthZ.Web.ConsumerAPI;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace FederatedIPAPIAuthenticationProviderWeb.Models
{
    public class LoginModeFormVM {
        public virtual void Validate(ModelStateDictionary modelState) { }
        [Required]
        public string Mode { get; set; }
    }

    public class UsernamePasswordVM : LoginModeFormVM
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
    public class EmailPasswordVM : LoginModeFormVM
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
    public class TestUserVM : LoginModeFormVM
    {
        [Required]
        public string TestUserId { get; set; }
        public IEnumerable<ApplicationUser> TestUsers { get; set; }
    }
}