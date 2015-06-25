using EngineSystem;
using System;

namespace RenderSystem
{
    public abstract class GraphicsComponent : IEntityComponent
    {
        /// <summary>
        /// Returns a delegate that will be executed on the rendering thread.
        /// </summary>
        /// <returns></returns>
        public abstract Action GetDrawAction();

        /// <summary>
        /// If the ID is not specified in the derived implementation. This class will use ID 50.
        /// </summary>
        /// <returns> A component ID. </returns>
        public virtual uint GetID()
        {
            return 50;
        }

        public abstract void OnCreate(Entity e);
        public abstract void OnRemove(Entity e);
    }
}
