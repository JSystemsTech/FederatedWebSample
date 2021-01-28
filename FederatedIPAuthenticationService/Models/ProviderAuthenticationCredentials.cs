using FederatedAuthNAuthZ.Enums;
using FederatedAuthNAuthZ.Models.Collection;
using System;
using System.Collections.Generic;
using System.Text;

namespace FederatedAuthNAuthZ.Models
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
