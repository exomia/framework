using System;
using System.IO;

namespace Exomia.Framework.Content
{
    /// <summary>
    ///     ContentReaderParameters struct
    /// </summary>
    public struct ContentReaderParameters
    {
        /// <summary>
        ///     Name of the asset currently loaded when using <see cref="IContentManager.Load{T}" />.
        /// </summary>
        public string AssetName;

        /// <summary>
        ///     Type of the asset currently loaded when using <see cref="IContentManager.Load{T}" />.
        /// </summary>
        public Type AssetType;

        /// <summary>
        ///     Stream of the asset to load.
        /// </summary>
        public Stream Stream;

        /// <summary>
        ///     This parameter is an out parameter for <see cref="IContentReader.ReadContent" />.
        ///     Set to true to let the ContentManager close the stream once the reader is done.
        /// </summary>
        public bool KeepStreamOpen;
    }
}