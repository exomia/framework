namespace Exomia.Framework.ContentManager.Fonts.MSDFFont;

public class Glyph
{
    public int          unicode     { get; set; }
    public double       advance     { get; set; }
    public PlaneBounds? planeBounds { get; set; }
    public AtlasBounds? atlasBounds { get; set; }
}