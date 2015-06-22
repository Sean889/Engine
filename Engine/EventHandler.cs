using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

using TP = ThreadPool;

namespace EngineSystem
{
    public class EventType<TSender, TArgs>
    {
        public delegate void EventAction(TSender Sender, TArgs Args);
    }

    internal class ThreadedEventHandler<TSender, TEventArgs> where TSender : class
    {
        private object SyncObject = new object();
        private ConcurrentQueue<EventType<TSender, TEventArgs>.EventAction> AddQueue = new ConcurrentQueue<EventType<TSender, TEventArgs>.EventAction>();
        private ConcurrentQueue<EventType<TSender, TEventArgs>.EventAction> RemoveQueue = new ConcurrentQueue<EventType<TSender, TEventArgs>.EventAction>();
        private List<EventType<TSender, TEventArgs>.EventAction> Actions = new List<EventType<TSender, TEventArgs>.EventAction>();

        private List<TP.Future> Futures = new List<TP.Future>();

        internal void AddEventListener(EventType<TSender, TEventArgs>.EventAction e)
        {
            AddQueue.Enqueue(e);
        }
        internal void RemoveEventListener(EventType<TSender, TEventArgs>.EventAction e)
        {
            RemoveQueue.Enqueue(e);
        }

        internal void Fire(TSender tSender, TEventArgs tArgs)
        {
            EventType<TSender, TEventArgs>.EventAction Action;
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

                foreach (EventType<TSender, TEventArgs>.EventAction tAct in Actions)
                {
                    Futures.Add(TP.ThreadPoolManager.QueueAsync(
                        new Action(delegate
                        {
                            tAct(tSender, tArgs);
                        })));
                }
            }
        }
        internal void Wait()
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
