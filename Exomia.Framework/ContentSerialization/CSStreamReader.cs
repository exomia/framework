#region License

// Copyright (c) 2018-2019, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

using System;
using System.IO;

namespace Exomia.Framework.ContentSerialization
{
    /// <summary>
    ///     A create struct stream reader. This class cannot be inherited.
    /// </summary>
    sealed class CSStreamReader : IDisposable
    {
        /// <summary>
        ///     The stream.
        /// </summary>
        private readonly Stream _stream;

        /// <summary>
        ///     The line.
        /// </summary>
        private int _line = 1;

        /// <summary>
        ///     Gets the zero-based index of this object.
        /// </summary>
        /// <value>
        ///     The index.
        /// </value>
        public long Index { get; private set; }

        /// <summary>
        ///     Gets the line.
        /// </summary>
        /// <value>
        ///     The line.
        /// </value>
        public long Line
        {
            get { return _line; }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CSStreamReader" /> class.
        /// </summary>
        /// <param name="stream"> The stream. </param>
        /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
        internal CSStreamReader(Stream stream)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _stream.Close();
            _stream.Dispose();
        }

        /// <summary>
        ///     Reads a character.
        /// </summary>
        /// <param name="c"> [out] The out char to process. </param>
        /// <returns>
        ///     True if it succeeds, false if it fails.
        /// </returns>
        internal bool ReadChar(out char c)
        {
            int z;
            if ((z = _stream.ReadByte()) != -1)
            {
                Index++;
                c = (char)z;
                if (c == '\n')
                {
                    _line++;
                }
                return true;
            }
            c = '\0';
            return false;
        }
    }
}