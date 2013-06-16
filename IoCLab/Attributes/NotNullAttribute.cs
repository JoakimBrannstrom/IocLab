using System;

namespace IoCLab.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class NotNullAttribute : Attribute
    {
    }
}