using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using EngineSystem;

namespace RenderSystem
{
    /// <summary>
    /// Camera interface.
    /// </summary>
    public interface ICamera
    {
        /// <summary>
        /// Called when the camera becomes the currently active camera.
        /// </summary>
        /// <param name="Sys"></param>
        void OnActive(GraphicsSystem Sys);
        /// <summary>
        /// Called when the camera stops being the active camera.
        /// </summary>
        /// <param name="Sys"></param>
        void OnDeactive(GraphicsSystem Sys);

        /// <summary>
        /// Should return the transform of the camera.
        /// </summary>
        /// <returns></returns>
        Coord GetTransform();
        /// <summary>
        /// Should return the projection matrix for the camera.
        /// </summary>
        /// <returns></returns>
        Matrix4d GetProjMat();
    }
}
