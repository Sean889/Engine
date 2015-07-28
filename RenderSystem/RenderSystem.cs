using EngineSystem;
using OpenTK;
using OpenTK.Graphics.OpenGL;
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
        private OpenTK.Graphics.Color4 InternalBackgroundColour = new OpenTK.Graphics.Color4(1, 1, 1, 1);

        void ISystem.Register(Engine Target)
        {
            Thread.SetEngine(Target);

            Target.OnUpdate += Update;
            Target.OnUpdateEnd += UpdateEnd;
            Target.OnDispose += OnEngineDispose;
        }
        void ISystem.Unregister(Engine Target)
        {
            Target.OnUpdate -= Update;
            Target.OnUpdateEnd -= UpdateEnd;
            Target.OnDispose -= OnEngineDispose;
        }

        private void OnEngineDispose(Engine Sender, EventArgs Args)
        {
            foreach(GraphicsComponent Component in Components)
            {
                Component.__OnRenderRemove(this);
            }

            Components.Clear();

            Thread.Dispose();
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
                ICamera PrevCam;
                do { PrevCam = NewCam; } while (CameraUpdates.TryDequeue(out NewCam));

                PrevCam.OnActive(this);
                if(CurrentCamera != null)
                    CurrentCamera.OnDeactive(this);
                CurrentCamera = PrevCam;
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
        /// Gets or sets the clear colour of the window
        /// </summary>
        public OpenTK.Graphics.Color4 ClearColour
        {
            set
            {
                InternalBackgroundColour = value;
                Thread.ScheduleEssentialRenderTask(new Action(delegate
                {
                    GL.ClearColor(value);
                }));
            }
            get
            {
                return InternalBackgroundColour;
            }
        }

        /// <summary>
        /// Creates a GraphicsSystem. Due to operating system limitations the GameWindow must be created on the rendering thread.
        /// </summary>
        /// <param name="Win"> A struct containing all the parameters for creating the window. </param>
        public GraphicsSystem(WindowParams Win)
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
