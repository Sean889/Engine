using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// Called when the component is removed.
        /// </summary>
        void OnRemove();
    }
}
