﻿using System;
using System.Security.Principal;

namespace FederatedAuthNAuthZ.Attributes.Common
{
    public class CommonIdentity : IIdentity
    {
        private static string AuthenticationTypeName => "FederatedIPAPIToken";
        public string Name => $"{FirstName} {MiddleInitial} {LastName}";
        public string AuthenticationType => AuthenticationTypeName;
        public bool IsAuthenticated { get; private set; }

        internal string FirstName { get; private set; }
        internal string MiddleInitial { get; private set; }
        internal string LastName { get; private set; }
        internal Guid Guid { get; private set; }

        internal void Update(string firstName, string middleInitial, string lastName)
        {
            FirstName = firstName;
            MiddleInitial = middleInitial; 
            LastName = lastName;
        }
        private CommonIdentity() { }
        internal static CommonIdentity Create(Guid guid, string firstName, string middleInitial, string lastName)
            => new CommonIdentity
            { 
                FirstName = firstName,
                MiddleInitial = middleInitial, 
                LastName = lastName, 
                Guid = guid, 
                IsAuthenticated = true };
        internal static CommonIdentity CreateLogout()
            => new CommonIdentity
            {
                IsAuthenticated = false
            };

    }
}