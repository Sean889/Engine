using EngineSystem.Messaging;
using EngineSystem.Threading;
using System;
using System.Threading;
using System.Collections.Generic;

namespace EngineSystem
{
    using UpdateEndEventHandler = MultithreadedEventHandler<Engine, EventArgs>;
    using UpdateEventHandler = MultithreadedEventHandler<Engine, UpdateEventArgs>;

#pragma warning disable 1591

    /// <summary>
    /// Thrown if a engine is created when another already exists
    /// </summary>
    [Serializable]
    public class EngineAlreadyCreatedException : Exception
    {
        public EngineAlreadyCreatedException() { }
        public EngineAlreadyCreatedException(string message) : base(message) { }
        public EngineAlreadyCreatedException(string message, Exception inner) : base(message, inner) { }
        protected EngineAlreadyCreatedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

#pragma warning restore 1591

    /// <summary>
    /// Central engine class.
    /// This class controls all update events.
    /// Any operations on this class should be thread safe.
    /// </summary>
    public class Engine : IDisposable
    {
        #region Static Members
        private static volatile bool Initialized;
        private static List<Action<Engine>> EngineCreateEvent = new List<Action<Engine>>();
        private static Engine CurrentEngineInternal;

        private static void InitStatics(Engine Eng)
        {
            CurrentEngine = Eng;
            foreach(var val in EngineCreateEvent)
            {
                val(Eng);
            }
        }
        private static void DisposeStatics()
        {
            EngineCreateEvent.Clear();
            CurrentEngine = null;
        }

        /// <summary>
        /// Allows for systems to lazily initialize themselves when the engine is created.
        /// The callbacks for this event are wiped when the engine is destroyed
        /// </summary>
        public static event Action<Engine> OnEngineCreate
        {
            add
            {
                EngineCreateEvent.Add(value);
            }
            remove
            {
                EngineCreateEvent.Remove(value);
            }
        }

        /// <summary>
        /// The currently active engine.
        /// </summary>
        public static Engine CurrentEngine
        {
            get
            {
                return CurrentEngineInternal;
            }
            private set
            {
                CurrentEngineInternal = value;
            }
        }
        /// <summary>
        /// Whether the an engine is initialized.
        /// </summary>
        public static bool EngineActive
        {
            get
            {
                return CurrentEngine != null;
            }
        }
        #endregion
        #region Nonstatic Members
        /// <summary>
        /// Type that evaluates a condition.
        /// </summary>
        /// <param name="Eng"> The Engine. </param>
        /// <returns></returns>
        public delegate bool Predicate(Engine Eng);

        private UpdateEventHandler InternalUpdateEvent = new UpdateEventHandler();
        private UpdateEndEventHandler InternalUpdateEndEvent = new UpdateEndEventHandler();
        private ThreadedEventHandler<Engine, EventArgs> InternalDisposeEvent = new ThreadedEventHandler<Engine, EventArgs>();
        private EventManager Manager = new EventManager();
        private EntitySystem InternalEntitySystem = new EntitySystem();

        /// <summary>
        /// The EntitySystem associated with the engine.
        /// </summary>
        public EntitySystem EntitySystem
        {
            get
            {
                return InternalEntitySystem;
            }
        }
        
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
            InternalUpdateEndEvent.Fire(this, EventArgs.Empty);
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
        /// <summary>
        /// Removes the system from the engine.
        /// </summary>
        /// <param name="System"></param>
        public void RemoveSystem(ISystem System)
        {
            System.Unregister(this);
        }

        /// <summary>
        /// Disposes of all engine resources that require disposing of.
        /// Calls the OnDispose callback. 
        /// It is safe to create a new engine after this has been called.
        /// </summary>
        public void Dispose()
        {
            InternalDisposeEvent.Fire(this, new EventArgs());
            ThreadPool.ThreadPoolManager.Terminate();
            Initialized = false;
            DisposeStatics();
        }
         
        /// <summary>
        /// Constructs the Engine.
        /// </summary>
        public Engine()
        {
            if (Initialized)
                throw new EngineAlreadyCreatedException("An engine is still active.");
            ThreadPool.ThreadPoolManager.Init(0);
            InitStatics(this);
            InternalEntitySystem = new EntitySystem();
            AddSystem(InternalEntitySystem);
        }
        #endregion
    }
}
