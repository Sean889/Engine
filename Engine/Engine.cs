using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineSystem
{
    using UpdateEventHandler = ThreadedEventHandler<Engine, UpdateEventArgs>;
    using UpdateEndEventHandler = ThreadedEventHandler<Engine, Null>;

    /// <summary>
    /// Central engine class.
    /// This class controls all update events.
    /// Any operations on this class should be thread safe.
    /// </summary>
    public class Engine
    {
        private UpdateEventHandler InternalUpdateEvent = new UpdateEventHandler();
        private UpdateEndEventHandler InternalUpdateEndEvent = new UpdateEndEventHandler();
        
        /// <summary>
        /// Event that is triggered everytime the engine updates.
        /// Event handlers will be run on different threads.
        /// </summary>
        public event EventType<Engine, UpdateEventArgs>.EventAction OnUpdate
        {
            add
            {
                InternalUpdateEvent.AddEventListener(value);
            }
            remove
            {
                InternalUpdateEvent.RemoveEventListener(value);
            }
        }
        /// <summary>
        /// Event that is fired after all modifications have been done.
        /// Use this if state has to be updated in a thread safe way.
        /// Code running in this event should not modify the state of any other systems.
        /// </summary>
        public event EventType<Engine, Null>.EventAction OnUpdateEnd
        {
            add
            {
                InternalUpdateEndEvent.AddEventListener(value);
            }
            remove
            {
                InternalUpdateEndEvent.RemoveEventListener(value);
            }
        }

        /// <summary>
        /// Begins an update sequence.
        /// The update will continue running until end update async is called.
        /// </summary>
        /// <param name="DeltaTime"> The time that passed since the last update. </param>
        public void UpdateAsync(double DeltaTime)
        {
            InternalUpdateEndEvent.Wait();
            InternalUpdateEvent.Fire(this, new UpdateEventArgs(DeltaTime));
        }
        /// <summary>
        /// Ends the current update and fires the OnUpdateEndEvent.
        /// </summary>
        public void UpdateEndAsync()
        {
            InternalUpdateEvent.Wait();
            InternalUpdateEndEvent.Fire(this, null);
        }

        /// <summary>
        /// Call after UpdateEndAsync.
        /// Ends the callback but doesn't fire the next callback.
        /// </summary>
        public void EndAll()
        {
            InternalUpdateEndEvent.Wait();
        }

        /// <summary>
        /// Allows a System to register itself.
        /// </summary>
        /// <param name="System"></param>
        public void AddSystem(ISystem System)
        {
            System.Register(this);
        }
    }
}
