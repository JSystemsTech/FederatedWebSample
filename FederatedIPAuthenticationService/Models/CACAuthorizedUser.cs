using FederatedIPAuthenticationService.Enums;
using FederatedIPAuthenticationService.Models.Collection;
using System;
using System.Collections.Generic;
using System.Text;

namespace FederatedIPAuthenticationService.Models
{
    public class CACAuthorizedUser: CollectionModel<CACAuthorizedUserOrdinal>
    {
		public string Token { get; private set; }
		public string EDIPI { get; private set; }
		/* TODO: fill in other values later*/

		public CACAuthorizedUser(IEnumerable<object> data) : base(data) { }
		protected override void Init()
		{
            Token = GetValue(CACAuthorizedUserOrdinal.Token);
			EDIPI = GetValue(CACAuthorizedUserOrdinal.EDIPI);
		}
	}
}
