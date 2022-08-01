using System.Xml.Serialization;

namespace Exomia.Framework.ContentManager.Texture;

[Serializable]
[XmlRoot("texture")]
internal class Texture
{
    [XmlElement("width")]
    public int Width { get; set; }

    [XmlElement("height")]
    public int Height { get; set; }
    
    [XmlElement("data")]
    public byte[]? Data { get; set; }
}