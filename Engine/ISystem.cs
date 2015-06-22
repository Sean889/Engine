using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineSystem
{
    /// <summary>
    /// Interface to be used by all systems.
    /// </summary>
    public interface ISystem
    {
        /// <summary>
        /// Allows the system to register itself with the callbacks within the Engine.
        /// </summary>
        /// <param name="Target"> The engine to register with. </param>
        void Register(Engine Target);
    }
}
