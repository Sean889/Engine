using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace Shader
{
    public interface IGLShader : IDisposable
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

        /// <summary>
        /// Adds a glVertexAttribPointer command to be executed.
        /// </summary>
        /// <param name="name"> The name of the parameter. </param>
        /// <param name="size"></param>
        /// <param name="type"></param>
        /// <param name="normalized"></param>
        /// <param name="stride"></param>
        /// <param name="offset"></param>
        void SetVertexAttribPointer(string name, int size, VertexAttribPointerType type, bool normalized, int stride, int offset);
        /// <summary>
        /// Passes all the parameters that have been set for the shader.
        /// </summary>
        void PassParameters();
        /// <summary>
        /// Makes the shader current in the OpenGL state.
        /// </summary>
        void UseShader();

        /// <summary>
        /// Gets the OpenGL id for this shader.
        /// </summary>
        /// <returns></returns>
        int GetShaderID();

        /// <summary>
        /// Allows for deletion of the shader.
        /// </summary>
        void Dispose();
    }
}
