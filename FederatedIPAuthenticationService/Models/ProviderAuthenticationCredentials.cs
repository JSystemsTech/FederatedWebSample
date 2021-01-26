using FederatedIPAuthenticationService.Enums;
using FederatedIPAuthenticationService.Models.Collection;
using System;
using System.Collections.Generic;
using System.Text;

namespace FederatedIPAuthenticationService.Models
{
    public class ProviderAuthenticationCredentials
    {
		public string Username { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public Guid? TestUserGuid { get; set; }
		public string UserData { get; set; }
	}
}
