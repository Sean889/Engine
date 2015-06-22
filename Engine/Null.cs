using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineSystem
{
    /// <summary>
    /// This class should never be instantiated and cannot be inherited.
    /// Use it to indicate a value which should be nothing.
    /// </summary>
    public sealed class Null
    {
        private Null()
        {
            throw new NotSupportedException("Null should never be instantiated.");
        }

        public const Null Value = null;
    }
}
