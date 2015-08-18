using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using ShaderRuntime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;

namespace DrawTest
{
    unsafe class Program
    {
        [DllImport("opengl32.dll", EntryPoint = "glDrawElements")]
        static extern unsafe void DrawElements(int v1, int v2, int v3, void* v4);
        delegate void ImitDel(int mode, int count, int type, void* indices);

        static uint[] indices = { 0, 1, 2 };
        static Vector2[] vertices = 
            {
                new Vector2(-0.5f, -0.5f),
                new Vector2(0f, 0.5f),
                new Vector2(0.5f, -0.5f)
            };

        static void Inject()
        {
            IntPtr ip = Marshal.GetFunctionPointerForDelegate(new ImitDel(DrawElements));
            FieldInfo Info = typeof(GL).GetField("EntryPoints", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            IntPtr[] Array = (IntPtr[])Info.GetValue(null);
            Array[0x1b6] = ip;
        }

        static void Main(string[] args)
        {
            using (GameWindow Win = new GameWindow(1080, 720, GraphicsMode.Default, "Test Window", GameWindowFlags.Default, DisplayDevice.Default, 4, 4, GraphicsContextFlags.Default))
            {
                GLShader Shader = new Shaders.Shader();

                Win.Visible = true;
                Win.MakeCurrent();

                Inject();

                int ibo = GL.GenBuffer();
                int vbo = GL.GenBuffer();

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);
                GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

                GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(12), indices, BufferUsageHint.StaticDraw);
                GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(sizeof(float) * 6), vertices, BufferUsageHint.StaticDraw);

                GL.PointSize(100);

                GL.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
                
                Shader.Compile();

                Win.RenderFrame += (sender, e) =>
                    {
                        GL.BindBuffer(BufferTarget.ElementArrayBuffer, ibo);

                        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);

                        Shader.UseShader();

                        GL.VertexAttribPointer(Shader.GetParameterLocation("Vertex"), 3, VertexAttribPointerType.Float, false, 0, 0);

                        Shader.PassUniforms();

                        GL.DrawElements(BeginMode.Points, 3, DrawElementsType.UnsignedInt, 0);

                        Win.SwapBuffers();
                    };

                Win.Run();

                Shader.Dispose();
            }
        }
    }
}
