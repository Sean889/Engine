using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanetLib
{
    /// <summary>
    /// Interface for a planet.
    /// </summary>
    public interface IPlanet
    {
        /// <summary>
        /// Allows for the planet to update itself.
        /// </summary>
        void Update();
    }
}
