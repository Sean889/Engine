using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace ShaderRuntime
{
    /// <summary>
    /// <para>An interface that allows for all properties of an OpenGL shader to be stored with the shader.</para>
    /// 
    /// <para>This interface is intended to be used with ShaderGenerator.exe. However you can define your own types deriving from this interface for more flexibility as long as they fulfill the contract.</para>
    /// </summary>
    /// 
    public interface GLShader : IDisposable
    {
        /// <summary>
        /// Sets the given parameter for the shader.
        /// The value will be preserved in the shader.
        /// </summary>
        /// <typeparam name="T"> The expected type of the parameter. </typeparam>
        /// <param name="ParameterName"> The name of the parameter as in the shader. </param>
        /// <param name="value"> The new value of the parameter. </param>
        /// <exception cref="InvalidIdentifierException">Should be thrown if <paramref name="ParameterName"/> is not an identifier in the shader.</exception>
        /// <exception cref="InvalidParameterTypeException">Should be thrown if <typeparamref name="T"/> is not the correct type.</exception>
        void SetParameter<T>(string ParameterName, T value);
        /// <summary>
        /// Gets the value of the parameter from the shader.
        /// </summary>
        /// <typeparam name="T"> The type of the parameter. </typeparam>
        /// <param name="ParameterName"> The name of the parameter as in the shader. </param>
        /// <returns> The value if it was found. </returns>
        /// <exception cref="InvalidParameterTypeException">Should be thrown if <typeparamref name="T"/> is not the type of <paramref name="ParameterName"/>.</exception>
        T GetParameter<T>(string ParameterName);

        int GetParameterLocation(string name);

        /// <summary>
        /// Compiles the shader. This must be called before <see cref="PassUniforms"/>, <see cref="UseShader"/> or <see cref="GetShaderID"/> can be called.
        /// </summary>
        void Compile();

        /// <summary>
        /// Passes any data that needs to be passed per draw call.
        /// </summary>
        void PassUniforms();
        /// <summary>
        /// Makes the shader current in the OpenGL state as well as any other attached state it needs that doesn't have to be reset with each draw call.
        /// </summary>
        void UseShader();

        /// <summary>
        /// Gets the OpenGL id for this shader.
        /// </summary>
        /// <returns>The OpenGL id for the shader program.</returns>
        /// <exception cref="ShaderNotInitializedException">Should be thrown if the shader has not been compiled.</exception>
        int GetShaderID();
        
        /// <summary>
        /// Whether the shader is supported on this platform. The OpenGL version has to be greater than 2.0 for shaders to be available. Shader versions may also require a higher version.
        /// </summary>
        bool IsSupported
        {
            get;
        }
    }
}
