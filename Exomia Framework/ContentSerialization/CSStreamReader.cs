using System;
using System.IO;

namespace Exomia.Framework.ContentSerialization
{
    internal sealed class CSStreamReader : IDisposable
    {
        private readonly Stream _stream;
        private int _line = 1;

        internal CSStreamReader(Stream stream)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        public long Index { get; private set; }

        public long Line
        {
            get { return _line; }
        }

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