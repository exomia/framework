#region License

// Copyright (c) 2018-2020, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.IO;
using Exomia.Framework.Content;
using SharpDX;
using SharpDX.Direct3D11;

namespace Exomia.Framework.Graphics
{
    /// <summary>
    ///     A default textures.
    /// </summary>
    public static class DefaultTextures
    {
        private const string WHITE_TEXTURE_BASE64 =
            "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+ip1sAAAAASUVORK5CYII=";

        private static Texture? s_whiteTexture;
        private static Texture2? s_whiteTexture2;
        private static bool s_isInitialized, s_isInitialized2;
        
        /// <summary>
        ///     Gets the white texture.
        /// </summary>
        /// <value>
        ///     The white texture.
        /// </value>
        public static Texture WhiteTexture
        {
            get { return s_whiteTexture!; }
        }

        /// <summary>
        ///     Gets the white texture 2.
        /// </summary>
        /// <value>
        ///     The white texture 2.
        /// </value>
        public static Texture2 WhiteTexture2
        {
            get { return s_whiteTexture2!; }
        }

        /// <summary>
        ///     Initializes the textures.
        /// </summary>
        /// <param name="device"> The device. </param>
        internal static void InitializeTextures(Device5 device)
        {
            if (!s_isInitialized)
            {
                s_isInitialized = true;
                s_disposedValue = false;

                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(WHITE_TEXTURE_BASE64)))
                {
                    s_whiteTexture = Texture.Load(device, ms) ??
                                     throw new NullReferenceException($"{nameof(WhiteTexture)}");
                }
            }
        }

        /// <summary>
        ///     Initializes the textures 2.
        /// </summary>
        /// <param name="manager"> The manager. </param>
        internal static void InitializeTextures2(ITexture2ContentManager manager)
        {
            if (!s_isInitialized2)
            {
                s_isInitialized2 = true;
                s_disposedValue = false;

                using (Stream stream = new MemoryStream(Convert.FromBase64String(WHITE_TEXTURE_BASE64)))
                {
                    s_whiteTexture2 = manager.AddTexture(stream, "WHITE_TEXTURE_BASE64");
                }
            }
        }

        #region IDisposable Support

        /// <summary>
        ///     True to disposed value.
        /// </summary>
        private static bool s_disposedValue;

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting
        ///     unmanaged resources.
        /// </summary>
        /// <param name="disposing">
        ///     True to release both managed and unmanaged resources; false to
        ///     release only unmanaged resources.
        /// </param>
        public static void Dispose(bool disposing)
        {
            if (!s_disposedValue)
            {
                if (disposing)
                {
                    Utilities.Dispose(ref s_whiteTexture);
                    Utilities.Dispose(ref s_whiteTexture2);
                }

                s_isInitialized = false;
                s_isInitialized2 = false;

                s_disposedValue = true;
            }
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting
        ///     unmanaged resources.
        /// </summary>
        public static void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}