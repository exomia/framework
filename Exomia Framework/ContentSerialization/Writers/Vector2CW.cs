using SharpDX;

namespace Exomia.Framework.ContentSerialization.Writers
{
    internal sealed class Vector2CW : AContentSerializationWriter<Vector2>
    {
        public override void WriteContext(ContentSerializationContext context, Vector2 obj)
        {
            context.Set("X", obj.X);
            context.Set("Y", obj.Y);
        }
    }
}