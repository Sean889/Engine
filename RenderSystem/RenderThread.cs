using EngineSystem;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Concurrent;
using System.Threading;
using EngineSystem.Messaging;

namespace RenderSystem
{
    /// <summary>
    /// A thread that executes all the rendering tasks.
    /// </summary>
    public class RenderThread : IDisposable
    {
        /// <summary>
        /// The number of miliseconds that the rendering thread will wait before checking if there is a new frame.
        /// If the thread is notified the wait time will be less.
        /// </summary>
        private const int THREAD_WAIT = 100;

        private ConcurrentQueue<Action<GameWindow>> WindowTasks = new ConcurrentQueue<Action<GameWindow>>();
        private ConcurrentQueue<ConcurrentQueue<Action>> FrameQueue = new ConcurrentQueue<ConcurrentQueue<Action>>();

        private ConcurrentQueue<Action> CurrentQueue;
        private volatile bool TerminateFlag = false;
        private GameWindow Window;
        private Thread ExecutorThread;
        private EventManager EventDispatcher;

        /// <summary>
        /// The Event used to signal that a frame is done.
        /// </summary>
        private AutoResetEvent Event = new AutoResetEvent(false); 
        
        /// <summary>
        /// Adds a task that will be executed by the rendering thread.
        /// </summary>
        /// <param name="Task"> The task to be executed. </param>
        public void ScheduleRenderTask(Action Task)
        {
            CurrentQueue.Enqueue(Task);
        }
        
        /// <summary>
        /// These tasks are not guarenteed to 
        /// </summary>
        /// <param name="WindowTask"></param>
        public void ScheduleWindowTask(Action<GameWindow> WindowTask)
        {
            WindowTasks.Enqueue(WindowTask);
        }

        /// <summary>
        /// Changes frames so rendering updates are pushed to the new frame.
        /// This should only be called once per frame
        /// </summary>
        public void SwapFrames()
        {
            //Swap the Current queue and add the previous queue to the framebuffer
            FrameQueue.Enqueue(Interlocked.Exchange<ConcurrentQueue<Action>>(ref CurrentQueue, new ConcurrentQueue<Action>()));
            //Notify the rendering thread
            Event.Set();
        }

        /// <summary>
        /// Creates the rendering thread using the given GameWindow.
        /// </summary>
        /// <param name="Window"> The window that the rendering thread will use. </param>
        public RenderThread(GameWindow Window)
        {
            this.Window = Window;
            Init();
        }

        internal void SetEngine(Engine e)
        {
            EventDispatcher = e.EventManager;
        }

        private void ThreadExecutor()
        {
            Interlocked.MemoryBarrier();

            Window.MakeCurrent();

            //Fired when the window resizes
            Window.Resize += (sender, e) =>
                {
                    GL.Viewport(Window.Size);
                    EventDispatcher.FireEvent(new WindowResizeEvent(Window.Size.Width, Window.Size.Height));
                };

            //Fired when the window is moved
            Window.Move += (sender, e) => EventDispatcher.FireEvent(new WindowMovedEvent(Window.Location.X, Window.Location.Y));

            //Key events
            Window.KeyPress += (sender, e) => EventDispatcher.FireEvent(new KeyPressedEvent(e));
            Window.KeyDown += (sender, e) => EventDispatcher.FireEvent(new KeyDownEvent(e));
            Window.KeyUp += (sender, e) => EventDispatcher.FireEvent(new KeyUpEvent(e));

            //Mouse events
            Window.MouseEnter += (sender, e) => EventDispatcher.FireEvent(new MouseEnterEvent());
            Window.MouseLeave += (sender, e) => EventDispatcher.FireEvent(new MouseLeaveEvent());
            Window.MouseDown += (sender, e) => EventDispatcher.FireEvent(new MouseDownEvent(e));
            Window.MouseUp += (sender, e) => EventDispatcher.FireEvent(new MouseUpEvent(e));
            Window.MouseWheel += (sender, e) => EventDispatcher.FireEvent(new MouseWheelEvent(e));
            Window.MouseMove += (sender, e) => EventDispatcher.FireEvent(new MouseMoveEvent(e));

            while(!TerminateFlag)
            {
                Event.WaitOne(THREAD_WAIT);

                ConcurrentQueue<Action> Frame;

                if (FrameQueue.TryDequeue(out Frame))
                {
                    while (FrameQueue.TryDequeue(out Frame)) ;

                    Action Act;
                    while (Frame.TryDequeue(out Act))
                    {
                        Act();
                    }

                    Action<GameWindow> WindowTask;
                    while (WindowTasks.TryDequeue(out WindowTask))
                    {
                        WindowTask(Window);
                    }
                    Window.SwapBuffers();
                    Window.ProcessEvents();
                }
            }
        }

        private void Init()
        {
            CurrentQueue = new ConcurrentQueue<Action>();
            ExecutorThread = new Thread(new ThreadStart(ThreadExecutor));
        }
        private void Terminate()
        {
            TerminateFlag = true;
            ExecutorThread.Join();
        }

        public void Dispose()
        {
            Terminate();
        }
    }
}
