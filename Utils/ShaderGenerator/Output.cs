using OpenTK.Graphics.OpenGL;
using System;

#pragma warning disable 168

namespace Shaders
{
	class Input : Shader.IGLShader
	{
		public bool TransposeMatrix = false;
		//public int ProgramID;

		private static int CompileShader(ShaderType stage, string source)
		{
			int shader = GL.CreateShader(stage);
			GL.ShaderSource(shader, 1, source, 0);
			GL.CompileShader(shader);
			return shader;
		}

		public int __MVP;
		public int __vertex;

		OpenTK.Matrix4 uniform_MVP;

		public static readonly string VertexShaderSource = "#version 430\n\n#pragma name Input\n#pragma stage Vertex\n\nuniform mat4 MVP;\nlayout(location = 1) in vec3 vertex;\n\nsmooth out vec3 ovec;\n\nvoid main()\n{\n	\n}";
		public static int ProgramID = 0;
		private static ShaderRuntime.Utility.Counter Ctr = new ShaderRuntime.Utility.Counter(new Action(delegate{ GL.DeleteProgram(ProgramID); }));
		public static void CompileShader()
		{
			int prg = GL.CreateProgram();
			int Vertex;
			int Fragment;
			int Geometry;
			int TessControl;
			int TessEvalutation;
			int Compute;
			Vertex = GL.CreateShader(ShaderType.VertexShader);
			GL.ShaderSource(Vertex, VertexShaderSource);
			GL.CompileShader(Vertex);
			GL.AttachShader(prg, Vertex);
			GL.LinkProgram(prg);
			GL.DetachShader(prg, Vertex);
			GL.DeleteShader(Vertex);
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
					default:
						throw new Shader.InvalidIdentifierException("There is no uniform variable named " + name + " in this shader.");
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
					default:
						throw new InvalidIdentifierException("There is no uniform variable named " + name + " in this shader.");
				}
			}
			catch(InvalidCastException e)
			{
				throw new InvalidParameterTypeException("Invalid paramater type: " + name + " is not convertible to the type \"" + typeof(T).FullName + "\".");
			}
		}
		public int GetParameterID(string name)
		{
			switch(name)
			{
				case "MVP":
					return __MVP;
				case "vertex":
					return __vertex;
				default:
					throw new InvalidIdentifierException("There is no parameter named " + name + ".");
			}
		}

		public void PassUniforms()
		{
			GL.UniformMatrix4(__uniform_MVP, TransposeMatrix, uniform_MVP);
		}
	}
}
