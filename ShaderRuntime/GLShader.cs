using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace ShaderRuntime
{
    public interface GLShader : IDisposable
    {
        /// <summary>
        /// Sets the given parameter for the shader.
        /// The value will be preserved in the shader.
        /// An InvalidIdentifierException will be thrown if name is not an identifier in the shader.
        /// An InvalidParameterTypeException will be thrown if T is not the correct type.
        /// </summary>
        /// <typeparam name="T"> The expected type of the parameter. </typeparam>
        /// <param name="name"> The identifier of the parameter. </param>
        /// <param name="value"> The value of the parameter. </param>
        void SetParameter<T>(string name, T value);
        /// <summary>
        /// Gets the value of the parameter from the shader.
        /// An InvalidParameterTypeExcpetion will be thrown if name is not the correct type.
        /// </summary>
        /// <typeparam name="T"> The type of the parameter. </typeparam>
        /// <param name="name"> The identifier of the parameter. </param>
        /// <returns> The value if it was found. </returns>
        T GetParameter<T>(string name);

        int GetParameterLocation(string name);

        /// <summary>
        /// Compiles the shader
        /// </summary>
        void Compile();

        void PassUniforms();
        /// <summary>
        /// Makes the shader current in the OpenGL state as well as any other attached state it needs that doesn't have to be reset with each draw call.
        /// </summary>
        void UseShader();

        /// <summary>
        /// Gets the OpenGL id for this shader.
        /// </summary>
        /// <returns></returns>
        int GetShaderID();
        
        /// <summary>
        /// Whether the shader is supported on this platform. The OpenG version has to higher than 2.0 for shaders to be available.
        /// </summary>
        bool IsSupported
        {
            get;
        }
    }
}
