#pragma warning disable 1591

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Exomia.Framework.ContentSerialization;

namespace Exomia.Framework.Security
{
    public static class ExomiaCryptography
    {
        #region Constants

        public const string DEFAULT_CRYPT_EXTENSION = ".ds1";

        private const int ITERATIONS = 1000;

        private const int BUFFER_SIZE = 1024;

        public enum CryptionMode
        {
            Bit128 = 128,
            Bit192 = 192,
            Bit256 = 256
        }

        #endregion

        #region Variables

        #region Statics

        #endregion

        private static readonly byte[] s_saltBytes = Encoding.ASCII.GetBytes("fVJrgEUuCYOuHXNcyMw4euKSXymKUjmb");
        private static readonly byte[] s_secureKey = Encoding.ASCII.GetBytes("e7b5c571-0a24-4d5f-8c4a-800157b3fd17");

        #endregion

        #region Properties

        #region Statics

        #endregion

        #endregion

        #region Constructors

        #region Statics

        #endregion

        #endregion

        #region Methods

        #region Statics

        /// <summary>
        ///     Encrypt's a given file and outputs a stream
        /// </summary>
        /// <param name="assetName">Path to the file</param>
        /// <param name="outStream">out Encrypted Stream</param>
        /// <param name="mode"></param>
        /// <returns><c>true</c> if the file was successfully encrypted; <c>false</c> otherwise</returns>
        public static bool Encrypt(string assetName, out Stream outStream, CryptionMode mode = CryptionMode.Bit128)
        {
            if (!Path.HasExtension(assetName)) { assetName += ContentSerializer.DEFAULT_EXTENSION; }

            using (FileStream fs = new FileStream(assetName, FileMode.Open, FileAccess.Read))
            {
                return Encrypt(fs, out outStream, mode);
            }
        }

        /// <summary>
        ///     Encrypt's a given file and outputs a stream
        /// </summary>
        /// <param name="stream">the stream to encrypt</param>
        /// <param name="outStream">out Encrypted Stream</param>
        /// <param name="mode"></param>
        /// <returns><c>true</c> if the file was successfully encrypted; <c>false</c> otherwise</returns>
        public static bool Encrypt(Stream stream, out Stream outStream, CryptionMode mode = CryptionMode.Bit128)
        {
            if (stream == null) { throw new ArgumentNullException(nameof(stream)); }

            outStream = new MemoryStream();
            try
            {
                using (RijndaelManaged rijndael =
                    new RijndaelManaged { Mode = CipherMode.CBC, Padding = PaddingMode.ANSIX923 })
                {
                    using (Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(s_secureKey, s_saltBytes, ITERATIONS))
                    {
                        rijndael.BlockSize = (int)mode;

                        rijndael.Key = key.GetBytes(rijndael.KeySize / 8);
                        rijndael.IV = key.GetBytes(rijndael.BlockSize / 8);

                        using (ICryptoTransform encryptor = rijndael.CreateEncryptor(rijndael.Key, rijndael.IV))
                        {
                            CryptoStream cs = new CryptoStream(outStream, encryptor, CryptoStreamMode.Write);
                            byte[] buffer = new byte[BUFFER_SIZE];
                            int count = -1;
                            while ((count = stream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                cs.Write(buffer, 0, count);
                            }
                            cs.FlushFinalBlock();
                            outStream.Position = 0;
                        }
                    }
                }
            }
            catch { return false; }
            return true;
        }

        /// <summary>
        ///     Decrypt's a given asset and outputs a stream
        /// </summary>
        /// <param name="assetName">Path to the file</param>
        /// <param name="outStream">out Decrypted Stream</param>
        /// <param name="mode"></param>
        /// <returns><c>true</c> if the file was successfully decrypted; <c>false</c> otherwise</returns>
        internal static bool Decrypt(string assetName, out Stream outStream, CryptionMode mode = CryptionMode.Bit128)
        {
            if (!Path.HasExtension(assetName)) { assetName += DEFAULT_CRYPT_EXTENSION; }

            using (Stream fs = new FileStream(assetName, FileMode.Open, FileAccess.Read))
            {
                return Decrypt(fs, out outStream, mode);
            }
        }

        /// <summary>
        ///     Decrypt's a given asset and outputs a stream
        /// </summary>
        /// <param name="inStream">stream</param>
        /// <param name="outStream">out Decrypted Stream</param>
        /// <param name="mode"></param>
        /// <returns><c>true</c> if the file was successfully decrypted; <c>false</c> otherwise</returns>
        internal static bool Decrypt(Stream inStream, out Stream outStream, CryptionMode mode = CryptionMode.Bit128)
        {
            outStream = new MemoryStream();
            if (!inStream.CanRead) { return false; }

            try
            {
                using (RijndaelManaged rijndael =
                    new RijndaelManaged { Mode = CipherMode.CBC, Padding = PaddingMode.ANSIX923 })
                {
                    using (Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(s_secureKey, s_saltBytes, ITERATIONS))
                    {
                        rijndael.BlockSize = (int)mode;

                        rijndael.Key = key.GetBytes(rijndael.KeySize / 8);
                        rijndael.IV = key.GetBytes(rijndael.BlockSize / 8);

                        inStream.Position = 0;
                        using (ICryptoTransform decryptor = rijndael.CreateDecryptor(rijndael.Key, rijndael.IV))
                        {
                            CryptoStream cs = new CryptoStream(outStream, decryptor, CryptoStreamMode.Write);
                            byte[] buffer = new byte[BUFFER_SIZE];
                            int count = -1;
                            while ((count = inStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                cs.Write(buffer, 0, count);
                            }
                            cs.FlushFinalBlock();
                            outStream.Position = 0;
                        }
                    }
                }
            }
            catch { return false; }
            return true;
        }

        public static string ToMD5(this object input)
        {
            string hashString = input.ToString();
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(hashString);
            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString();
        }

        #endregion

        #endregion
    }
}