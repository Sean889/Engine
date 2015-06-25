
namespace EngineSystem
{
    /// <summary>
    /// Interface for object components.
    /// </summary>
    public interface IEntityComponent
    {
        /// <summary>
        /// Gets a unique ID for this class type.
        /// </summary>
        /// <returns> A unique ID for this component type. </returns>
        uint GetID();

        /// <summary>
        /// Called when the component is added to an Entity
        /// </summary>
        void OnCreate(Entity e);
        /// <summary>
        /// Called when the component is removed from an Entity.
        /// </summary>
        void OnRemove(Entity e);
    }
}
