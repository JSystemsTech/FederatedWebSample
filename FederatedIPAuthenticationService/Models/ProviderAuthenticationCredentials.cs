using FederatedIPAuthenticationService.Enums;
using FederatedIPAuthenticationService.Models.Collection;
using System;
using System.Collections.Generic;
using System.Text;

namespace FederatedIPAuthenticationService.Models
{
    public class ProviderAuthenticationCredentials : CollectionModel<ProviderAuthenticationCredentialsOrdinal>
    {
		public string Username { get; private set; }
		public string Email { get; private set; }
		public string Password { get; private set; }
		public Guid? TestUserGuid { get; private set; }

		public ProviderAuthenticationCredentials(IEnumerable<object> data) : base(data) { }
		public static IEnumerable<object> BuildProviderAuthenticationCredentialsData(string username = null, string email = null, string password = null, Guid? testUserGuid = null)
			=> new object[] { username, email, password, testUserGuid };
		protected override void Init()
		{
			Username = GetValue(ProviderAuthenticationCredentialsOrdinal.Username);
			Email = GetValue(ProviderAuthenticationCredentialsOrdinal.Email);
			Password = GetValue(ProviderAuthenticationCredentialsOrdinal.Password);
			TestUserGuid = GetValue(ProviderAuthenticationCredentialsOrdinal.TestUserGuid) is string value && !string.IsNullOrWhiteSpace(value) ? Guid.Parse(value): default(Guid?);
		}
	}
}
