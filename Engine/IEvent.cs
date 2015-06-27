
namespace EngineSystem.Messaging
{
    /// <summary>
    /// Base interface for an event.
    /// The event should be cast to the correct event once the receiving handler knows that it is the correct event.
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// Returns the unique ID of the event type.
        /// </summary>
        /// <returns></returns>
        uint GetID();
    }
}
