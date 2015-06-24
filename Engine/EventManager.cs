﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreadingUtils;

namespace EngineSystem.Messaging
{
    public class EventManager : ISystem
    {
        private ConcurrentDictionary<uint, ThreadedEventHandler<Object, IEvent>> EventHandlers = new ConcurrentDictionary<uint, ThreadedEventHandler<object, IEvent>>();
        private ConcurrentQueue<uint> ActiveEvents = new ConcurrentQueue<uint>();

        private void FireInvalidEvent(string Error)
        {
            InvalidEvent Event = new InvalidEvent(Error);

            ThreadedEventHandler<Object, IEvent> Handler;
            if (EventHandlers.TryGetValue(0, out Handler))
            {
                Handler.Fire(this, Event);
            }
        }

        /// <summary>
        /// Fires the Event using the given Event
        /// </summary>
        /// <param name="Event"> The event to be fired. </param>
        public void FireEvent(IEvent Event)
        {
            if (Event == null)
            {
                FireInvalidEvent("Event was null.");
                ActiveEvents.Enqueue(0);
            }
            else
            {
                ThreadedEventHandler<Object, IEvent> Delegate;
                if (EventHandlers.TryGetValue(Event.GetID(), out Delegate))
                {
                    Delegate.Fire(this, Event);
                }
                ActiveEvents.Enqueue(Event.GetID());
            }
        }
        public void AddEventHandler(uint ID, EventAction<Object, IEvent> EventHandler)
        {
            if (EventHandler == null)
                throw new ArgumentNullException("EventHandler was null.");

            ThreadedEventHandler<Object, IEvent> Handler;
            if(EventHandlers.TryGetValue(ID, out Handler))
            {
                Handler.AddEventListener(EventHandler);
            }
            else
            {
                Handler = new ThreadedEventHandler<object, IEvent>();
                Handler.AddEventListener(EventHandler);
                EventHandlers.TryAdd(ID, Handler);
            }
        }
        public void RemoveEventHandler(uint ID, EventAction<Object, IEvent> EventHandler)
        {
            if (EventHandler == null)
                throw new ArgumentNullException("EventHandler was null.");

            ThreadedEventHandler<Object, IEvent> Handler;
            if (EventHandlers.TryGetValue(ID, out Handler))
            {
                Handler.RemoveEventListener(EventHandler);
            }
        }

        public void Register(Engine Target)
        {
            
        }

        private void UpdateEnd(Engine Eng, EventArgs Args)
        {
            List<uint> Cleared = new List<uint>();

            uint ID;
            ThreadedEventHandler<Object, IEvent> Handler;

            while(ActiveEvents.TryDequeue(out ID))
            {
                if(!Cleared.Contains(ID))
                {
                    Cleared.Add(ID);
                    if(EventHandlers.TryGetValue(ID, out Handler))
                    {
                        Handler.Wait();
                    }
                }
            }
        }
    }
}
