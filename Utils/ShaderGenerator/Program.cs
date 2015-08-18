using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;


namespace ShaderGenerator
{
    class Program
    {
#pragma warning disable 168
        struct Property
        {
            public string Type;
            public string Name;

            public Property(string Type, string Name)
            {
                this.Type = Type;
                this.Name = Name;
            }

            public static bool operator ==(Property rhs, string o)
            {
                return rhs.Name == o;
            }
            public static bool operator !=(Property rhs, string o)
            {
                return rhs.Name != o;
            }

            public static bool operator ==(Property rhs, Property o)
            {
                return rhs.Name == o.Name;
            }
            public static bool operator !=(Property rhs, Property o)
            {
                return rhs.Name != o.Name;
            }

            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        struct Stage
        {
            public string Source;
            public string ShaderStage;

            public Stage(string S, string Stage)
            {
                Source = S;
                ShaderStage = Stage;
            }
        }

        enum ShaderStage
        {
            Vertex,
            Fragment,
            Geometry,
            TessControl,
            TessEvaluation,
            Compute
        }

        [Serializable]
        public class InvalidShaderTypeException : Exception
        {
            public InvalidShaderTypeException() { }
            public InvalidShaderTypeException(string message) : base(message) { }
            public InvalidShaderTypeException(string message, Exception inner) : base(message, inner) { }
            protected InvalidShaderTypeException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context)
                : base(info, context) { }
        }
        [Serializable]
        public class StageIdentifierMissingException : Exception
        {
            public StageIdentifierMissingException() { }
            public StageIdentifierMissingException(string message) : base(message) { }
            public StageIdentifierMissingException(string message, Exception inner) : base(message, inner) { }
            protected StageIdentifierMissingException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context)
                : base(info, context) { }
        }
        [Serializable]
        public class InvalidTypeException : Exception
        {
            public InvalidTypeException() { }
            public InvalidTypeException(string message) : base(message) { }
            public InvalidTypeException(string message, Exception inner) : base(message, inner) { }
            protected InvalidTypeException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context)
                : base(info, context) { }
        }
        [Serializable]
        public class ParseFailedException : Exception
        {
            public ParseFailedException() { }
            public ParseFailedException(string message) : base(message) { }
            public ParseFailedException(string message, Exception inner) : base(message, inner) { }
            protected ParseFailedException(
              System.Runtime.Serialization.SerializationInfo info,
              System.Runtime.Serialization.StreamingContext context)
                : base(info, context) { }
        }

        private static string NamePragma = "#pragma name";
        private static string StagePragma = "#pragma stage";

        static int Counter = 0;

        #region Arrays
        static string[,] Types =
        {
            {"vec2", "OpenTK.Vector2"},
            {"vec3", "OpenTK.Vector3"},
            {"vec4", "OpenTK.Vetor4"},
            {"dvec2", "OpenTK.Vector2d"},
            {"dvec3", "OpenTK.Vector3d"},
            {"dvec4", "OpenTK.Vector4d"},
            {"ivec2", "OpenTK.Vector2i"},
            {"ivec3", "OpenTK.Vector3i"},
            {"ivec4", "OpenTK.Vector4i"},
            {"uvec2", "OpenTK.Vector2u"},
            {"uvec3", "OpenTK.Vector3u"},
            {"uvec4", "OpenTK.Vector4u"},
            {"bvec2", "OpenTK.Vector2b"},
            {"bvec3", "OpenTK.Vector3b"},
            {"bvec4", "OpenTK.Vector4b"},
            {"bool", "bool"},
            {"float", "float"},
            {"double", "double"},
            {"int", "int"},
            {"uint", "uint"},
            {"mat2", "OpenTK.Matrix2"},
            {"mat3", "OpenTK.Matrix3"},
            {"mat4", "OpenTK.Matrix4"},
            {"mat2d", "OpenTK.Matrix2d"},
            {"mat3d", "OpenTK.Matrix3d"},
            {"mat4d", "OpenTK.Matrix4d"},
            {"mat2x3", "OpenTK.Matrix2x3"},
            {"mat2x4", "OpenTK.Matrix2x4"},
            {"mat3X2", "OpenTK.Matrix3x2"},
            {"mat3x4", "OpenTK.Matrix3x4"},
            {"mat4x2", "OpenTK.Matrix4x2"},
            {"mat4x3", "OpenTK.Matrix4x3"},
            {"dmat2x3", "OpenTK.Matrix2x3d"},
            {"dmat2x4", "OpenTK.Matrix2x4d"},
            {"dmat3x2", "OpenTK.Matrix3x2d"},
            {"dmat3x4", "OpenTK.Matrix3x4d"},
            {"dmat4x2", "OpenTK.Matrix4x2d"},
            {"dmat4x3", "OpenTK.Matrix4x3d"},
            {"atomic_uint", "uint"}
        };

        static string[] Samplers =
        { 
            "sampler1D",
            "sampler2D",
            "sampler3D", 
            "samplerCube",
            "sampler2DRect",
            "sampler1DArray",
            "sampler2DArray",
            "samplerCubeArray", 
            "samplerBuffer",
            "sampler2DMS",
            "sampler2DMSArray",
            "usampler",
            "usampler1D",
            "usampler2D",
            "usampler3D", 
            "usamplerCube",
            "usampler2DRect",
            "usampler1DArray",
            "usampler2DArray",
            "usamplerCubeArray", 
            "usamplerBuffer",
            "usampler2DMS",
            "usampler2DMSArray",
            "isampler",
            "isampler1D",
            "isampler2D",
            "isampler3D", 
            "isamplerCube",
            "isampler2DRect",
            "isampler1DArray",
            "isampler2DArray",
            "isamplerCubeArray", 
            "isamplerBuffer",
            "isampler2DMS",
            "isampler2DMSArray",
            "sampler1DShadow",
            "sampler2DShadow", 
            "samplerCubeShadow",
            "sampler2DRectShadow",
            "sampler1DArrayShadow",
            "sampler2DArrayShadow",
            "samplerCubeArrayShadow"
        };
        #endregion

        static string GetShaderType(string stage)
        {
            switch(stage)
            {
                case "Vertex":
                    return "OpenTK.Graphics.OpenGL.ShaderType.VertexShader";
                case "vertex":
                    return "OpenTK.Graphics.OpenGL.ShaderType.VertexShader";
                case "Fragment":
                    return "OpenTK.Graphics.OpenGL.ShaderType.FragmentShader";
                case "fragment":
                    return "OpenTK.Graphics.OpenGL.ShaderType.FragmentShader";
                case "Geometry":
                    return "OpenTK.Graphics.OpenGL.ShaderType.GeometryShader";
                case "geometry":
                    return "OpenTK.Graphics.OpenGL.ShaderType.GeometryShader";
                case "TessControl":
                    return "OpenTK.Graphics.OpenGL.ShaderType.TessControlShader";
                case "tessControl":
                    return "OpenTK.Graphics.OpenGL.ShaderType.TessControlShader";
                case "tess_control":
                    return "OpenTK.Graphics.OpenGL.ShaderType.TessControlShader";
                case "TessEvaluation":
                    return "OpenTK.Graphics.OpenGL.ShaderType.TessEvaluationShader";
                case "tessEvaluation":
                    return "OpenTK.Graphics.OpenGL.ShaderType.TessEvaluationShader";
                case "tess_evaluation":
                    return "OpenTK.Graphics.OpenGL.ShaderType.TessEvaluationShader";
                case "Compute":
                    return "OpenTK.Graphics.OpenGL.ShaderType.ComputeShader";
                case "compute":
                    return "OpenTK.Graphics.OpenGL.ShaderType.ComputeShader";
                default:
                    throw new InvalidShaderTypeException();
            }
        }
        static string GetUniformCommand(string type, string name)
        {
            string pName = "uniform_" + name;
            if(type == "OpenTK.Vector2" || type == "OpenTK.Vector2d" || type == "OpenTK.Vector2i" || type == "OpenTK.Vector2u" || type == "OpenTK.Vector2b")
            {
                return "GL.Uniform2(__" + name + ", " + pName + ");";
            }
            if(type == "OpenTK.Vector3" || type == "OpenTK.Vector3d" || type == "OpenTK.Vector3i" || type == "OpenTK.Vector3u" || type == "OpenTK.Vector3b")
            {
                return "GL.Uniform3(__" + name + ", " + pName + ");";
            }
            if(type == "OpenTK.Vector4" || type == "OpenTK.Vector4d" || type == "OpenTK.Vector4i" || type == "OpenTK.Vector4u" || type == "OpenTK.Vector4b")
            {
                return "GL.Uniform4(__" + name + ", " + pName + ");";
            }
            if(type == "OpenTK.Matrix2" || type == "OpenTK.Matrix2d")
            {
                return "GL.UniformMatrix2(__" + name + ", TransposeMatrix, ref " + pName + ");";
            }
            if(type == "OpenTK.Matrix3" || type == "OpenTK.Matrix3d")
            {
                return "GL.UniformMatrix3(__" + name + ", TransposeMatrix, ref " + pName + ");";
            }
            if(type == "OpenTK.Matrix4" || type == "OpenTK.Matrix4d")
            {
                return "GL.UniformMatrix4(__" + name + ", TransposeMatrix, ref " + pName + ");";
            }
            if(type.StartsWith("OpenTK.Matrix2x3"))
            {
                return "GL.UniformMatrix2x3(__" + name + ", TransposeMatrix, ref " + pName + ");";
            }
            if(type.StartsWith("OpenTK.Matrix2x4"))
            {
                return "GL.UniformMatrix2x4(__" + name + ", TransposeMatrix, ref " + pName + ");";
            }
            if(type.StartsWith("OpenTK.Matrix3x2"))
            {
                return "GL.UniformMatrix3x2(__" + name + ", TransposeMatrix, ref " + pName + ");";
            }
            if(type.StartsWith("OpenTK.Matrix3x4"))
            {
                return "GL.UniformMatrix3x4(__" + name + ", TransposeMatrix, ref " + pName + ");";
            }
            if(type.StartsWith("OpenTK.Matrix4x2"))
            {
                return "GL.UniformMatrix4x2(__" + name + ", TransposeMatrix, ref " + pName + ");";
            }
            if(type.StartsWith("OpenTK.Matrix4x3"))
            {
                return "GL.UniformMatrix4x3(__" + name + ", TransposeMatrix, ref " + name + ");";
            }
            if(type == "int" || type == "float" || type == "double" || type == "uint")
            {
                return "GL.Uniform1(__" + name + ", " + pName + ");";
            }
            if(type == "bool")
            {
                return "GL.Uniform1(__" + name + ", " + pName + " ? 1 : 0);";
            }
            throw new InvalidTypeException(name + " is not a valid type.");
        }

        enum Type
        {
            Uniform,
            In
        }

        static void Main(string[] args)
        {
            if (args.Length < 1)
                return;

            List<string> Lines = new List<string>();
            string name = "__UnnamedShader" + Convert.ToString(args[0].GetHashCode());

            List<Property> Properties = new List<Property>();
            List<string> InitCommands = new List<string>();
            List<string> DrawCommands = new List<string>();
            List<Pair<string, Type>> Positions = new List<Pair<string, Type>>();
            List<Stage> Stages = new List<Stage>();

            #region Parse
            for (int i = 1; i < args.Length; i++)
            {
                try
                {
                    string stage = null;
                    string file = File.ReadAllText(args[i]);
                    Lexer Lexer = new Lexer(file);
                    Parser Parser = new Parser(Lexer);

                    Parser.Parse();

                    foreach (Pair<string, string> p in Parser.Uniform)
                    {
                        string pName = "" + p.first;

                        if (Samplers.Contains(p.second))
                        {
                            if (!Properties.Contains(new Property("", pName)))
                            {
                                //The property isn't already there.
                                Properties.Add(new Property("Texture", p.first));
                                InitCommands.Add("GL.ActiveTexture(TextureUnit.Texture" + Counter + ");");
                                InitCommands.Add("GL.BindTexture(uniform_" + pName + ".Target, uniform_" + pName + ".TextureID);");
                                DrawCommands.Add("GL.Uniform1(__" + p.first + ", " + Counter + ");");
                                Positions.Add(new Pair<string, Type>(p.first, Type.Uniform));
                                Counter++;
                            }
                        }
                        else
                        {
                            if (!Properties.Contains(new Property("", pName)))
                            {
                                int j = 0;
                                for (; j < Types.GetLength(0); j++)
                                {
                                    if (Types[j, 0] == p.second)
                                        break;
                                }

                                Properties.Add(new Property(Types[j, 1], p.first));
                                DrawCommands.Add(GetUniformCommand(Types[j, 1], p.first));
                                Positions.Add(new Pair<string, Type>(p.first, Type.Uniform));
                            }
                        }
                    }

                    foreach (string str in Parser.Preprocessor)
                    {
                        string formatted = Regex.Replace(str, "//s+", " ");
                        if (formatted.StartsWith(NamePragma))
                        {
                            string new_name = formatted.Substring(NamePragma.Length);
                            new_name = new_name.Trim();
                            name = new_name;
                        }
                        else if (formatted.StartsWith(StagePragma))
                        {
                            string new_stage = formatted.Substring(StagePragma.Length);
                            new_stage = new_stage.Trim();
                            stage = new_stage;
                        }
                    }

                    if(stage == null)
                    {
                        if (args[i].EndsWith(".vert", StringComparison.OrdinalIgnoreCase))
                            stage = "Vertex";
                        else if (args[i].EndsWith(".frag", StringComparison.OrdinalIgnoreCase))
                            stage = "Fragment";
                        else if (args[i].EndsWith(".geom", StringComparison.OrdinalIgnoreCase))
                            stage = "Geometry";
                    }

                    if (GetShaderType(stage) == "OpenTK.Graphics.OpenGL.ShaderType.VertexShader")
                    {
                        foreach (Pair<string, string> p in Parser.In)
                        {
                            if (!Positions.Contains(new Pair<string, Type>(p.first, Type.In)))
                            {
                                Positions.Add(new Pair<string, Type>(p.first, Type.In));
                            }
                            InitCommands.Add("GL.EnableVertexAttribArray(__" + p.first + ");");
                        }
                    }

                    Stages.Add(new Stage(file, GetShaderType(stage)));
                }
                catch (InvalidTokenException e)
                {
                    throw new Exception("Faliure");

                }
                catch (InvalidShaderTypeException e)
                {
                    throw new Exception("Faliure");

                }
                catch (FileNotFoundException e)
                {
                    Console.WriteLine("File " + args[i] + " was not found.");
                    throw new Exception("Faliure");
                }
                catch (StageIdentifierMissingException e)
                {
                    Console.WriteLine("File " + args[i] + " does not specify which stage it is.");
                    throw new Exception("Faliure");
                }
                catch (InvalidTypeException e)
                {
                    throw new Exception("Faliure");
                }
                catch (ParseFailedException e)
                {
                    Console.WriteLine("Parse failed while parsing " + args[i] + ".");
                    throw new Exception("Faliure");
                }
            }
            #endregion

            Lines.Add("//Generated File");
            Lines.Add("");
            Lines.Add("using OpenTK.Graphics.OpenGL;");
            Lines.Add("using System;");
            Lines.Add("using System.Diagnostics;");
            Lines.Add("using ShaderRuntime;");
            Lines.Add("");
            Lines.Add("#pragma warning disable 168");
            Lines.Add("");
            Lines.Add("namespace Shaders");
            Lines.Add("{");
            Lines.Add("\tclass " + name + " : GLShader");
            Lines.Add("\t{");
            Lines.Add("\t\tpublic bool TransposeMatrix = false;");

            #region ImplementationSupportsShaders
            Lines.Add("\t\tpublic static bool ImplementationSupportsShaders");
            Lines.Add("\t\t{");
            Lines.Add("\t\t\tget");
            Lines.Add("\t\t\t{");
            Lines.Add("\t\t\t\treturn (new Version(GL.GetString(StringName.Version).Substring(0, 3)) >= new Version(2, 0) ? true : false);");
            Lines.Add("\t\t\t}");
            Lines.Add("\t\t}");
            #endregion
            #region Properties
            foreach (Pair<string, Type> p in Positions)
            {
                Lines.Add("\t\tpublic static int __" + p.first + ";");
            }

            

            foreach (Property p in Properties)
            {
                Lines.Add("\t\tpublic " + p.Type + " uniform_" + p.Name + ";");
            }

            
            #endregion
            #region Compile
            {
                string Vertex = null;
                string Fragment = null;
                string Geometry = null;
                string TessControl = null;
                string TessEvaluation = null;
                string Compute = null;

                foreach (Stage s in Stages)
                {
                    string source = Regex.Replace(s.Source, "(\r\n|\r|\n)", "\\n");
                    switch (s.ShaderStage)
                    {
                        case "OpenTK.Graphics.OpenGL.ShaderType.VertexShader":
                            Vertex = source;
                            break;
                        case "OpenTK.Graphics.OpenGL.ShaderType.FragmentShader":
                            Fragment = source;
                            break;
                        case "OpenTK.Graphics.OpenGL.ShaderType.GeometryShader":
                            Geometry = source;
                            break;
                        case "OpenTK.Graphics.OpenGL.ShaderType.TessControlShader":
                            TessControl = source;
                            break;
                        case "OpenTK.Graphics.OpenGL.ShaderType.TessEvaluationShader":
                            TessEvaluation = source;
                            break;
                        case "OpenTK.Graphics.OpenGL.ShaderType.ComputeShader":
                            Compute = source;
                            break;
                    }
                }

                if (Vertex != null)
                    Lines.Add("\t\tpublic static readonly string VertexShaderSource = \"" + Vertex + "\";");
                if (Fragment != null)
                    Lines.Add("\t\tpublic static readonly string FragmentShaderSource = \"" + Fragment + "\";");
                if (Geometry != null)
                    Lines.Add("\t\tpublic static readonly string GeometryShaderSource = \"" + Geometry + "\";");
                if (TessControl != null)
                    Lines.Add("\t\tpublic static readonly string TessControlShaderSource = \"" + TessControl + "\";");
                if (TessEvaluation != null)
                    Lines.Add("\t\tpublic static readonly string TessEvaluationShaderSource = \"" + TessEvaluation + "\";");
                if (Compute != null)
                    Lines.Add("\t\tpublic static readonly string ComputeShaderSource = \"" + Compute + "\";");

                Lines.Add("\t\tpublic static int ProgramID = 0;");
                Lines.Add("\t\tprivate static ShaderRuntime.Utility.Counter Ctr = new ShaderRuntime.Utility.Counter(new Action(delegate{ GL.DeleteProgram(ProgramID); }));");
                Lines.Add("\t\tpublic static void CompileShader()");
                Lines.Add("\t\t{");
                Lines.Add("\t\t\tint prg = GL.CreateProgram();");

                if (Vertex != null)
                {
                    Lines.Add("");
                    Lines.Add("\t\t\tint Vertex = GL.CreateShader(ShaderType.VertexShader);");
                    Lines.Add("\t\t\tGL.ShaderSource(Vertex, VertexShaderSource);");
                    Lines.Add("\t\t\tGL.CompileShader(Vertex);");
                    Lines.Add("\t\t\tDebug.WriteLine(GL.GetShaderInfoLog(Vertex));");
                    Lines.Add("\t\t\tGL.AttachShader(prg, Vertex);");
                }
                if (Fragment != null)
                {
                    Lines.Add("");
                    Lines.Add("\t\t\tint Fragment = GL.CreateShader(ShaderType.FragmentShader);");
                    Lines.Add("\t\t\tGL.ShaderSource(Fragment, FragmentShaderSource);");
                    Lines.Add("\t\t\tGL.CompileShader(Fragment);");
                    Lines.Add("\t\t\tDebug.WriteLine(GL.GetShaderInfoLog(Fragment));");
                    Lines.Add("\t\t\tGL.AttachShader(prg, Fragment);");
                }
                if (Geometry != null)
                {
                    Lines.Add("");
                    Lines.Add("\t\t\tint Geometry = GL.CreateShader(ShaderType.GeometryShader);");
                    Lines.Add("\t\t\tGL.ShaderSource(Geometry, GeometryShaderSource);");
                    Lines.Add("\t\t\tGL.CompileShader(Geometry);");
                    Lines.Add("\t\t\tDebug.WriteLine(GL.GetShaderInfoLog(Geometry));");
                    Lines.Add("\t\t\tGL.AttachShader(prg, Geometry);");
                }
                if (TessControl != null)
                {
                    Lines.Add("");
                    Lines.Add("\t\t\tint TessControl = GL.CreateShader(ShaderType.TessControlShader);");
                    Lines.Add("\t\t\tGL.ShaderSource(TessControl, TessControlShaderSource);");
                    Lines.Add("\t\t\tGL.CompileShader(TessControl);");
                    Lines.Add("\t\t\tDebug.WriteLine(GL.GetShaderInfoLog(TessControl));");
                    Lines.Add("\t\t\tGL.AttachShader(prg, TessControl);");
                }
                if (TessEvaluation != null)
                {
                    Lines.Add("");
                    Lines.Add("\t\t\tint TessEvaluation = GL.CreateShader(ShaderType.TessEvaluationShader);");
                    Lines.Add("\t\t\tGL.ShaderSource(TessEvaluation, TessEvaluationShaderSource);");
                    Lines.Add("\t\t\tGL.CompileShader(TessEvaluation);");
                    Lines.Add("\t\t\tDebug.WriteLine(GL.GetShaderInfoLog(TessEvaluation));");
                    Lines.Add("\t\t\tGL.AttachShader(prg, TessEvaluation);");
                }
                if (Compute != null)
                {
                    Lines.Add("");
                    Lines.Add("\t\t\tint Compute = GL.CreateShader(ShaderType.ComputeShader);");
                    Lines.Add("\t\t\tGL.ShaderSource(Compute, ComputeShaderSource);");
                    Lines.Add("\t\t\tGL.CompileShader(Compute);");
                    Lines.Add("\t\t\tDebug.WriteLine(GL.GetShaderInfoLog(Compute));");
                    Lines.Add("\t\t\tGL.AttachShader(prg, Compute);");
                }

                Lines.Add("");
                Lines.Add("\t\t\tGL.LinkProgram(prg);");
                Lines.Add("\t\t\tDebug.WriteLine(GL.GetProgramInfoLog(prg));");

                if (Vertex != null)
                {
                    Lines.Add("");
                    Lines.Add("\t\t\tGL.DetachShader(prg, Vertex);");
                    Lines.Add("\t\t\tGL.DeleteShader(Vertex);");
                }
                if (Fragment != null)
                {
                    Lines.Add("");
                    Lines.Add("\t\t\tGL.DetachShader(prg, Fragment);");
                    Lines.Add("\t\t\tGL.DeleteShader(Fragment);");
                }
                if (Geometry != null)
                {
                    Lines.Add("");
                    Lines.Add("\t\t\tGL.DetachShader(prg, Geometry);");
                    Lines.Add("\t\t\tGL.DeleteShader(Geometry);");
                }
                if (TessControl != null)
                {
                    Lines.Add("");
                    Lines.Add("\t\t\tGL.DetachShader(prg, TessControl);");
                    Lines.Add("\t\t\tGL.DeleteShader(TessControl);");
                }
                if (TessEvaluation != null)
                {
                    Lines.Add("");
                    Lines.Add("\t\t\tGL.DetachShader(prg, TessEvaluation);");
                    Lines.Add("\t\t\tGL.DeleteShader(TessEvaluation);");
                }
                if (Compute != null)
                {
                    Lines.Add("");
                    Lines.Add("\t\t\tGL.DetachShader(prg, Compute);");
                    Lines.Add("\t\t\tGL.DeleteShader(Compute);");
                }

                foreach(Pair<string, Type> p in Positions)
                {
                    if(p.second == Type.Uniform)
                    {
                        Lines.Add("\t\t\t__" + p.first + " = GL.GetUniformLocation(prg, \"" + p.first + "\");");                        
                    }
                    else
                    {
                        Lines.Add("\t\t\t__" + p.first + " = GL.GetAttribLocation(prg, \"" + p.first + "\");");
                    }
                }

                Lines.Add("\t\t\tProgramID = prg;");
                Lines.Add("\t\t}");
            }

            
            Lines.Add("\t\tpublic void Compile()");
            Lines.Add("\t\t{");
            Lines.Add("\t\t\tif(ProgramID == 0)");
            Lines.Add("\t\t\t\tCompileShader();");
            Lines.Add("\t\t\tCtr++;");
            Lines.Add("\t\t}");
            #endregion
            #region SetParameter
            
            Lines.Add("\t\tpublic void SetParameter<T>(string name, T value)");
            Lines.Add("\t\t{");
            Lines.Add("\t\t\ttry");
            Lines.Add("\t\t\t{");
            Lines.Add("\t\t\t\tswitch(name)");
            Lines.Add("\t\t\t\t{");

            foreach(Property p in Properties)
            {
                Lines.Add("\t\t\t\t\tcase \"" + p.Name + "\":");
                Lines.Add("\t\t\t\t\t\tuniform_" + p.Name + " = (" + p.Type + ")(object)value;");
                Lines.Add("\t\t\t\t\t\tbreak;");
            }


            Lines.Add("\t\t\t\t\tdefault:");
            Lines.Add("\t\t\t\t\t\tthrow new InvalidIdentifierException(\"There is no uniform variable named \" + name + \" in this shader.\");");
            Lines.Add("\t\t\t\t}");
            Lines.Add("\t\t\t}");
            Lines.Add("\t\t\tcatch(InvalidCastException e)");
            Lines.Add("\t\t\t{");
            Lines.Add("\t\t\t\tthrow new InvalidParameterTypeException(\"Invalid parameter type: \" + name + \" is not convertible to the type \\\"\" + typeof(T).FullName + \"\\\".\");");
            Lines.Add("\t\t\t}");
            Lines.Add("\t\t}");
            #endregion
            #region GetParameter
            
            Lines.Add("\t\tpublic T GetParameter<T>(string name)");
            Lines.Add("\t\t{");
            Lines.Add("\t\t\ttry");
            Lines.Add("\t\t\t{");
            Lines.Add("\t\t\t\tswitch(name)");
            Lines.Add("\t\t\t\t{");

            foreach(Property p in Properties)
            {
                Lines.Add("\t\t\t\t\tcase \"" + p.Name + "\":");
                Lines.Add("\t\t\t\t\t\treturn (T)(object)uniform_" + p.Name + ";");
            }

            Lines.Add("\t\t\t\t\tdefault:");
            Lines.Add("\t\t\t\t\t\tthrow new InvalidIdentifierException(\"There is no uniform variable named \" + name + \" in this shader.\");");
            Lines.Add("\t\t\t\t}");
            Lines.Add("\t\t\t}");
            Lines.Add("\t\t\tcatch(InvalidCastException e)");
            Lines.Add("\t\t\t{");
            Lines.Add("\t\t\t\tthrow new InvalidParameterTypeException(\"Invalid paramater type: \" + name + \" is not convertible to the type \\\"\" + typeof(T).FullName + \"\\\".\");");
            Lines.Add("\t\t\t}");
            Lines.Add("\t\t}");
            #endregion
            #region GetParameterLocation
            Lines.Add("\t\tpublic int GetParameterLocation(string name)");
            Lines.Add("\t\t{");
            Lines.Add("\t\t\tswitch(name)");
            Lines.Add("\t\t\t{");

            foreach(Pair<string, Type> p in Positions)
            {
                switch (p.second)
	            {
		            case Type.Uniform:
                        Lines.Add("\t\t\t\tcase \"" + p.first + "\":");
                        Lines.Add("\t\t\t\t\treturn __" + p.first + ";");
                        break;
                    case Type.In:
                        Lines.Add("\t\t\t\tcase \"" + p.first + "\":");
                        Lines.Add("\t\t\t\t\treturn __" + p.first + ";");
                        break;
                    default:
                        break;
	            }
            }

            Lines.Add("\t\t\t\tdefault:");
            Lines.Add("\t\t\t\t\tthrow new InvalidIdentifierException(\"There is no parameter named \" + name + \".\");");
            Lines.Add("\t\t\t}");
            Lines.Add("\t\t}");
            #endregion
            #region PassUniforms

            
            Lines.Add("\t\tpublic void PassUniforms()");
            Lines.Add("\t\t{");
            
            foreach(string str in DrawCommands)
            {
                Lines.Add("\t\t\t" + str);
            }

            Lines.Add("\t\t}");
            #endregion
            #region UseShader
            
            Lines.Add("\t\tpublic void UseShader()");
            Lines.Add("\t\t{");
            Lines.Add("\t\t\tGL.UseProgram(ProgramID);");
            
            foreach(string str in InitCommands)
            {
                Lines.Add("\t\t\t" + str);
            }

            Lines.Add("\t\t}");
            #endregion
            #region GetShaderID
            
            Lines.Add("\t\tpublic int GetShaderID()");
            Lines.Add("\t\t{");
            Lines.Add("\t\t\tif(ProgramID != 0)");
            Lines.Add("\t\t\t\treturn ProgramID;");
            Lines.Add("\t\t\tthrow new ShaderNotInitializedException(\"The shader \\\"" + name + "\\\" has not been initialized. Call Compile() on one of the instances or CompileShader() to compile the shader\");");
            Lines.Add("\t\t}");
            #endregion
            #region Dispose
            
            Lines.Add("\t\tpublic void Dispose()");
            Lines.Add("\t\t{");
            Lines.Add("\t\t\tCtr--;");
            Lines.Add("\t\t}");
            #endregion
            #region IsSupported
            Lines.Add("\t\tpublic bool IsSupported");
            Lines.Add("\t\t{");
            Lines.Add("\t\t\tget");
            Lines.Add("\t\t\t{");
            Lines.Add("\t\t\t\treturn ImplementationSupportsShaders;");
            Lines.Add("\t\t\t}");
            Lines.Add("\t\t}");
            #endregion

            Lines.Add("\t}");
            Lines.Add("}");

            StreamWriter Stream = new StreamWriter(args[0]);

            foreach(string line in Lines)
            {
                Stream.WriteLine(Regex.Replace(line, "\t", "    "));
            }

            Stream.Close();
        }
    }
}
