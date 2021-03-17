namespace Exomia.Framework.Windows.Input
{
    /// <summary> Interface for input device. </summary>
    public interface IWindowsInputDevice
    {
        /// <summary> Registers the raw key event. </summary>
        /// <param name="handler">  The handler. </param>
        /// <param name="position"> (Optional) The position. </param>
        /// <remarks>
        ///     <para>
        ///         <paramref name="position" /> a negative index inserts the handler from the back
        ///     </para>
        ///     <para>
        ///         <paramref name="position" /> a positive index inserts the handler from the start
        ///     </para>
        /// </remarks>
        void RegisterRawKeyEvent(RawKeyEventHandler handler, int position = -1);

        /// <summary> Unregister the raw key event described by handler. </summary>
        /// <param name="handler"> The handler. </param>
        void UnregisterRawKeyEvent(RawKeyEventHandler handler);
    }
}
