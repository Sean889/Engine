using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace ThreadPool
{
    /// <summary>
    /// Class that manages the thread pool.
    /// </summary>
    public class ThreadPoolManager
    {
        /// <summary>
        /// A queue containing all the tasks.
        /// </summary>
        private static ConcurrentQueue<PromiseBase> TaskQueue = new ConcurrentQueue<PromiseBase>();
        /// <summary>
        /// A queue containing all the background tasks.
        /// </summary>
        private static ConcurrentQueue<PromiseBase> BackgroundTaskQueue = new ConcurrentQueue<PromiseBase>();
        /// <summary>
        /// A list of all the threads in the thread pool.
        /// </summary>
        private static List<Thread> Threads = new List<Thread>();
        /// <summary>
        /// A flag to indicate to worker threads that they should exit.
        /// </summary>
        private static AtomicBoolean TerminateFlag = false;
        /// <summary>
        /// An event used to signal to worker threads that there is work to be done.
        /// </summary>
        private static AutoResetEvent Event = new AutoResetEvent(false);
        /// <summary>
        /// An event used to signal to the background task worker thread that there is work to be done.
        /// </summary>
        private static AutoResetEvent BackgroundEvent = new AutoResetEvent(false);

        /// <summary>
        /// Maximum amount of time a thread will wait before checking whether there is work to do.
        /// </summary>
        private const int THREAD_WAIT = 20;

        /// <summary>
        /// A helper class for thread pool termination.
        /// </summary>
        private class BoolFlag
        {
            internal AtomicBoolean EndFlag = false;
        }

        /// <summary>
        /// Initializes with one less than the number of logical threads for the processor. 
        /// </summary>
        public static void Init()
        {
            Init(Environment.ProcessorCount - 1);
        }
        /// <summary>
        /// Initializes the thread pool with a given number of threads.
        /// </summary>
        /// <param name="NumThreads"> The number of threads to initialize the thread pool with. </param>
        public static void Init(int NumThreads)
        {
            NumThreads = NumThreads < 1 ? 1 : NumThreads;

            TaskQueue = new ConcurrentQueue<PromiseBase>();
            BackgroundTaskQueue = new ConcurrentQueue<PromiseBase>();
            Threads = new List<Thread>();
            TerminateFlag = false;
            Event = new AutoResetEvent(false);
            BackgroundEvent = new AutoResetEvent(false);

            {
                // Create the background thread
                Thread t = new Thread(new ThreadStart(BackgroundThreadExecutor));
                t.Start();
                Threads.Add(t);
            }

            for(int i = 1; i < NumThreads; i++)
            {
                //Create the normal threadpool threads
                Thread t = new Thread(new ThreadStart(ThreadExecutor));
                t.Start();
                Threads.Add(t);
            }
        }
        /// <summary>
        /// Shut down all threads in the threadpool and executes any remaining tasks.
        /// </summary>
        public static void Terminate()
        {
            TerminateFlag.FalseToTrue();
            BoolFlag Flag = new BoolFlag();
            Thread EndThread = new Thread(new ThreadStart(delegate
                {
                    while (!Flag.EndFlag.Value)
                    {
                        Event.Set();
                    }
                }));
            EndThread.Start();
            foreach(Thread t in Threads)
            {
                t.Join();
            }

            Flag.EndFlag.FalseToTrue();
            EndThread.Join();

            PromiseBase Promise;
            while(TaskQueue.TryDequeue(out Promise))
            {
                Promise.Execute();
            }

            while(BackgroundTaskQueue.TryDequeue(out Promise))
            {
                Promise.Execute();
            }

            TaskQueue = null;
            BackgroundTaskQueue = null;
            Threads = null;
            Event = null;
            BackgroundEvent = null;
        }

        /// <summary>
        /// Executes a single task in the threadpool.
        /// </summary>
        public static void ExecuteSingleTask()
        {
            PromiseBase Promise;
            if(TaskQueue.TryDequeue(out Promise))
            {
                Promise.Execute();
            }
            else if(BackgroundTaskQueue.TryDequeue(out Promise))
            {
                Promise.Execute();
            }
        }

        /// <summary>
        /// Enqueues a task to be executed by the thread pool.
        /// </summary>
        /// <param name="Task"> The task to be executed. </param>
        /// <returns> A future representing the task. </returns>
        public static Future QueueAsync(Action Task)
        {
            Promise p = new Promise(Task);
            TaskQueue.Enqueue(p);
            Event.Set();
            return new Future(p);
        }
        /// <summary>
        /// Enqueues a task to be executed by the thread pool.
        /// </summary>
        /// <param name="Task"> The task to be executed. </param>
        /// <returns> A future representing the task. </returns>
        public static Future<TRet> QueueAsync<TRet>(Func<TRet> Task)
        {
            Promise<TRet> p = new Promise<TRet>(Task);
            TaskQueue.Enqueue(p);
            Event.Set();
            return new Future<TRet>(p);
        }

        /// <summary>
        /// Enqueues a task that will be executed without returning a Future.
        /// </summary>
        /// <param name="Task"> The task to be executed. </param>
        public static void QueueAutoTask(Action Task)
        {
            Promise p = new Promise(Task);
            TaskQueue.Enqueue(p);
            Event.Set();
        }
        /// <summary>
        /// Enqueues a background task. Background tasks are executed after all tasks have been executed.
        /// </summary>
        /// <param name="Task"></param>
        public static void QueueBackgroundTask(Action Task)
        {
            Promise p = new Promise(Task);
            BackgroundTaskQueue.Enqueue(p);
            BackgroundEvent.Set();
        }

        /// <summary>
        /// Method that a worker thread executes.
        /// </summary>
        private static void ThreadExecutor()
        {
            while(!TerminateFlag.Value)
            {
                Event.WaitOne(THREAD_WAIT);

                PromiseBase Promise;
                if (TaskQueue.TryDequeue(out Promise))
                {
                    do
                    {
                        Promise.Execute();
                    } while (TaskQueue.TryDequeue(out Promise));
                }
                else if (BackgroundTaskQueue.TryDequeue(out Promise))
                {
                    Promise.Execute();
                }
            }
        }
        /// <summary>
        /// Special thread that executes more background tasks.
        /// </summary>
        private static void BackgroundThreadExecutor()
        {
            while(!TerminateFlag.Value)
            {
                BackgroundEvent.WaitOne(THREAD_WAIT);

                PromiseBase Promise;
                while(TaskQueue.TryDequeue(out Promise))
                {
                    Promise.Execute();
                }

                while(BackgroundTaskQueue.TryDequeue(out Promise))
                {
                    Promise.Execute();
                }
            }
        }

    }

    /// <summary>
    /// Interface through which the ThreadPoolManager interfaces with the tasks.
    /// </summary>
    public interface PromiseBase
    {
        /// <summary>
        /// Called to execute the task.
        /// </summary>
        void Execute();
    }

    /// <summary>
    /// A promise where the function has no return type.
    /// </summary>
    public class Promise : PromiseBase
    {
        /// <summary>
        /// Marks whether the task has been canceled.
        /// </summary>
        internal AtomicBoolean Canceled;
        /// <summary>
        /// Marks whether the task has been executed.
        /// </summary>
        internal AtomicBoolean Complete;

        /// <summary>
        /// Stores any exception thrown by the task. 
        /// It will be rethrown when the task is queried for completeness.
        /// </summary>
        internal volatile Exception CaughtException;

        /// <summary>
        /// The function that is executed by the task.
        /// </summary>
        private Action Func;

        /// <summary>
        /// Creates the promise from the given promise.
        /// </summary>
        /// <param name="func"> The function that will be executed. </param>
        internal Promise(Action func)
        {
            Init();
            Func = func;
        }

        /// <summary>
        /// Cancels the promise.
        /// </summary>
        public void Cancel()
        {
            Canceled.FalseToTrue();
        }
        /// <summary>
        /// Executes the promise.
        /// </summary>
        public void Execute()
        {
            if(!Canceled.Value)
            {
                try
                {
                    Func();
                }
                catch(Exception e)
                {
                    CaughtException = e;
                }
                Complete.FalseToTrue();
            }
        }
        private void Init()
        {
            Canceled = false;
            Complete = false;
            CaughtException = null;
        }
    }
    /// <summary>
    /// A promise where the function returns a value.
    /// </summary>
    /// <typeparam name="TRet"> The type of the value that the function returns. </typeparam>
    public class Promise<TRet> : PromiseBase
    {
        /// <summary>
        /// Marks whether a task has been canceled.
        /// </summary>
        internal AtomicBoolean Canceled;
        /// <summary>
        /// Marks whether a task has been completed.
        /// </summary>
        internal AtomicBoolean Complete;

        /// <summary>
        /// Stores an exception if the method threw an exception.
        /// It will be rethrown when the task is queried for completeness
        /// </summary>
        internal volatile Exception CaughtException;

        /// <summary>
        /// The function that will be executed by the task.
        /// </summary>
        private Func<TRet> Func;
        /// <summary>
        /// The return value of the task
        /// </summary>
        private TRet RetValue;

        /// <summary>
        /// Creates a promise with the specified task.
        /// </summary>
        /// <param name="func"></param>
        internal Promise(Func<TRet> func)
        {
            Init();
            Func = func;
        }

        /// <summary>
        /// Cancels the task.
        /// </summary>
        public void Cancel()
        {
            Canceled.FalseToTrue();
        } 
        /// <summary>
        /// Executes the promise.
        /// </summary>
        public void Execute()
        {
            if (!Canceled.Value)
            {
                try
                {
                    RetValue = Func();
                }
                catch (Exception e)
                {
                    CaughtException = e;
                }
                Complete.FalseToTrue();
            }
        }
        private void Init()
        {
            Canceled = false;
            Complete = false;
            CaughtException = null;
        }

        /// <summary>
        /// Gets the return value of the task.
        /// If the task is not completed this will be whatever the default value is.
        /// </summary>
        public TRet ReturnValue
        {
            get
            {
                return RetValue;
            }
        }
    }

    /// <summary>
    /// An exception thrown when an attempt to complete a canceled task happens.
    /// </summary>
    [Serializable]
    public class TaskCanceledException : Exception
    {
        public TaskCanceledException() { }
        public TaskCanceledException( string message ) : base( message ) { }
        public TaskCanceledException( string message, Exception inner ) : base( message, inner ) { }
        protected TaskCanceledException( 
            System.Runtime.Serialization.SerializationInfo info, 
            System.Runtime.Serialization.StreamingContext context ) : base( info, context ) { }
    }

    /// <summary>
    /// Allows access to the promise as well as interfacing with the task system.
    /// If the future is not valid a NullReferenceException will be thrown when the state is changed.
    /// </summary>
    public struct Future
    {
        /// <summary>
        /// The promise this future refers to.
        /// </summary>
        private Promise MyPromise;

        /// <summary>
        /// A boolean value indicating whether the Future is valid.
        /// </summary>
        public bool Valid
        {
            get
            {
                return MyPromise != null;
            }
        }
        /// <summary>
        /// A boolean value idicating whether the Task has been completed.
        /// </summary>
        public bool Completed
        {
            get
            {
                return MyPromise.CaughtException != null && MyPromise.Complete.Value;
            }
        }
        /// <summary>
        /// A boolean value indicating whether the Task has been cancelled.
        /// </summary>
        public bool Canceled
        {
            get
            {
                return MyPromise.Canceled.Value;
            }
        }
        /// <summary>
        /// A boolean value indicating whether the Task threw an exception.
        /// </summary>
        public bool ThrewException
        {
            get
            {
                return MyPromise.CaughtException != null;
            }
        }

        /// <summary>
        /// Completes the Task before returning.
        /// If the task has been canceled a TaskCanceledException is thrown.
        /// </summary>
        public void Complete()
        {
            if (MyPromise.Canceled.Value)
            {
                throw new TaskCanceledException("Task was canceled.");
            }
            else
            {
                while (!MyPromise.Complete.Value)
                {
                    ThreadPoolManager.ExecuteSingleTask();
                }

                if (MyPromise.CaughtException != null)
                {
                    throw MyPromise.CaughtException;
                }
            }
        }
        /// <summary>
        /// Completes the Task before returning.
        /// If the task has been canceled a TaskCanceledException is thrown.
        /// </summary>
        public void Get()
        {
            Complete();
        }

        /// <summary>
        /// Cancels the task that this future refers to.
        /// </summary>
        public void Cancel()
        {
            MyPromise.Cancel();
        }

        public static bool operator ==(Future Lhs, Future Rhs)
        {
            return Lhs.MyPromise == Rhs.MyPromise;
        }
        public static bool operator !=(Future Lhs, Future Rhs)
        {
            return Lhs.MyPromise != Rhs.MyPromise;
        }

        /// <summary>
        /// Creates the future with the given promise.
        /// </summary>
        /// <param name="Promise"> The promise that the future will refer to. </param>
        public Future(Promise Promise)
        {
            MyPromise = Promise;
        }
        /// <summary>
        /// Creates a future with the promise of the other future.
        /// </summary>
        /// <param name="Other"> The future to copy. </param>
        public Future(Future Other)
        {
            MyPromise = Other.MyPromise;
        }

        /// <summary>
        /// The promise that the future refers to.
        /// </summary>
        public Promise Promise
        {
            get
            {
                return MyPromise;
            }
        }

        public bool Equals(Future Other)
        {
            return this == Other;
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    /// <summary>
    /// Allows access to the promise as well as interfacing with the task system.
    /// If the future is not valid a NullReferenceException will be thrown when the state is changed.
    /// </summary>
    public struct Future<TRet>
    {
        /// <summary>
        /// The promise this future refers to.
        /// </summary>
        private Promise<TRet> MyPromise;

        /// <summary>
        /// A boolean value indicating whether the Future is valid.
        /// </summary>
        public bool Valid
        {
            get
            {
                return MyPromise != null;
            }
        }
        /// <summary>
        /// A boolean value idicating whether the Task has been completed.
        /// </summary>
        public bool Completed
        {
            get
            {
                return MyPromise.Complete.Value;
            }
        }
        /// <summary>
        /// A boolean value indicating whether the Task has been canceled.
        /// </summary>
        public bool Canceled
        {
            get
            {
                return MyPromise.Canceled.Value;
            }
        }

        /// <summary>
        /// Completes the Task before returning.
        /// If the task has been canceled a TaskCanceledException is thrown.
        /// </summary>
        public void Complete()
        {
            if (MyPromise.Canceled.Value)
            {
                throw new TaskCanceledException("Task was canceled.");
            }
            else
            {
                while (!MyPromise.Complete.Value)
                {
                    ThreadPoolManager.ExecuteSingleTask();
                }

                if (MyPromise.CaughtException != null)
                {
                    throw MyPromise.CaughtException;
                }
            }
        }
        /// <summary>
        /// Completes the task then returns its return value.
        /// Throws a TaskCanceledException if the task has been canceled.
        /// </summary>
        /// <returns> The return value of the Task. </returns>
        public TRet Get()
        {
            if (MyPromise.Canceled.Value)
            {
                throw new TaskCanceledException("Task was canceled.");
            }
            else
            {
                while (!MyPromise.Complete.Value)
                {
                    ThreadPoolManager.ExecuteSingleTask();
                }

                if (MyPromise.CaughtException != null)
                {
                    throw MyPromise.CaughtException;
                }

                return MyPromise.ReturnValue;
            }
        }

        /// <summary>
        /// Cancels the task that this future refers to.
        /// </summary>
        public void Cancel()
        {
            MyPromise.Cancel();
        }

        public static bool operator ==(Future<TRet> Lhs, Future<TRet> Rhs)
        {
            return Lhs.MyPromise == Rhs.MyPromise;
        }
        public static bool operator !=(Future<TRet> Lhs, Future<TRet> Rhs)
        {
            return Lhs.MyPromise != Rhs.MyPromise;
        }

        /// <summary>
        /// Creates the future with the given promise.
        /// </summary>
        /// <param name="Promise"> The promise that the future will refer to. </param>
        public Future(Promise<TRet> Promise)
        {
            MyPromise = Promise;
        }
        /// <summary>
        /// Creates a future with the promise of the other future.
        /// </summary>
        /// <param name="Other"> The future to copy. </param>
        public Future(Future<TRet> Other)
        {
            MyPromise = Other.MyPromise;
        }

        /// <summary>
        /// The promise that the future refers to.
        /// </summary>
        public Promise<TRet> Promise
        {
            get
            {
                return MyPromise;
            }
        }

        public bool Equals(Future<TRet> Other)
        {
            return this == Other;
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
