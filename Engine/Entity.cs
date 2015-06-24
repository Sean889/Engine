using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineSystem
{
    using PositionUpdate = Pair<Coord, uint>;

    public class Entity
    {
        private const uint DEFAULT_PRIORITY = 1u;

        /// <summary>
        /// Position update queue.
        /// Second part of the pair is the priority of the update.
        /// The highest priority update get applied.
        /// </summary>
        private ConcurrentQueue<PositionUpdate> UpdateQueue = new ConcurrentQueue<PositionUpdate>();
        /// <summary>
        /// Actual position object.
        /// </summary>
        private volatile Coord InternalTransform;
        /// <summary>
        /// A thread safe dictionary of the current components attached to this object.
        /// </summary>
        private ConcurrentDictionary<uint, IEntityComponent> Components = new ConcurrentDictionary<uint, IEntityComponent>();

        /// <summary>
        /// Transform property. All access to this property is thread safe.
        /// Setting this property does not have immediate effects. 
        /// If another update has a higher priority then that update will be used.
        /// To set the priority use the SetTransform method.
        /// </summary>
        public Coord Transform
        {
            get
            {
                return InternalTransform;
            }
            set
            {
                SetTransform(value, DEFAULT_PRIORITY);
            }
        }

        /// <summary>
        /// Adds a transform update to be set at the end of the frame at the given priority.
        /// </summary>
        /// <param name="newTransform"> The new transform. </param>
        /// <param name="priority"> The priority of the update. The highest priority update gets applied. </param>
        public void SetTransform(Coord newTransform, uint priority)
        {
            UpdateQueue.Enqueue(new PositionUpdate(newTransform, priority));
        }
        /// <summary>
        /// Adds a transform update at DEFAULT_PRIORITY priority.
        /// </summary>
        /// <param name="newTransform"> The new transform. </param>
        public void SetTransform(Coord newTransform)
        {
            SetTransform(newTransform, DEFAULT_PRIORITY);
        }

        /// <summary>
        /// Changes the transform to the heighest priority update.
        /// </summary>
        public void UpdateTransform()
        {
            PositionUpdate update, prev;

            if(UpdateQueue.TryDequeue(out prev))
            {
                while(UpdateQueue.TryDequeue(out update))
                {
                    if(prev.second <= update.second)
                    {
                        prev = update;
                    }
                }

                InternalTransform = prev.first;
            }
        }

        /// <summary>
        /// Gets the component with the key value.
        /// </summary>
        /// <param name="idx"> The key value of the component. </param>
        /// <returns> The component or null if it wasn't found. </returns>
        public IEntityComponent GetComponent(uint idx)
        {
            IEntityComponent OutComponent = null;
            if (Components.TryGetValue(idx, out OutComponent))
                return OutComponent;
            return null;
        }
        /// <summary>
        /// Gets the component with the key value and casts it to the desired component type.
        /// </summary>
        /// <param name="idx"> The key value of the component. </param>
        /// <returns> The component or null if it wasn't found or couldn't be cast to the type. </returns>
        public T GetComponent<T>(uint idx) where T: class, IEntityComponent
        {
            IEntityComponent Component;
            if (Components.TryGetValue(idx, out Component))
            {
                try
                {
                    return (T)Component;
                }
#pragma warning disable 168
                catch(InvalidCastException e)
                {

                }
            }

            return null;
        }
        /// <summary>
        /// Attempts to add the component to the object.
        /// It should work fine if there aren't any conflicting component Ids.
        /// You can't add a type of component more than once.
        /// </summary>
        /// <param name="Component"> The Component to add. </param>
        /// <returns> Whether the operation succeded. </returns>
        public bool AddComponent(IEntityComponent Component)
        {
            uint key = Component.GetID();
            Component.OnCreate(this);
            return Components.TryAdd(key, Component);
        }
    }
}
