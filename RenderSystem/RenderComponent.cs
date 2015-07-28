using EngineSystem;
using System;

namespace RenderSystem
{
    /// <summary>
    /// An abstract base class for all graphics components.
    /// </summary>
    public abstract class GraphicsComponent : IEntityComponent
    {
        /// <summary>
        /// The parent of this graphics component
        /// </summary>
        protected GraphicsSystem ParentSystem;

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

        /// <summary>
        /// Called when the component is added to a entity.
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnCreate(Entity e) { }
        /// <summary>
        /// Called when the component is removed from an entity.
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnRemove(Entity e) { }

        /// <summary>
        /// Called when the class is added to the GraphicsSystem.
        /// </summary>
        /// <param name="Sys"></param>
        protected virtual void OnRenderAdd(GraphicsSystem Sys) { }
        /// <summary>
        /// Called when the class is removed from the GraphicsSystem.
        /// </summary>
        /// <param name="Sys"></param>
        protected virtual void OnRenderRemove(GraphicsSystem Sys) { }

        void IEntityComponent.OnRemove(Entity e)
        {
            ParentSystem.RemoveGraphicsComponent(this);
            this.OnRemove(e);
        }

        /// <summary>
        /// An internal method called by the graphics system when the GraphicsComponent is added to the graphics system.
        /// It calls OnRenderAdd and sets the ParentSystem member.
        /// </summary>
        /// <param name="Sys"></param>
        internal void __OnRenderAdd(GraphicsSystem Sys)
        {
            ParentSystem = Sys;
            OnRenderAdd(Sys);
        }
        /// <summary>
        /// An internal method called by the graphics system when the GraphicsComponent is removed from the graphics system.
        /// </summary>
        /// <param name="Sys"></param>
        internal void __OnRenderRemove(GraphicsSystem Sys)
        {
            OnRenderRemove(Sys);
            ParentSystem = null;
        }
    }
}
