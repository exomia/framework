#region License

// Copyright (c) 2018-2022, exomia
// All rights reserved.
// 
// This source code is licensed under the BSD-style license found in the
// LICENSE file in the root directory of this source tree.

#endregion

namespace Exomia.Framework.Core.ContentSerialization
{
    internal sealed class CsStreamReader : IDisposable
    {
        private readonly Stream _stream;
        private          int    _line = 1;

        /// <summary> Gets the zero-based index of this object. </summary>
        /// <value> The index. </value>
        public long Index { get; private set; }

        /// <summary> Gets the line. </summary>
        /// <value> The line. </value>
        public long Line
        {
            get { return _line; }
        }

        /// <summary> Initializes a new instance of the <see cref="CsStreamReader" /> class. </summary>
        /// <param name="stream"> The stream. </param>
        /// <exception cref="ArgumentNullException"> Thrown when one or more required arguments are null. </exception>
        internal CsStreamReader(Stream stream)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _stream.Close();
            _stream.Dispose();
        }

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