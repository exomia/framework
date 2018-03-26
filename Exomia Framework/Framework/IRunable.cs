namespace Exomia.Framework
{
    /// <summary>
    ///     An interface to run a object.
    /// </summary>
    public interface IRunnable
    {
        /// <summary>
        ///     return true if the object is running
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        ///     starts the run
        /// </summary>
        void Run();

        /// <summary>
        ///     shutdown the run
        /// </summary>
        void Shutdown();
    }
}