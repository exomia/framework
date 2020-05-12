using System;
using System.Runtime.CompilerServices;

namespace Exomia.Framework.Game
{
    sealed partial class RenderForm
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int LowWord(IntPtr number)
        {
            return (int)number.ToInt64() & 0x0000FFFF;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int HighWord(IntPtr number)
        {
            return (int)number.ToInt64() >> 16;
        }
    }
}
