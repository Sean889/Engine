//Generated File

using OpenTK.Graphics.OpenGL;
using System;
using System.Diagnostics;
using ShaderRuntime;

#pragma warning disable 168

namespace Shaders
{
    class Shader : GLShader
    {
        public bool TransposeMatrix = false;
        public static bool ImplementationSupportsShaders
        {
            get
            {
                return (new Version(GL.GetString(StringName.Version).Substring(0, 3)) >= new Version(2, 0) ? true : false);
            }
        }
        public static int __Vertex;
        public static readonly string VertexShaderSource = "#version 330\n\n#pragma stage Vertex\n#pragma name Shader\n\nin vec2 Vertex;\n\nvoid main()\n{\n    gl_Position = vec4(0.0, 0.0, 0.0, 0.0);\n}";
        public static readonly string FragmentShaderSource = "#version 330\n\n#pragma stage Fragment\n#pragma name Shader\n\nout vec3 colour;\n\nvoid main()\n{\n    colour = vec3(1.0, 1.0, 1.0);\n}";
        public static int ProgramID = 0;
        private static ShaderRuntime.Utility.Counter Ctr = new ShaderRuntime.Utility.Counter(new Action(delegate{ GL.DeleteProgram(ProgramID); }));
        public static void CompileShader()
        {
            int prg = GL.CreateProgram();

            int Vertex = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(Vertex, VertexShaderSource);
            GL.CompileShader(Vertex);
            Debug.WriteLine(GL.GetShaderInfoLog(Vertex));
            GL.AttachShader(prg, Vertex);

            int Fragment = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(Fragment, FragmentShaderSource);
            GL.CompileShader(Fragment);
            Debug.WriteLine(GL.GetShaderInfoLog(Fragment));
            GL.AttachShader(prg, Fragment);

            GL.LinkProgram(prg);
            Debug.WriteLine(GL.GetProgramInfoLog(prg));

            GL.DetachShader(prg, Vertex);
            GL.DeleteShader(Vertex);

            GL.DetachShader(prg, Fragment);
            GL.DeleteShader(Fragment);
            __Vertex = GL.GetAttribLocation(prg, "Vertex");
            ProgramID = prg;
        }
        public void Compile()
        {
            if(ProgramID == 0)
                CompileShader();
            Ctr++;
        }
        public void SetParameter<T>(string name, T value)
        {
            try
            {
                switch(name)
                {
                    default:
                        throw new InvalidIdentifierException("There is no uniform variable named " + name + " in this shader.");
                }
            }
            catch(InvalidCastException e)
            {
                throw new InvalidParameterTypeException("Invalid parameter type: " + name + " is not convertible to the type \"" + typeof(T).FullName + "\".");
            }
        }
        public T GetParameter<T>(string name)
        {
            try
            {
                switch(name)
                {
                    default:
                        throw new InvalidIdentifierException("There is no uniform variable named " + name + " in this shader.");
                }
            }
            catch(InvalidCastException e)
            {
                throw new InvalidParameterTypeException("Invalid paramater type: " + name + " is not convertible to the type \"" + typeof(T).FullName + "\".");
            }
        }
        public int GetParameterLocation(string name)
        {
            switch(name)
            {
                case "Vertex":
                    return __Vertex;
                default:
                    throw new InvalidIdentifierException("There is no parameter named " + name + ".");
            }
        }
        public void PassUniforms()
        {
        }
        public void UseShader()
        {
            GL.UseProgram(ProgramID);
            GL.EnableVertexAttribArray(__Vertex);
        }
        public int GetShaderID()
        {
            if(ProgramID != 0)
                return ProgramID;
            throw new ShaderNotInitializedException("The shader \"Shader\" has not been initialized. Call Compile() on one of the instances or CompileShader() to compile the shader");
        }
        public void Dispose()
        {
            Ctr--;
        }
        public bool IsSupported
        {
            get
            {
                return ImplementationSupportsShaders;
            }
        }
    }
}
