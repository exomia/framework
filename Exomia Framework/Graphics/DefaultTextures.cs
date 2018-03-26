#pragma warning disable 1591

using System;
using System.IO;
using Exomia.Framework.Content;
using SharpDX;
using SharpDX.Direct3D11;

namespace Exomia.Framework.Graphics
{
    public static class DefaultTextures
    {
        #region Constants

        private const string WHITE_TEXTURE_BASE64 =
            "iVBORw0KGgoAAAANSUhEUgAAAAIAAAACCAQAAADYv8WvAAAAEElEQVR42mP8/5+BgRFEAAAYAQP/58fuIwAAAABJRU5ErkJggg==";

        private const string BLACK_TEXTURE_BASE64 =
            "iVBORw0KGgoAAAANSUhEUgAAAAIAAAACCAQAAADYv8WvAAAAD0lEQVR42mNk+M/AwAgiAAsOAgGA6bm/AAAAAElFTkSuQmCC";

        #endregion

        #region Variables

        #region Statics

        private static Texture s_whiteTexture;
        private static Texture s_blackTexture;

        private static Texture2 s_whiteTexture2;
        private static Texture2 s_blackTexture2;

        private static bool s_isInitialized;
        private static bool s_isInitialized2;

        #endregion

        #endregion

        #region Properties

        #region Statics

        public static Texture WhiteTexture
        {
            get { return s_whiteTexture; }
        }

        public static Texture BlackTexture
        {
            get { return s_blackTexture; }
        }

        public static Texture2 WhiteTexture2
        {
            get { return s_whiteTexture2; }
        }

        public static Texture2 BlackTexture2
        {
            get { return s_blackTexture2; }
        }

        #endregion

        #endregion

        #region Constructors

        #region Statics

        #endregion

        #endregion

        #region Methods

        #region Statics

        internal static void InitializeTextures(Device5 device)
        {
            if (!s_isInitialized)
            {
                s_isInitialized = true;
                s_disposedValue = false;

                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(WHITE_TEXTURE_BASE64)))
                {
                    s_whiteTexture = Texture.Load(device, ms);
                }

                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(BLACK_TEXTURE_BASE64)))
                {
                    s_blackTexture = Texture.Load(device, ms);
                }
            }
        }

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

                using (Stream stream = new MemoryStream(Convert.FromBase64String(BLACK_TEXTURE_BASE64)))
                {
                    s_blackTexture2 = manager.AddTexture(stream, "BLACK_TEXTURE_BASE64");
                }
            }
        }

        #endregion

        #endregion

        #region IDisposable Support

        private static bool s_disposedValue;

        public static void Dispose(bool disposing)
        {
            if (!s_disposedValue)
            {
                if (disposing)
                {
                    Utilities.Dispose(ref s_blackTexture);
                    Utilities.Dispose(ref s_blackTexture2);
                    Utilities.Dispose(ref s_whiteTexture);
                    Utilities.Dispose(ref s_whiteTexture2);
                }

                s_isInitialized = false;
                s_isInitialized2 = false;

                s_disposedValue = true;
            }
        }

        public static void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}