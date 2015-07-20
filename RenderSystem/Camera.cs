using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngineSystem;
using EngineSystem.Messaging;

namespace RenderSystem
{
    /// <summary>
    /// Makes the entity this is attached to a camera.
    /// </summary>
    public class CameraComponent : IEntityComponent, ICamera
    {
        private EventManager EventManager;

        /// <summary>
        /// The Entity that this component is attached to.
        /// </summary>
        public Entity ParentEntity;
        /// <summary>
        /// The field of view on the Y axis of this camera.
        /// </summary>
        public double Fovy;
        /// <summary>
        /// The aspect of the camera's view port.
        /// </summary>
        public double Aspect;
        /// <summary>
        /// The distance from the camera to the near plane.
        /// </summary>
        public double NearZ;
        /// <summary>
        /// The distance from the camera to the far plane.
        /// </summary>
        public double FarZ;

        /// <summary>
        /// Gets the ID of the camera component.
        /// </summary>
        /// <returns> 52. </returns>
        public uint GetID()
        {
            return 52;
        }

        /// <summary>
        /// Sets the parent entity to the one the camera component has been attached to.
        /// </summary>
        /// <param name="e"></param>
        public void OnCreate(Entity e)
        {
            ParentEntity = e;
        }

        /// <summary>
        /// Called when the component is removed from the Entity.
        /// </summary>
        /// <param name="e"></param>
        public void OnRemove(Entity e)
        {
            ParentEntity = null;
        }

        /// <summary>
        /// Called when the camera is set as the active camera.
        /// </summary>
        /// <param name="Sys"></param>
        void ICamera.OnActive(GraphicsSystem Sys)
        {
            EventManager = Engine.CurrentEngine.EventManager;

            EventManager.AddEventHandler(WindowResizeEvent.Id, OnResize);
        }
        /// <summary>
        /// Called when the camera is no longer the active camera.
        /// </summary>
        /// <param name="Sys"></param>
        void ICamera.OnDeactive(GraphicsSystem Sys)
        {
            EventManager.RemoveEventHandler(WindowResizeEvent.Id, OnResize);
        }

        private void OnResize(Object Sender, IEvent e)
        {
            if(e.GetID() == WindowResizeEvent.Id)
            {
                WindowResizeEvent Event = e as WindowResizeEvent;

                Aspect = (double)Event.SizeX / (double)Event.SizeY;
            }
        }

        /// <summary>
        /// Returns the trasform of the camera.
        /// </summary>
        /// <returns></returns>
        public Coord GetTransform()
        {
            return ParentEntity.Transform;
        }

        /// <summary>
        /// Returns the projection matrix of the camera.
        /// </summary>
        /// <returns> The projection matrix for the camera. </returns>
        public OpenTK.Matrix4d GetProjMat()
        {
            return OpenTK.Matrix4d.Perspective(Fovy, Aspect, NearZ, FarZ);
        }

        /// <summary>
        /// Creates the camera.
        /// </summary>
        /// <param name="Aspect"> The aspect ratio of the camera. </param>
        /// <param name="Fovy"> The field of view on the Y axis. </param>
        /// <param name="NearZ"> The near plane. </param>
        /// <param name="FarZ"> The far plane. </param>
        public CameraComponent(double Aspect, double Fovy, double NearZ, double FarZ)
        {
            this.Aspect = Aspect;
            this.Fovy = Fovy;
            this.NearZ = NearZ;
            this.FarZ = FarZ;
        }
    }
}
