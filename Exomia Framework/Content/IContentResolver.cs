using System.IO;

namespace Exomia.Framework.Content
{
    /// <summary>
    ///     A content resolver is in charge of locating a stream from an asset name.
    /// </summary>
    public interface IContentResolver
    {
        /// <summary>
        ///     Checks if the specified asset name exists.
        /// </summary>
        /// <param name="assetName">Name of the asset.</param>
        /// <returns><c>true</c> if the specified asset name exists, <c>false</c> otherwise</returns>
        bool Exists(string assetName);

        /// <summary>
        ///     Resolves the specified asset name to a stream.
        /// </summary>
        /// <param name="assetName">Name of the asset.</param>
        /// <returns>A Stream of the asset. This value can be null if this resolver was not able to locate the asset.</returns>
        Stream Resolve(string assetName);
    }
}