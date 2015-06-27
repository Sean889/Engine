using EngineSystem;
using OpenTK;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace RenderSystem
{
    /// <summary>
    /// The system that manages the rendering of all graphics components.
    /// </summary>
    public class GraphicsSystem : ISystem
    {
        private RenderThread Thread;
        private List<GraphicsComponent> Components = new List<GraphicsComponent>();
        private ConcurrentQueue<GraphicsComponent> ToAdd = new ConcurrentQueue<GraphicsComponent>();
        private ConcurrentQueue<GraphicsComponent> ToRemove = new ConcurrentQueue<GraphicsComponent>();
        private ICamera CurrentCamera;
        private ConcurrentQueue<ICamera> CameraUpdates = new ConcurrentQueue<ICamera>();
        internal Engine Engine;

        void ISystem.Register(Engine Target)
        {
            Thread.SetEngine(Target);

            Target.OnUpdate += Update;
            Target.OnUpdateEnd += UpdateEnd;

            Engine = Target;
        }

        private void Update(Engine e, UpdateEventArgs Args)
        {
            //TODO: Replace with some sort of parallel for iteration.
            foreach(GraphicsComponent Component in Components)
            {
                Thread.ScheduleRenderTask(Component.GetDrawAction());
            }
        }
        private void UpdateEnd(Engine e, EventArgs Args)
        {
            Thread.SwapFrames();

            GraphicsComponent Component;
            while(ToAdd.TryDequeue(out Component))
            {
                Components.Add(Component);
                Component.__OnRenderAdd(this);
            }

            while (ToRemove.TryDequeue(out Component))
            {
                Components.Remove(Component);
                Component.__OnRenderRemove(this);
            }

            ICamera NewCam;
            if (CameraUpdates.TryDequeue(out NewCam))
            {
                do { } while (CameraUpdates.TryDequeue(out NewCam));

                NewCam.OnActive(this);
                CurrentCamera.OnDeactive(this);
                CurrentCamera = NewCam;
            }
        }

        /// <summary>
        /// The thread that rendering commands are submitted to.
        /// </summary>
        public RenderThread GraphicsThread
        {
            get
            {
                return Thread;
            }
        }
        /// <summary>
        /// The current camera. Sets do not take place until the end of the frame.
        /// </summary>
        public ICamera Camera
        {
            get
            {
                return CurrentCamera;
            }
            set
            {
                CameraUpdates.Enqueue(value);
            }
        }

        /// <summary>
        /// Creates a GraphicsSystem using the given window.
        /// </summary>
        /// <param name="Win"></param>
        public GraphicsSystem(GameWindow Win)
        {
            Thread = new RenderThread(Win);
        }

        /// <summary>
        /// Adds a component. The component will be added at the end of the frame.
        /// </summary>
        /// <param name="NewComponent"> The component to add. </param>
        public void AddGraphicsComponent(GraphicsComponent NewComponent)
        {
            ToAdd.Enqueue(NewComponent);
        }
        /// <summary>
        /// Removes a component. The component will be removed at the end of the frame.
        /// </summary>
        /// <param name="Component"> The component to be removed. </param>
        public void RemoveGraphicsComponent(GraphicsComponent Component)
        {
            ToRemove.Enqueue(Component);
        }

        /// <summary>
        /// Allows iteration over all the attached components.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GraphicsComponent> GetComponents()
        {
            return Components;
        }
    }
}
