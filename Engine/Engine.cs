using EngineSystem.Messaging;
using EngineSystem.Threading;
using System;
using System.Threading;

namespace EngineSystem
{
    using UpdateEndEventHandler = MultithreadedEventHandler<Engine, EventArgs>;
    using UpdateEventHandler = MultithreadedEventHandler<Engine, UpdateEventArgs>;

    /// <summary>
    /// Central engine class.
    /// This class controls all update events.
    /// Any operations on this class should be thread safe.
    /// </summary>
    public class Engine : IDisposable
    {
        public delegate bool Predicate(Engine Eng);

        private UpdateEventHandler InternalUpdateEvent = new UpdateEventHandler();
        private UpdateEndEventHandler InternalUpdateEndEvent = new UpdateEndEventHandler();
        private ThreadedEventHandler<Engine, EventArgs> InternalDisposeEvent = new ThreadedEventHandler<Engine, EventArgs>();
        private EventManager Manager = new EventManager();
        
        /// <summary>
        /// Event that is triggered everytime the engine updates.
        /// Event handlers will be run on different threads.
        /// </summary>
        public event EventAction<Engine, UpdateEventArgs> OnUpdate
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
        public event EventAction<Engine, EventArgs> OnUpdateEnd
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
        /// Fired when the engine is disposed of.
        /// </summary>
        public event EventAction<Engine, EventArgs> OnDispose
        {
            add
            {
                InternalDisposeEvent.AddEventListener(value);
            }
            remove
            {
                InternalDisposeEvent.RemoveEventListener(value);
            }
        }

        /// <summary>
        /// The event manager of the engine. Use this to fire off any events.
        /// </summary>
        public EventManager EventManager
        {
            get
            {
                return Manager;
            }
        }

        /// <summary>
        /// Fires the UpdateEvent.
        /// The update will continue running until UpdateEndAsync is called.
        /// </summary>
        /// <param name="DeltaTime"> The time that passed since the last update. </param>
        public void UpdateAsync(double DeltaTime)
        {
            InternalUpdateEndEvent.Wait();
            Interlocked.MemoryBarrier();
            InternalUpdateEvent.Fire(this, new UpdateEventArgs(DeltaTime));
        }
        /// <summary>
        /// Ends the current update and fires the OnUpdateEndEvent.
        /// </summary>
        public void UpdateEndAsync()
        {
            InternalUpdateEvent.Wait();
            Interlocked.MemoryBarrier();
            InternalUpdateEndEvent.Fire(this, null);
        }
        /// <summary>
        /// Call after UpdateEndAsync.
        /// Ends the callback but doesn't fire the next callback.
        /// </summary>
        public void StopAsyncUpdate()
        {
            InternalUpdateEndEvent.Wait();
        }

        /// <summary>
        /// Fires the UpdateEvent.
        /// </summary>
        /// <param name="DeltaTime"></param>
        public void FireUpdate(double DeltaTime)
        {
            Interlocked.MemoryBarrier();
            InternalUpdateEvent.Fire(this, new UpdateEventArgs(DeltaTime));
            InternalUpdateEvent.Wait();
        }
        /// <summary>
        /// Fires the UpdateEndEvent.
        /// </summary>
        public void FireUpdateEnd()
        {
            Interlocked.MemoryBarrier();
            InternalUpdateEndEvent.Fire(this, null);
            InternalUpdateEndEvent.Wait();
        }

        /// <summary>
        /// Performs a full update step of the engine.
        /// </summary>
        /// <param name="DeltaTime"></param>
        public void Update(double DeltaTime)
        {
            Interlocked.MemoryBarrier();
            InternalUpdateEvent.Fire(this, new UpdateEventArgs(DeltaTime));
            InternalUpdateEvent.Wait();
            Interlocked.MemoryBarrier();
            InternalUpdateEndEvent.Fire(this, null);
            InternalUpdateEndEvent.Wait();
        }

        /// <summary>
        /// Continuously runs an update until a predicate indicates to stop.
        /// </summary>
        /// <param name="Pred"> Returns true if the loop should continue, false otherwise. </param>
        public void UpdateLoop(Predicate<Engine> Pred)
        {
            System.Diagnostics.Stopwatch Timer = new System.Diagnostics.Stopwatch();
            Timer.Start();

            do
            {
                double Time = (double)Timer.ElapsedMilliseconds;
                Timer.Reset();
                UpdateAsync(Time);
                UpdateEndAsync();
            } while (Pred(this));

            StopAsyncUpdate();
            Timer.Stop();
        }

        /// <summary>
        /// Adds a system to the engine.
        /// </summary>
        /// <param name="System"></param>
        public void AddSystem(ISystem System)
        {
            System.Register(this);
        }

        public void Dispose()
        {
            InternalDisposeEvent.Fire(this, new EventArgs());
        }
    }
}
