using EngineSystem;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using System;
using System.Collections.Concurrent;
using System.Threading;
using EngineSystem.Messaging;

namespace RenderSystem
{
    using WindowType = GameWindow;

    /// <summary>
    /// The parameters governing what type of game window is created on the rendering thread.
    /// </summary>
    public struct WindowParams
    {
        /// <summary>
        /// The height of the window in pixels.
        /// </summary>
        public int Height;
        /// <summary>
        /// The width of the window in pixels.
        /// </summary>
        public int Width;
        /// <summary>
        /// The titke of the window.
        /// </summary>
        public string Title;
        /// <summary>
        /// The major version of the created OpenGL context.
        /// </summary>
        public int GLMajorVersion;
        /// <summary>
        /// Thi minor version of the OpenGL context.
        /// </summary>
        public int GLMinorVersion;
        /// <summary>
        /// Flags governing the type of the window.
        /// </summary>
        public GameWindowFlags WindowFlags;
        /// <summary>
        /// Flags governing the type of context that will be created.
        /// </summary>
        public GraphicsContextFlags ContextFlags;
        /// <summary>
        /// The graphics mode of the window.
        /// </summary>
        public GraphicsMode Mode;
        /// <summary>
        /// The DisplayDevice that the window will be displayed on.
        /// </summary>
        public DisplayDevice Device;

        /// <summary>
        /// Creates a Window with the desired width and height.
        /// </summary>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        public WindowParams(int Width, int Height)
           : this(Width, Height, "Game Window")
        {

        }
        /// <summary>
        /// Creates a window with the given width, height and title.
        /// </summary>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <param name="Title"></param>
        public WindowParams(int Width, int Height, string Title)
            : this(Width, Height, Title, 3, 0)
        {

        }
        /// <summary>
        /// Creates a window with the given width, height, title, and OpenGL context version.
        /// </summary>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <param name="Title"></param>
        /// <param name="GLMajorVersion"></param>
        /// <param name="GLMinorVersion"></param>
        public WindowParams(int Width, int Height, string Title, int GLMajorVersion, int GLMinorVersion)
            : this(Width, Height, Title, GLMajorVersion, GLMinorVersion, GameWindowFlags.Default)
        {

        }
        /// <summary>
        /// Creates a window with the given width, height, title, OpenGL context version and GameWindowFlags.
        /// </summary>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <param name="Title"></param>
        /// <param name="GLMajorVersion"></param>
        /// <param name="GLMinorVersion"></param>
        /// <param name="Flags"></param>
        public WindowParams(int Width, int Height, string Title, int GLMajorVersion, int GLMinorVersion, GameWindowFlags Flags)
        {
            this.Width = Width;
            this.Height = Height;
            this.Title = Title;
            this.GLMajorVersion = GLMajorVersion;
            this.GLMinorVersion = GLMinorVersion;
            this.WindowFlags = Flags;
            this.ContextFlags = GraphicsContextFlags.Default;
            this.Mode = GraphicsMode.Default;
            this.Device = DisplayDevice.Default;
        }

        internal WindowType Window
        {
            get
            {
                return new WindowType(Width, Height, Mode, Title, WindowFlags, Device, GLMajorVersion, GLMinorVersion, ContextFlags);
            }
        }
    }

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

        private ConcurrentQueue<Action<WindowType>> WindowTasks = new ConcurrentQueue<Action<WindowType>>();
        private ConcurrentQueue<ConcurrentQueue<Action>> FrameQueue = new ConcurrentQueue<ConcurrentQueue<Action>>();
        private ConcurrentQueue<Action> EssentialRenderTasks = new ConcurrentQueue<Action>();

        private ConcurrentQueue<Action> CurrentQueue;
        private volatile bool TerminateFlag = false;
        private GameWindow InternalWindow;
        private Thread ExecutorThread;
        private EventManager EventDispatcher;
        private WindowParams Info;

        /// <summary>
        /// The OpenGL context attached to this rendering thread.
        /// </summary>
        public IGraphicsContext Context
        {
            get
            {
                return Window.Context;
            }
        }
        /// <summary>
        /// The window that this thread is using.
        /// </summary>
        public WindowType Window
        {
            get
            {
                return InternalWindow;
            }
        }

        /// <summary>
        /// The Event used to signal that a frame is done.
        /// </summary>
        private AutoResetEvent Event = new AutoResetEvent(false); 
        
