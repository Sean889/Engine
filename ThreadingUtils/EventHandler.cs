using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

using TP = ThreadPool;

namespace ThreadingUtils
{
    public delegate void EventAction<TSender, TEventArgs>(TSender Sender, TEventArgs Args);

    public class ThreadedEventHandler<TSender, TEventArgs> where TSender : class
    {
        private object SyncObject = new object();
        private ConcurrentQueue<EventAction<TSender, TEventArgs>> AddQueue = new ConcurrentQueue<EventAction<TSender, TEventArgs>>();
        private ConcurrentQueue<EventAction<TSender, TEventArgs>> RemoveQueue = new ConcurrentQueue<EventAction<TSender, TEventArgs>>();
        private List<EventAction<TSender, TEventArgs>> Actions = new List<EventAction<TSender, TEventArgs>>();

        private List<TP.Future> Futures = new List<TP.Future>();

        public void AddEventListener(EventAction<TSender, TEventArgs> e)
        {
            AddQueue.Enqueue(e);
        }
        public void RemoveEventListener(EventAction<TSender, TEventArgs> e)
        {
            RemoveQueue.Enqueue(e);
        }

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
