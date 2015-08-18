//Generated File

using OpenTK.Graphics.OpenGL;
using System;
using System.Diagnostics;
using ShaderRuntime;

#pragma warning disable 168

namespace Shaders
{
    class PlanetSurfaceShader : GLShader
    {
        public bool TransposeMatrix = false;
        public static bool ImplementationSupportsShaders
        {
            get
            {
                return (new Version(GL.GetString(StringName.Version).Substring(0, 3)) >= new Version(2, 0) ? true : false);
            }
        }
        public static int __MVP;
        public static int __LightRot;
        public static int __max_height;
        public static int __heightmap;
        public static int __lightdir;
        public static int __Vertex;
        public static int __Texcoord;
        public static int __colour_texture;
        public static int __normal_texture;
        public static int __light_dir;
        public OpenTK.Matrix4 uniform_MVP;
        public OpenTK.Matrix4 uniform_LightRot;
        public float uniform_max_height;
        public Texture uniform_heightmap;
        public OpenTK.Vector3 uniform_lightdir;
        public Texture uniform_colour_texture;
        public Texture uniform_normal_texture;
        public OpenTK.Vector3 uniform_light_dir;
        public static readonly string VertexShaderSource = "#version 430\n\n#pragma stage Vertex\n#pragma name PlanetSurfaceShader\n\n\nlayout(location = 0) in vec3 Vertex;\nlayout(location = 1) in vec3 Texcoord;\n\nlayout(location = 0) uniform mat4 MVP;\nlayout(location = 4) uniform mat4 LightRot;\nlayout(location = 9) uniform float max_height;\nlayout(location = 11) uniform samplerCube heightmap;\nlayout(location = 20) uniform vec3 lightdir;\n\n#define vert Vertex\n#define texcoord Texcoord\n\n#define mvp MVP\n#define rot LightRot\n\nout vec3 vs_lightdir;\nsmooth out vec3 texcoord0;\n\nvoid main()\n{\n    vs_lightdir = normalize((rot * vec4(lightdir, 1.0)).xyz);\n    gl_Position = mvp * vec4(vert + (texcoord * texture(heightmap, texcoord).x * (max_height - max_height * 0.5)), 1.0);\n    texcoord0 = texcoord;\n}";
        public static readonly string FragmentShaderSource = "#version 430\n\n#pragma stage Fragment\n\nhighp float rand(vec2 co)\n{\n    highp float a = 12.9898;\n    highp float b = 78.233;\n    highp float c = 43758.5453;\n    highp float dt= dot(co.xy ,vec2(a,b));\n    highp float sn= mod(dt,3.14);\n    return fract(sin(sn) * c);\n}\n\nhighp float rand(vec3 co)\n{\n    return vec3(rand(co.xy), rand(co.yz), rand(co.zx));\n}\n\nin vec3 texcoord0;\n\nlayout (location = 10) uniform samplerCube colour_texture;\nlayout (location = 12) uniform samplerCube normal_texture;\nlayout (location = 15) uniform vec3 light_dir;\n\nout vec3 colour;\n\nvoid main()\n{\n    vec3 normal = texture(normal_texture, texcoord0).rgb;\n    \n    vec3 tangent;\n    if(normal.x > normal.y && normal.x > normal.z)\n        tangent = sign(normal.x) * normalize(cross(normal, cross(vec3(0.0, 0.0, 1.0), normal)));\n    else if(normal.y > normal.z && normal.y > normal.x)\n        tangent = normalize(cross(normal, cross(vec3(1.0, 0.0, 0.0), normal)));\n    else\n        tangent = sign(normal.z) * normalize(cross(normal, cross(vec3(-1.0, 0.0, 0.0), normal)));\n        \n    vec3 bitangent = cross(tangent, normal);\n    \n    mat3 TBN = mat3(tangent, bitangent, texcoord0);\n    \n    colour = (texture(colour_texture, texcoord0).rgb + rand(texcoord0 * 10000) * 0.0625) * max(dot(light_dir, TBN * normal), 0.1);\n};";
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
            __MVP = GL.GetUniformLocation(prg, "MVP");
            __LightRot = GL.GetUniformLocation(prg, "LightRot");
            __max_height = GL.GetUniformLocation(prg, "max_height");
            __heightmap = GL.GetUniformLocation(prg, "heightmap");
            __lightdir = GL.GetUniformLocation(prg, "lightdir");
            __Vertex = GL.GetAttribLocation(prg, "Vertex");
            __Texcoord = GL.GetAttribLocation(prg, "Texcoord");
            __colour_texture = GL.GetUniformLocation(prg, "colour_texture");
            __normal_texture = GL.GetUniformLocation(prg, "normal_texture");
            __light_dir = GL.GetUniformLocation(prg, "light_dir");
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
                    case "MVP":
                        uniform_MVP = (OpenTK.Matrix4)(object)value;
                        break;
                    case "LightRot":
                        uniform_LightRot = (OpenTK.Matrix4)(object)value;
                        break;
                    case "max_height":
                        uniform_max_height = (float)(object)value;
                        break;
                    case "heightmap":
                        uniform_heightmap = (Texture)(object)value;
                        break;
                    case "lightdir":
                        uniform_lightdir = (OpenTK.Vector3)(object)value;
                        break;
                    case "colour_texture":
                        uniform_colour_texture = (Texture)(object)value;
                        break;
                    case "normal_texture":
                        uniform_normal_texture = (Texture)(object)value;
                        break;
                    case "light_dir":
                        uniform_light_dir = (OpenTK.Vector3)(object)value;
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
                    case "MVP":
                        return (T)(object)uniform_MVP;
                    case "LightRot":
                        return (T)(object)uniform_LightRot;
                    case "max_height":
                        return (T)(object)uniform_max_height;
                    case "heightmap":
                        return (T)(object)uniform_heightmap;
                    case "lightdir":
                        return (T)(object)uniform_lightdir;
                    case "colour_texture":
                        return (T)(object)uniform_colour_texture;
                    case "normal_texture":
                        return (T)(object)uniform_normal_texture;
                    case "light_dir":
                        return (T)(object)uniform_light_dir;
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
                case "MVP":
                    return __MVP;
                case "LightRot":
                    return __LightRot;
                case "max_height":
                    return __max_height;
                case "heightmap":
                    return __heightmap;
                case "lightdir":
                    return __lightdir;
                case "Vertex":
                    return __Vertex;
                case "Texcoord":
                    return __Texcoord;
                case "colour_texture":
                    return __colour_texture;
                case "normal_texture":
                    return __normal_texture;
                case "light_dir":
                    return __light_dir;
                default:
                    throw new InvalidIdentifierException("There is no parameter named " + name + ".");
            }
        }
        public void PassUniforms()
        {
            GL.UniformMatrix4(__MVP, TransposeMatrix, ref uniform_MVP);
            GL.UniformMatrix4(__LightRot, TransposeMatrix, ref uniform_LightRot);
            GL.Uniform1(__max_height, uniform_max_height);
            GL.Uniform1(__heightmap, 0);
            GL.Uniform3(__lightdir, uniform_lightdir);
            GL.Uniform1(__colour_texture, 1);
            GL.Uniform1(__normal_texture, 2);
            GL.Uniform3(__light_dir, uniform_light_dir);
        }
        public void UseShader()
        {
            GL.UseProgram(ProgramID);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(uniform_heightmap.Target, uniform_heightmap.TextureID);
            GL.EnableVertexAttribArray(__Vertex);
            GL.EnableVertexAttribArray(__Texcoord);
            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(uniform_colour_texture.Target, uniform_colour_texture.TextureID);
            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(uniform_normal_texture.Target, uniform_normal_texture.TextureID);
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
        public bool IsSupported
        {
            get
            {
                return ImplementationSupportsShaders;
            }
        }
    }
}
