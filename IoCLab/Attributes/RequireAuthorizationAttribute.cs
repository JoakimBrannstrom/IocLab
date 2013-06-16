using System;

namespace IoCLab.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false)]
    public class RequireAuthorizationAttribute : Attribute
    {
        public readonly string Role;

        public RequireAuthorizationAttribute(string role)
        {
            Role = role;
        }
    }
}
