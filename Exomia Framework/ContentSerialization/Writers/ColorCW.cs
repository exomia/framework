using SharpDX;

namespace Exomia.Framework.ContentSerialization.Writers
{
    internal sealed class ColorCW : AContentSerializationWriter<Color>
    {
        public override void WriteContext(ContentSerializationContext context, Color obj)
        {
            context.Set("A", obj.A);
            context.Set("R", obj.R);
            context.Set("G", obj.G);
            context.Set("B", obj.B);
        }
    }
}