using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using EngineSystem;
using RenderSystem;
using Shader;

using TP = ThreadPool.ThreadPoolManager;

namespace PlanetLib
{
    internal class Executor
    {
        private struct DrawItem
        {
            internal int Id;
            internal Vector3d Offset;

            internal DrawItem(int Id, Vector3d Offset)
            {
                this.Id = Id;
                this.Offset = Offset;
            }
            internal DrawItem(int Id)
            {
                this.Id = Id;
                Offset = new Vector3d();
            }

            public static bool operator ==(DrawItem lhs, DrawItem rhs)
            {
                return lhs.Id == rhs.Id;
            }
            public static bool operator !=(DrawItem lhs, DrawItem rhs)
            {
                return !(lhs == rhs);
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

        private ConcurrentQueue<Buffer> ToUpload = new ConcurrentQueue<Buffer>();
        private ConcurrentQueue<Buffer> ToAdd = new ConcurrentQueue<Buffer>();
        private ConcurrentQueue<Buffer> ToRemove = new ConcurrentQueue<Buffer>();
        private ConcurrentQueue<Buffer> ToDelete = new ConcurrentQueue<Buffer>();

        private static Matrix4 Convert(Matrix4d i)
        {
            return new Matrix4(
                (float)i[0, 0], (float)i[0, 1], (float)i[0, 2], (float)i[0, 3],
                (float)i[1, 0], (float)i[1, 1], (float)i[1, 2], (float)i[1, 3],
                (float)i[2, 0], (float)i[2, 1], (float)i[2, 2], (float)i[2, 3],
                (float)i[3, 0], (float)i[3, 1], (float)i[3, 2], (float)i[3, 3]);
        }
        private static Vector3 Convert(Vector3d i)
        {
            return new Vector3((float)i.X, (float)i.Y, (float)i.Z);
        }

        private List<Buffer> Buffers = new List<Buffer>();
        private List<DrawItem> Vbos = new List<DrawItem>();

        private int IboID;
        private GLShader Shader;

        // Generates the mesh data for the patches and removes their parent
        internal void GenMeshData(Patch nw, Patch ne, Patch sw, Patch se)
        {
            TP.QueueAutoTask(new Action(delegate
            {
                //Generate the meshes for the children
                nw.GenData();
                ne.GenData();
                sw.GenData();
                se.GenData();

                //Start rendering the children
                ToUpload.Enqueue(nw.Buffer);
                ToUpload.Enqueue(ne.Buffer);
                ToUpload.Enqueue(sw.Buffer);
                ToUpload.Enqueue(se.Buffer);

                //Stop rendering the parent in the tree.
                ToRemove.Enqueue(nw.Parent.Buffer);
            }));
        }
        // Removes the patch from the render list
        internal void RemovePatch(Patch p)
        {
            ToRemove.Enqueue(p.Buffer);
        }
        // Deletes the buffer of the patch
        internal void DeletePatch(Patch p)
        {
            ToDelete.Enqueue(p.Buffer);
        }
        // Re-adds an existing patch to the render list
        internal void AddPatch(Patch p)
        {
            ToAdd.Enqueue(p.Buffer);
        }
        // Uploads the mesh data of the patch
        internal void UploadPatch(Patch p)
        {
            ToUpload.Enqueue(p.Buffer);
        }

        /// <summary>
        /// Draws the planet.
        /// </summary>
        /// <param name="PlanetTrans"> The transform of the planet. </param>
        /// <param name="CamMat"> A matrix containing all the transforms for the camera. </param>
        /// <param name="CamPos"> The camera position. </param>
        /// <param name="LightDir"> The direction of the light. </param>
        internal void DrawPlanet(Coord PlanetTrans, Matrix4d CamMat, Vector3d CamPos, Vector3d LightDir)
        {
            Buffer Buf;
            while(ToUpload.TryDequeue(out Buf))
            {
                int ID = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, ID);
                GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(sizeof(float) * Patch.NUM_VERTICES * 6), Buf.Verts, BufferUsageHint.StaticDraw);

                Buf.VboID = ID;
                Vbos.Add(new DrawItem(ID, Buf.Offset));
                Buffers.Add(Buf);
            }

            while(ToAdd.TryDequeue(out Buf))
            {
                Vbos.Add(new DrawItem(Buf.VboID, Buf.Offset));
            }

            while(ToDelete.TryDequeue(out Buf))
            {
                Buffers.Remove(Buf);
                Vbos.Remove(new DrawItem(Buf.VboID));
                GL.DeleteBuffer(Buf.VboID);
            }

            while(ToRemove.TryDequeue(out Buf))
            {
                Vbos.Remove(new DrawItem(Buf.VboID));
            }

            Shader.UseShader();

            Matrix4d PlanetMat = Matrix4d.Rotate(PlanetTrans.Rotation) * Matrix4d.CreateTranslation(PlanetTrans.Position);
            Vector3d RelCamPos = Vector3d.Transform(CamPos, PlanetMat.Inverted());
            Matrix4 MVP;

            Vector3 RelLightDir = Convert(Vector3d.Transform(LightDir, PlanetMat.ExtractRotation()));

            unsafe
            {
                foreach (DrawItem Item in Vbos)
                {
                    //Calculate final mvp matrix
                    MVP = Convert(CamMat * PlanetMat * Matrix4d.CreateTranslation(Item.Offset));

                    //Bind vertex data
                    GL.BindBuffer(BufferTarget.ArrayBuffer, Item.Id);

                    //Setup vertex attributes
                    GL.VertexAttribPointer(Shader.GetParameterLocation("Vertex"), 3, VertexAttribPointerType.Float, false, sizeof(Vector3d), 0);
                    GL.VertexAttribPointer(Shader.GetParameterLocation("Texcoord"), 3, VertexAttribPointerType.Float, true, sizeof(Vector3d), (IntPtr)sizeof(Vector3d));

                    //Pass matrices
                    Shader.SetParameter("MVP", MVP);

                    //Light direction
                    Shader.SetParameter("RelLightDir", RelLightDir);

                    Shader.PassUniforms();

                    //Draw call
                    GL.DrawElements(PrimitiveType.Triangles, (int)Patch.NUM_INDICES, DrawElementsType.UnsignedInt, IntPtr.Zero);
                }
            }
        }

        internal void Dispose()
        {
            {
                Buffer Buf;
                while (ToUpload.TryDequeue(out Buf))
                {
                    int ID = GL.GenBuffer();
                    GL.BindBuffer(BufferTarget.ArrayBuffer, ID);
                    GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(sizeof(float) * Patch.NUM_VERTICES * 6), Buf.Verts, BufferUsageHint.StaticDraw);

                    Buf.VboID = ID;
                    Vbos.Add(new DrawItem(ID, Buf.Offset));
                    Buffers.Add(Buf);
                }
            }

            foreach(Buffer Buf in Buffers)
            {
                GL.DeleteBuffer(Buf.VboID);
            }
        }

        internal Executor(GraphicsSystem Sys, GLShader Shader)
        {
            Sys.GraphicsThread.ScheduleRenderTask(new Action(delegate
            {
                IboID = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, IboID);
                GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(sizeof(uint) * Patch.NUM_INDICES), Patch.Indices, BufferUsageHint.StaticDraw);
            }));

            this.Shader = Shader;
        }
    }
}
