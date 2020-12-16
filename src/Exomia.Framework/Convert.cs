using System;

namespace Exomia.Framework
{
    /// <summary>
    ///     A conversion helper class.
    /// </summary>
    public static class Convert
    {
        private const int LOWERCASE_A_OFFSET = 'a' - 10;
        private const int UPPERCASE_A_OFFSET = 'A' - 10;

        /// <summary>
        ///     Converts the <paramref name="value"/> to an <see cref="UInt32"/>.
        /// </summary>
        /// <param name="value"> The value. </param>
        /// <param name="b">     (Optional) The number base. </param>
        /// <returns>
        ///     The given <paramref name="value"/> converted to an <see cref="UInt32"/>.
        /// </returns>
        public static unsafe uint ToUInt32(string value, uint b = 10)
        {
            fixed (char* ptr = value)
            {
                char* cPtr = ptr;
                while (*cPtr == ' ') { cPtr++; }

                switch (b)
                {
                    case 16 when *cPtr == '0' && (*(cPtr + 1) == 'x' || *(cPtr + 1) == 'X'):
                    case 2 when *cPtr == '0' && (*(cPtr + 1) == 'b' || *(cPtr + 1) == 'B'):
                        cPtr += 2;
                        break;
                }

                uint acc = 0;
                for (uint c = *cPtr; cPtr < ptr + value.Length; c = *++cPtr)
                {
                    if (c >= '0' && c <= '9')
                        c -= '0';
                    else if (c >= 'a' && c <= 'f')
                        c -= LOWERCASE_A_OFFSET;
                    else if (c >= 'A' && c <= 'F')
                        c -= UPPERCASE_A_OFFSET;
                    else
                        break;
                    if (c >= b)
                        break;

                    acc *= b;
                    acc += c;
                }
                
                return acc;
            }
        }

        /// <summary>
        ///     Converts the <paramref name="value"/> to an <see cref="UInt64"/>.
        /// </summary>
        /// <param name="value"> The value. </param>
        /// <param name="b">     (Optional) The number base. </param>
        /// <returns>
        ///     The given <paramref name="value"/> converted to an <see cref="UInt64"/>.
        /// </returns>
        public static unsafe ulong ToUInt64(string value, uint b = 10)
        {
            fixed (char* ptr = value)
            {
                char* cPtr = ptr;
                while (*cPtr == ' ') { cPtr++; }

                switch (b)
                {
                    case 16 when *cPtr == '0' && (*(cPtr + 1) == 'x' || *(cPtr + 1) == 'X'):
                    case 2 when *cPtr == '0' && (*(cPtr + 1) == 'b' || *(cPtr + 1) == 'B'):
                        cPtr += 2;
                        break;
                }

                ulong acc = 0;
                for (uint c = *cPtr; cPtr < ptr + value.Length; c = *++cPtr)
                {
                    if (c >= '0' && c <= '9')
                        c -= '0';
                    else if (c >= 'a' && c <= 'f')
                        c -= LOWERCASE_A_OFFSET;
                    else if (c >= 'A' && c <= 'F')
                        c -= UPPERCASE_A_OFFSET;
                    else
                        break;
                    if (c >= b)
                        break;

                    acc *= b;
                    acc += c;
                }

                return acc;
            }
        }
    }
}
