using SharpDX;

namespace Exomia.Framework.ContentSerialization.Readers
{
    internal sealed class RectangleFCR : AContentSerializationReader<RectangleF>
    {
        public override RectangleF ReadContext(ContentSerializationContext context)
        {
            RectangleF result = new RectangleF();
            result.X = context.Get<float>("X");
            result.Y = context.Get<float>("Y");
            result.Width = context.Get<float>("Width");
            result.Height = context.Get<float>("Height");
            return result;
        }
    }

    internal sealed class RectangleCR : AContentSerializationReader<Rectangle>
    {
        public override Rectangle ReadContext(ContentSerializationContext context)
        {
            Rectangle result = new Rectangle();
            result.X = context.Get<int>("X");
            result.Y = context.Get<int>("Y");
            result.Width = context.Get<int>("Width");
            result.Height = context.Get<int>("Height");
            return result;
        }
    }
}