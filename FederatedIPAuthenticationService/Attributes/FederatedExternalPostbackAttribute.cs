using System;
using System.Collections.Generic;
using System.Text;

namespace FederatedAuthNAuthZ.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class FederatedExternalPostbackAttribute : Attribute { }
}
