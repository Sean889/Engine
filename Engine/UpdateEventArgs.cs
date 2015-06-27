
namespace EngineSystem
{
    /// <summary>
    /// Arguments for the update method.
    /// </summary>
    public struct UpdateEventArgs
    {
        /// <summary>
        /// The time passed since the last frame.
        /// </summary>
        public readonly double TimePassed;

        /// <summary>
        /// Creates the args with the given arguments.
        /// </summary>
        /// <param name="DeltaTime"> The time that has passed since the last frame. </param>
        public UpdateEventArgs(double DeltaTime)
        {
            TimePassed = DeltaTime;
        }
    }
}
