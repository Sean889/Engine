using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shader
{
    [Serializable]
    public class InvalidParameterTypeException : Exception
    {
        public InvalidParameterTypeException() { }
        public InvalidParameterTypeException(string message) : base(message) { }
        public InvalidParameterTypeException(string message, Exception inner) : base(message, inner) { }
        protected InvalidParameterTypeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class InvalidIdentifierException : Exception
    {
        public InvalidIdentifierException() { }
        public InvalidIdentifierException(string message) : base(message) { }
        public InvalidIdentifierException(string message, Exception inner) : base(message, inner) { }
        protected InvalidIdentifierException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    [Serializable]
    public class ShaderNotInitializedException : Exception
    {
        public ShaderNotInitializedException() { }
        public ShaderNotInitializedException(string message) : base(message) { }
        public ShaderNotInitializedException(string message, Exception inner) : base(message, inner) { }
        protected ShaderNotInitializedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
