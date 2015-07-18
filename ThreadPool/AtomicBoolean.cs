using System.Threading;

namespace ThreadPool
{
#pragma warning disable 420
    internal struct AtomicBoolean
    {
        private volatile int value;

        private const int TRUE = 1;
        private const int FALSE = 0;

        public bool Value
        {
            get
            {
                return value != FALSE;
            }
            set
            {
                this.value = value ? TRUE : FALSE;
            }
        }

        /// <summary>
        /// Attempt changing the value from CompareAnd to Target.
        /// </summary>
        /// <param name="Target"> The target value. </param>
        /// <param name="Expected"> The value that is being compared to. </param>
        /// <returns> Whether the change succeded. </returns>
        public bool SetWhen(bool Target, bool Expected)
        {
            int comparand = Expected ? TRUE : FALSE;
            int result = Interlocked.CompareExchange(ref value, Target ? TRUE : FALSE, comparand);
            return (result == TRUE) == Expected;
        }
        /// <summary>
        /// Attempt changing the value from False to True.
        /// </summary>
        /// <returns> Whether the change occured. </returns>
        public bool FalseToTrue()
        {
            return SetWhen(true, false);
        }
        /// <summary>
        /// Attempt changing the value from True to False.
        /// </summary>
        /// <returns> Whether the change occured. </returns>
        public bool TrueToFalse()
        {
            return SetWhen(false, true);
        }
        /// <summary>
        /// Attempts to switch the value.
        /// </summary>
        /// <returns> Whether the change occured. </returns>
        public bool Switch()
        {
            bool val = value != FALSE;
            return SetWhen(!val, val);
        }

        /// <summary>
        /// Initalizes with the given value.
        /// </summary>
        /// <param name="Value"></param>
        public AtomicBoolean(bool Value = false)
        {
            value = Value ? 1 : 0;
        }

        /// <summary>
        /// Creates a new AtomicBoolean with the given value.
        /// </summary>
        /// <param name="value"> The value to initialize the AtomicBoolean with. </param>
        /// <returns></returns>
        public static implicit operator AtomicBoolean(bool value)
        {
            return new AtomicBoolean(value);
        }
    }
}

