using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineSystem
{
#pragma warning disable 1591
    [Serializable]
    public class SystemNotInitializedException : Exception
    {
        public SystemNotInitializedException() { }
        public SystemNotInitializedException(string message) : base(message) { }
        public SystemNotInitializedException(string message, Exception inner) : base(message, inner) { }
        protected SystemNotInitializedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
