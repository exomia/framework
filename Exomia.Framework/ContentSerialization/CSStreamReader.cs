#region MIT License

// Copyright (c) 2019 exomia - Daniel Bätz
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

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