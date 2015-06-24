using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using OpenTK.Graphics;
using EngineSystem;
using ThreadingUtils;
using System.Drawing;

namespace RenderSystem
{
    /// <summary>
    /// A thread that executes all the rendering tasks.
    /// </summary>
    public class RenderThread
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

        public delegate void TEventFunc<TArgs>(Object Obj, TArgs Args);

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
        /// Creates a window with the given width and height.
        /// </summary>
        /// <param name="Width"> The width of the window in pixels. </param>
        /// <param name="Height"> The height of the window in pixels. </param>
        public RenderThread(int Width, int Height)
        {
            Window = new GameWindow(Width, Height);
        }
        /// <summary>
        /// Creates a window with the given width, height and title.
        /// </summary>
        /// <param name="Width"> The width of the window in pixels. </param>
        /// <param name="Height"> The height of the window in pixels. </param>
        /// <param name="Title"> The title of the window. </param>
        public RenderThread(int Width, int Height, string Title)
        {
            Window = new GameWindow(Width, Height, new GraphicsMode(), Title);
            Init();
        }
        /// <summary>
        /// Creates a rendering thread and window with the given width, height, title and OpenGL context version.
        /// </summary>
        /// <param name="Width"> The width of the window. </param>
        /// <param name="Height"> The height of the window. </param>
        /// <param name="Title"> The title of the window. </param>
        /// <param name="GLMajorVersion"> The major version of the OpenGL context. </param>
        /// <param name="GLMinorVersion"> The minor version of the OpenGL context. </param>
        public RenderThread(int Width, int Height, string Title, int GLMajorVersion, int GLMinorVersion)
        {
            Window = new GameWindow(Width, Height, new GraphicsMode(), Title, GameWindowFlags.Default, DisplayDevice.Default, GLMajorVersion, GLMinorVersion, GraphicsContextFlags.Default);
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

        private void ThreadExecutor()
        {
            Interlocked.MemoryBarrier();

            Window.MakeCurrent();

            Window.Resize += (sender, e) =>
                {
                    GL.Viewport(Window.Size);
                };

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
    }
}