        /// <summary>
        /// Adds a task that will be executed by the rendering thread.
        /// These tasks are not guarenteed to be executed. 
        /// If a task needs to be executed use ScheduleEssentialRenderTask instead.
        /// </summary>
        /// <param name="Task"> The task to be executed. </param>
        public void ScheduleRenderTask(Action Task)
        {
            CurrentQueue.Enqueue(Task);
        }
        /// <summary>
        /// Schedules a task that is guareteed to be executed by the rendering thread.
        /// </summary>
        /// <param name="Task"></param>
        public void ScheduleEssentialRenderTask(Action Task)
        {
            EssentialRenderTasks.Enqueue(Task);
        }
        
        /// <summary>
        /// These tasks are not guarenteed to 
        /// </summary>
        /// <param name="WindowTask"></param>
        public void ScheduleWindowTask(Action<WindowType> WindowTask)
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
        /// <param name="Info"> A group of parameters for the window. </param>
        public RenderThread(WindowParams Info)
        {
            this.Info = Info;
        }

        internal void SetEngine(Engine e)
        {
            EventDispatcher = e.EventManager;
            Init();
        }

        private void ThreadExecutor()
        {
            this.InternalWindow = Info.Window;

            Interlocked.MemoryBarrier();

            InternalWindow.MakeCurrent();

            //Fired when the window resizes
            Window.Resize += (sender, e) =>
                {
                    GL.Viewport(Window.Size);
                    EventDispatcher.FireEvent(new WindowResizeEvent(Window.Size.Width, Window.Size.Height));
                };

            //Fired when the window is moved
            Window.Move += (sender, e) => EventDispatcher.FireEvent(new WindowMovedEvent(Window.Location.X, Window.Location.Y));

            //Key events
            Window.KeyPress += (sender, e) => 
                EventDispatcher.FireEvent(new KeyPressedEvent(e));
            Window.KeyDown += (sender, e) => EventDispatcher.FireEvent(new KeyDownEvent(e));
            Window.KeyUp += (sender, e) => EventDispatcher.FireEvent(new KeyUpEvent(e));

            //Mouse events
            Window.MouseEnter += (sender, e) => EventDispatcher.FireEvent(new MouseEnterEvent());
            Window.MouseLeave += (sender, e) => EventDispatcher.FireEvent(new MouseLeaveEvent());
            Window.MouseDown += (sender, e) => EventDispatcher.FireEvent(new MouseDownEvent(e));
            Window.MouseUp += (sender, e) => EventDispatcher.FireEvent(new MouseUpEvent(e));
            Window.MouseWheel += (sender, e) => EventDispatcher.FireEvent(new MouseWheelEvent(e));
            Window.MouseMove += (sender, e) => EventDispatcher.FireEvent(new MouseMoveEvent(e));
            
            InternalWindow.RenderFrame += (sender, e) =>
                {
                    if (TerminateFlag)
                    {
                        Window.Close();
                        return;
                    }

                    Event.WaitOne(THREAD_WAIT);

                    ConcurrentQueue<Action> NewFrame;

                    if (FrameQueue.TryDequeue(out NewFrame))
                    {
                        ConcurrentQueue<Action> Frame = NewFrame;
                        while (FrameQueue.TryDequeue(out NewFrame)) Frame = NewFrame;

                        Action<WindowType> WindowTask;
                        while (WindowTasks.TryDequeue(out WindowTask))
                        {
                            WindowTask(Window);
                        }

                        Action Act;
                        while(EssentialRenderTasks.TryDequeue(out Act))
                        {
                            Act();
                        }

                        while (Frame.TryDequeue(out Act))
                        {
                            Act();
                        }

                        Window.SwapBuffers();
                    }
                };

            InternalWindow.Run();

            InternalWindow.Dispose();
        }

        private void Init()
        {
            CurrentQueue = new ConcurrentQueue<Action>();
            ExecutorThread = new Thread(new ThreadStart(ThreadExecutor));
            ExecutorThread.Start();
        }
        private void Terminate()
        {
            TerminateFlag = true;
            ExecutorThread.Join();
        }

        /// <summary>
        /// Stops the rendering thread.
        /// </summary>
        public void Dispose()
        {
            Terminate();
        }
    }
}
