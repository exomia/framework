namespace Exomia.Framework.Input
{
    /// <summary>
    ///     Delegate for handling mouse events.
    /// </summary>
    /// <param name="x">          The x coordinate. </param>
    /// <param name="y">          The y coordinate. </param>
    /// <param name="buttons">    The buttons. </param>
    /// <param name="clicks">     The clicks. </param>
    /// <param name="wheelDelta"> The wheel delta. </param>
    public delegate void MouseEventHandler(int x, int y, MouseButtons buttons, int clicks, int wheelDelta);
}
