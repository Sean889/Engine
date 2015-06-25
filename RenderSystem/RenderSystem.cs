using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngineSystem;
using OpenTK;

namespace RenderSystem
{
    public class GraphicsSystem : ISystem
    {
        private RenderThread Thread;
        private List<GraphicsComponent> Components = new List<GraphicsComponent>();
        private ConcurrentQueue<GraphicsComponent> ToAdd = new ConcurrentQueue<GraphicsComponent>();
        private ConcurrentQueue<GraphicsComponent> ToRemove = new ConcurrentQueue<GraphicsComponent>();

        void ISystem.Register(Engine Target)
        {
            Thread.SetEngine(Target);

            Target.OnUpdate += Update;
            Target.OnUpdateEnd += UpdateEnd;
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
            }

            while (ToRemove.TryDequeue(out Component))
            {
                Components.Remove(Component);
            }
        }

        public RenderThread RenderThread
        {
            get
            {
                return Thread;
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
