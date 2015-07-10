//Generated File

using OpenTK.Graphics.OpenGL;
using System;
using System.Diagnostics;
using Shader;

#pragma warning disable 168

namespace Shaders
{
    class PlanetSurfaceShader : GLShader
    {
        public bool TransposeMatrix = false;
        public static int __MaxDeform;
        public static int __MVP;
        public static int __LightRotation;
        public static int __BumpMap;
        public static int __RelLightDir;
        public static int __ColourTexture;
        public static int __NormalTexture;
        public float uniform_MaxDeform;
        public OpenTK.Matrix4 uniform_MVP;
        public OpenTK.Matrix4 uniform_LightRotation;
        public Texture uniform_BumpMap;
        public OpenTK.Vector3 uniform_RelLightDir;
        public Texture uniform_ColourTexture;
        public Texture uniform_NormalTexture;
        public static readonly string VertexShaderSource = "#version 330\n\n#pragma name PlanetSurfaceShader\n#pragma stage Vertex\n\nin vec3 Vertex;\nin vec3 Texcoord;\n\nuniform float MaxDeform;\nuniform mat4 MVP;\nuniform mat4 LightRotation;\nuniform samplerCube BumpMap;\n\nsmooth out vec3 Texcoord0;\n\nvoid main()\n{\n    Texcoord0 = Texcoord;\n    gl_Position = MVP * vec4(Vertex + (Texcoord * texture(BumoMap, Texcoord).x * (MaxDeform - MaxDeform * 0.5)), 1.0);\n}";
        public static readonly string FragmentShaderSource = "#version 330\n\n#pragma name PlanetSurfaceShader\n#pragma stage Fragment\n\nin vec3 Texcoord0;\n\nuniform vec3 RelLightDir;\n\nuniform samplerCube ColourTexture;\nuniform samplerCube NormalTexture;\n\nout vec3 Colour;\n\nvoid main()\n{\n    vec3 normal = texture(NormalTexture, Texcoord0).rgb;\n    \n    vec3 tangent;\n    if(normal.x > normal.y && normal.x > normal.z)\n        tangent = sign(normal.x) * normalize(cross(normal, cross(vec3(0.0, 0.0, 1.0), normal)));\n    else if(normal.y > normal.z && normal.y > normal.x)\n        tangent = normalize(cross(normal, cross(vec3(1.0, 0.0, 0.0), normal)));\n    else\n        tangent = sign(normal.z) * normalize(cross(normal, cross(vec3(-1.0, 0.0, 0.0), normal)));\n        \n    vec3 bitangent = cross(tangent, normal);\n    \n    mat3 TBN = mat3(tangent, bitangent, Texcoord0);\n    \n    Colour = texture(colour_texture, texcoord0).rgb * max(dot(light_dir, TBN * normal), 0.1);\n}";
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
            __MaxDeform = GL.GetUniformLocation(prg, "MaxDeform");
            __MVP = GL.GetUniformLocation(prg, "MVP");
            __LightRotation = GL.GetUniformLocation(prg, "LightRotation");
            __BumpMap = GL.GetUniformLocation(prg, "BumpMap");
            __RelLightDir = GL.GetUniformLocation(prg, "RelLightDir");
            __ColourTexture = GL.GetUniformLocation(prg, "ColourTexture");
            __NormalTexture = GL.GetUniformLocation(prg, "NormalTexture");
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
                    case "MaxDeform":
                        uniform_MaxDeform = (float)(object)value;
                        break;
                    case "MVP":
                        uniform_MVP = (OpenTK.Matrix4)(object)value;
                        break;
                    case "LightRotation":
                        uniform_LightRotation = (OpenTK.Matrix4)(object)value;
                        break;
                    case "BumpMap":
                        uniform_BumpMap = (Texture)(object)value;
                        break;
                    case "RelLightDir":
                        uniform_RelLightDir = (OpenTK.Vector3)(object)value;
                        break;
                    case "ColourTexture":
                        uniform_ColourTexture = (Texture)(object)value;
                        break;
                    case "NormalTexture":
                        uniform_NormalTexture = (Texture)(object)value;
                        break;
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
                    case "MaxDeform":
                        return (T)(object)uniform_MaxDeform;
                    case "MVP":
                        return (T)(object)uniform_MVP;
                    case "LightRotation":
                        return (T)(object)uniform_LightRotation;
                    case "BumpMap":
                        return (T)(object)uniform_BumpMap;
                    case "RelLightDir":
                        return (T)(object)uniform_RelLightDir;
                    case "ColourTexture":
                        return (T)(object)uniform_ColourTexture;
                    case "NormalTexture":
                        return (T)(object)uniform_NormalTexture;
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
                case "MaxDeform":
                    return __MaxDeform;
                case "MVP":
                    return __MVP;
                case "LightRotation":
                    return __LightRotation;
                case "BumpMap":
                    return __BumpMap;
                case "RelLightDir":
                    return __RelLightDir;
                case "ColourTexture":
                    return __ColourTexture;
                case "NormalTexture":
                    return __NormalTexture;
                default:
                    throw new InvalidIdentifierException("There is no parameter named " + name + ".");
            }
        }
        public void PassUniforms()
        {
            GL.Uniform1(__MaxDeform, uniform_MaxDeform);
            GL.UniformMatrix4(__MVP, TransposeMatrix, ref uniform_MVP);
            GL.UniformMatrix4(__LightRotation, TransposeMatrix, ref uniform_LightRotation);
            GL.Uniform1(__BumpMap, 0);
            GL.Uniform3(__RelLightDir, uniform_RelLightDir);
            GL.Uniform1(__ColourTexture, 1);
            GL.Uniform1(__NormalTexture, 2);
        }
        public void UseShader()
        {
            GL.UseProgram(ProgramID);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(uniform_BumpMap.Target, uniform_BumpMap.TextureID);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(uniform_ColourTexture.Target, uniform_ColourTexture.TextureID);
            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(uniform_NormalTexture.Target, uniform_NormalTexture.TextureID);
        }
        public int GetShaderID()
        {
            if(ProgramID != 0)
                return ProgramID;
            throw new ShaderNotInitializedException("The shader \"PlanetSurfaceShader\" has not been initialized. Call Compile() on one of the instances or CompileShader() to compile the shader");
        }
        public void Dispose()
        {
            Ctr--;
        }
    }
}
