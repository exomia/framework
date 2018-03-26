namespace Exomia.Framework
{
    /// <summary>
    ///     An interface to clone a object.
    /// </summary>
    public interface ICloneable
    {
        /// <summary>
        ///     returns a deep copy of the object
        /// </summary>
        /// <returns>a new deep copied object</returns>
        object Clone();
    }
}