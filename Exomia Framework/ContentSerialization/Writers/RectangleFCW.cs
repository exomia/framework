using SharpDX;

namespace Exomia.Framework.ContentSerialization.Writers
{
    internal sealed class RectangleFCW : AContentSerializationWriter<RectangleF>
    {
        public override void WriteContext(ContentSerializationContext context, RectangleF obj)
        {
            context.Set("X", obj.X);
            context.Set("Y", obj.Y);
            context.Set("Width", obj.Width);
            context.Set("Height", obj.Height);
        }
    }

    internal sealed class RectangleCW : AContentSerializationWriter<Rectangle>
    {
        public override void WriteContext(ContentSerializationContext context, Rectangle obj)
        {
            context.Set("X", obj.X);
            context.Set("Y", obj.Y);
            context.Set("Width", obj.Width);
            context.Set("Height", obj.Height);
        }
    }
}