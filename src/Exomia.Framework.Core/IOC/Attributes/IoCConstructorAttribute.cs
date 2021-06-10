using System;

namespace Exomia.Framework.Core.IOC.Attributes
{
    /// <summary> Attribute for ioc constructor. This class cannot be inherited. </summary>
    [AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
    public sealed class IoCConstructorAttribute : Attribute
    {
    }
}
