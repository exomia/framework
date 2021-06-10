using System;

namespace Exomia.Framework.Core.IOC.Attributes
{
    /// <summary> Attribute for ioc use default. This class cannot be inherited. </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class IoCUseDefaultAttribute : Attribute
    {
        /// <summary> Gets the default value. </summary>
        /// <value> The default value or null. </value>
        public object? DefaultValue { get; }

        /// <summary> Initializes a new instance of the <see cref="IoCUseDefaultAttribute"/> class. </summary>
        /// <param name="defaultValue"> (Optional) The default value. </param>
        public IoCUseDefaultAttribute(object? defaultValue = null)
        {
            DefaultValue = defaultValue;
        }
    }
}
