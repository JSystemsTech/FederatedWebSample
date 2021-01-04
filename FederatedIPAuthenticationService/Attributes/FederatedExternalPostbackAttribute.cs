using System;
using System.Collections.Generic;
using System.Text;

namespace FederatedIPAuthenticationService.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class FederatedExternalPostbackAttribute : Attribute { }
}
