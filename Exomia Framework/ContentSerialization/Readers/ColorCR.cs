using SharpDX;

namespace Exomia.Framework.ContentSerialization.Readers
{
    internal sealed class ColorCR : AContentSerializationReader<Color>
    {
        public override Color ReadContext(ContentSerializationContext context)
        {
            Color result = new Color();
            result.A = context.Get<byte>("A");
            result.R = context.Get<byte>("R");
            result.G = context.Get<byte>("G");
            result.B = context.Get<byte>("B");
            return result;
        }
    }
}