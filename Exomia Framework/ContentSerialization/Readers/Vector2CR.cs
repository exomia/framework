using SharpDX;

namespace Exomia.Framework.ContentSerialization.Readers
{
    internal sealed class Vector2CR : AContentSerializationReader<Vector2>
    {
        public override Vector2 ReadContext(ContentSerializationContext context)
        {
            Vector2 result = new Vector2();
            result.X = context.Get<float>("X");
            result.Y = context.Get<float>("Y");
            return result;
        }
    }
}