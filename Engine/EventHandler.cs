using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TP = ThreadPool;

namespace EngineSystem.Threading
{
    /// <summary>
    /// A ganeric function that supports an sender type and any argument type.
    /// </summary>
    /// <typeparam name="TSender"> The sender object type. </typeparam>
    /// <typeparam name="TEventArgs"> The arguments object type. </typeparam>
    /// <param name="Sender"> The sender. </param>
    /// <param name="Args"> The arguments. </param>
    public delegate void EventAction<TSender, TEventArgs>(TSender Sender, TEventArgs Args);

    /// <summary>
    /// A multithreaded event handler that supports any sender type and any argument type.
    /// </summary>
    /// <typeparam name="TSender"> The sender type. </typeparam>
    /// <typeparam name="TEventArgs"> The argument type. </typeparam>
    public class ThreadedEventHandler<TSender, TEventArgs> where TSender : class
    {
        private object SyncObject = new object();
        private ConcurrentQueue<EventAction<TSender, TEventArgs>> AddQueue = new ConcurrentQueue<EventAction<TSender, TEventArgs>>();
        private ConcurrentQueue<EventAction<TSender, TEventArgs>> RemoveQueue = new ConcurrentQueue<EventAction<TSender, TEventArgs>>();
        private List<EventAction<TSender, TEventArgs>> Actions = new List<EventAction<TSender, TEventArgs>>();

        private List<TP.Future> Futures = new List<TP.Future>();

        /// <summary>
        /// Adds an event listener to the event list.
        /// </summary>
        /// <param name="e"> The listener to add. </param>
        public void AddEventListener(EventAction<TSender, TEventArgs> e)
        {
            AddQueue.Enqueue(e);
        }
        /// <summary>
        /// Removes an event listener from the event list.
        /// </summary>
        /// <param name="e"></param>
        public void RemoveEventListener(EventAction<TSender, TEventArgs> e)
        {
            RemoveQueue.Enqueue(e);
        }

        /// <summary>
        /// Begins executing all the event listeners.
        /// </summary>
        /// <param name="tSender"> The sender. </param>
        /// <param name="tArgs"> The arguments. </param>
        public void Fire(TSender tSender, TEventArgs tArgs)
        {
            EventAction<TSender, TEventArgs> Action;
            lock (SyncObject)
            {
                while (AddQueue.TryDequeue(out Action))
                {
                    Actions.Add(Action);
                }

                while (RemoveQueue.TryDequeue(out Action))
                {
                    Actions.Remove(Action);
                }

                foreach (EventAction<TSender, TEventArgs> tAct in Actions)
                {
                    Futures.Add(TP.ThreadPoolManager.QueueAsync(
                        new Action(delegate
                        {
                            tAct(tSender, tArgs);
                        })));
                }
            }
        }
        /// <summary>
        /// Waits for all the currently executing event listeners to finish.
        /// </summary>
        public void Wait()
        {
            lock(SyncObject)
            {
                foreach(TP.Future Future in Futures)
                {
                    Future.Complete();
                }
            }
        }
    }
}
