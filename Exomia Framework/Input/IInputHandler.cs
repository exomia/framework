namespace Exomia.Framework.Input
{
    /// <summary>
    ///     an interface used for input handling
    /// </summary>
    public interface IInputHandler
    {
        /// <summary>
        ///     called than the mouse moved
        /// </summary>
        /// <param name="x">x-coordinate</param>
        /// <param name="y">x-coordinate</param>
        /// <param name="buttons">buttons</param>
        /// <param name="clicks">clicks</param>
        /// <param name="wheelDelta">wheeldelta</param>
        void Input_MouseMove(int x, int y, MouseButtons buttons, int clicks, int wheelDelta);

        /// <summary>
        ///     called than a mouse button is down
        /// </summary>
        /// <param name="x">x-coordinate</param>
        /// <param name="y">x-coordinate</param>
        /// <param name="buttons">buttons</param>
        /// <param name="clicks">clicks</param>
        /// <param name="wheelDelta">wheeldelta</param>
        void Input_MouseDown(int x, int y, MouseButtons buttons, int clicks, int wheelDelta);

        /// <summary>
        ///     called than a mouse button is up
        /// </summary>
        /// <param name="x">x-coordinate</param>
        /// <param name="y">x-coordinate</param>
        /// <param name="buttons">buttons</param>
        /// <param name="clicks">clicks</param>
        /// <param name="wheelDelta">wheeldelta</param>
        void Input_MouseUp(int x, int y, MouseButtons buttons, int clicks, int wheelDelta);

        /// <summary>
        ///     called than a mouse button is clicked
        /// </summary>
        /// <param name="x">x-coordinate</param>
        /// <param name="y">x-coordinate</param>
        /// <param name="buttons">buttons</param>
        /// <param name="clicks">clicks</param>
        /// <param name="wheelDelta">wheeldelta</param>
        void Input_MouseClick(int x, int y, MouseButtons buttons, int clicks, int wheelDelta);

        /// <summary>
        ///     called than the mouse wheel delta is changed
        /// </summary>
        /// <param name="x">x-coordinate</param>
        /// <param name="y">x-coordinate</param>
        /// <param name="buttons">buttons</param>
        /// <param name="clicks">clicks</param>
        /// <param name="wheelDelta">wheeldelta</param>
        void Input_MouseWheel(int x, int y, MouseButtons buttons, int clicks, int wheelDelta);

        /// <summary>
        ///     called than a key is pressed
        /// </summary>
        /// <param name="key">key char</param>
        void Input_KeyPress(char key);

        /// <summary>
        ///     called than a key is up
        /// </summary>
        /// <param name="keyValue">key value</param>
        /// <param name="shift"><c>true</c> if shift ley is down; <c>false</c> otherwise.</param>
        /// <param name="alt"><c>true</c> if alt key is down; <c>false</c> otherwise.</param>
        /// <param name="ctrl"><c>true</c> if ctrl key is down; <c>false</c> otherwise.</param>
        void Input_KeyUp(int keyValue, bool shift, bool alt, bool ctrl);

        /// <summary>
        ///     called than a key is down
        /// </summary>
        /// <param name="keyValue">key value</param>
        /// <param name="shift"><c>true</c> if shift ley is down; <c>false</c> otherwise.</param>
        /// <param name="alt"><c>true</c> if alt key is down; <c>false</c> otherwise.</param>
        /// <param name="ctrl"><c>true</c> if ctrl key is down; <c>false</c> otherwise.</param>
        void Input_KeyDown(int keyValue, bool shift, bool alt, bool ctrl);
    }
}