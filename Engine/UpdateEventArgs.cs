
namespace EngineSystem
{
    /// <summary>
    /// Arguments for the update method.
    /// </summary>
    public struct UpdateEventArgs
    {
        public readonly double TimePassed;

        public UpdateEventArgs(double DeltaTime)
        {
            TimePassed = DeltaTime;
        }
    }
}
