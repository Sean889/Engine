using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineSystem
{
    public class EntitySystem : ISystem
    {
        /// <summary>
        /// An object used for locking access to the Entity list.
        /// </summary>
        private object SyncObject = new object();

        /// <summary>
        /// A list of all the entities.
        /// </summary>
        private List<Entity> Entities = new List<Entity>();

        /// <summary>
        /// A queue containing all the entities for the system. 
        /// </summary>
        private ConcurrentQueue<Entity> AddQueue = new ConcurrentQueue<Entity>();
        /// <summary>
        /// A queue containing all the items to be removed at the end of the frame.
        /// </summary>
        private ConcurrentQueue<Entity> RemoveQueue = new ConcurrentQueue<Entity>();

        /// <summary>
        /// Adds an entity to the list.
        /// The entity will not actually be on the list until the next frame but its state can be freely modified until then anyways.
        /// </summary>
        /// <param name="ToAdd"> The entitity to add to the list. </param>
        public void AddEntity(Entity ToAdd)
        {
            AddQueue.Enqueue(ToAdd);
        }
        /// <summary>
        /// Removes an entitiy from the list.
        /// The entity will be removed at the end of the frame.
        /// </summary>
        /// <param name="ToRemove"> The entity that will be removed. </param>
        public void RemoveEntity(Entity ToRemove)
        {
            RemoveQueue.Enqueue(ToRemove);
        }

        /// <summary>
        /// Updates the positions of the objects. 
        /// If this is called recursively it will deadlock. 
        /// </summary>
        private void UpdateEndCallback(Engine Sender, EventArgs Args)
        {
            lock(SyncObject)
            {
                Entity e;
                while(AddQueue.TryDequeue(out e))
                {
                    Entities.Add(e);
                }

                while(RemoveQueue.TryDequeue(out e))
                {
                    Entities.Remove(e);
                }

                foreach(Entity Ent in Entities)
                {
                    Ent.UpdateTransform();
                }
            }
        }

        /// <summary>
        /// Registers callbacks within the engine.
        /// </summary>
        /// <param name="Eng"> The engine with which to register the callback. </param>
        public void Register(Engine Eng)
        {
            Eng.OnUpdateEnd += UpdateEndCallback;
        }
    }
}
