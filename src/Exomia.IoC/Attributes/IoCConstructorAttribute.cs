using System;

namespace Exomia.IoC.Attributes
{
    /// <summary> Attribute for ioc constructor. This class cannot be inherited. </summary>
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    public sealed class IoCConstructorAttribute : Attribute
    {
    }
}
