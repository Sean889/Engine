using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RenderSystem;
using EngineSystem;

namespace PlanetLib
{
    /// <summary>
    /// Manages a planet and renders it.
    /// </summary>
    public class PlanetGraphicsComponent : GraphicsComponent
    {
        private Entity ParentEntity;

        /// <summary>
        /// Returns a action that will draw the planet.
        /// </summary>
        /// <returns></returns>
        public override Action GetDrawAction()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns 61 as the id.
        /// </summary>
        /// <returns> 61. </returns>
        public override uint GetID()
        {
            return 61;
        }

        /// <summary>
        /// Sets the ParentEntity to the currently attached entity
        /// </summary>
        /// <param name="e"></param>
        public override void OnCreate(Entity e)
        {
            ParentEntity = e;
        }
    }
}
