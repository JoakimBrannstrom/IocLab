using System;

namespace IoCLab.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = false)]
    public class RequireAuthenticationAttribute : Attribute
    {
    }
}
