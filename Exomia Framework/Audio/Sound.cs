using SharpDX.X3DAudio;
using SharpDX.XAudio2;

namespace Exomia.Framework.Audio
{
    internal sealed class Sound
    {
        internal Emitter Emitter;
        internal Sound Next = null;
        internal Sound Previous = null;
        internal SourceVoice SourceVoice;
    }
}