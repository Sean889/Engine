using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace PlanetLib
{
    using dvec3 = OpenTK.Vector3d;
    using fvec3 = OpenTK.Vector3;

    /*
        Quadrant info

         _______________
        |Nwc	|		|Nec
        |	0	|	1	|
        |_______|_______|
        |		|		|
        |	2	|	3	|
        |_______|_______|
        Swc				 Sec
    */

    /// <summary>
    /// A quadtree patch.
    /// </summary>
    public class Patch
    {
        internal const uint SIDE_LEN = 33;
        internal const uint SKIRT_DEPTH = 5000;
        internal const uint NUM_VERTICES = (SIDE_LEN * SIDE_LEN + SIDE_LEN * 4);
        internal const uint NUM_INDICES = (SIDE_LEN - 1) * (SIDE_LEN - 1) * 6 + 24 * (SIDE_LEN - 1);

        private const uint PATCH_MULT = 8;
        private const uint DIS_MULT = 1;

        /// <summary>
        /// The number of vertices on each side of the patch mesh.
        /// </summary>
        public static readonly uint MeshSideLen = SIDE_LEN;
        /// <summary>
        /// The depth of the skirt below the surface.
        /// </summary>
        public static readonly uint MeshSkirtDepth = SKIRT_DEPTH;
        /// <summary>
        /// The number of vertices in the patch mesh.
        /// </summary>
        public static readonly uint NumVertices = NUM_VERTICES;
        /// <summary>
        /// The number of indices that are contained in Indices.
        /// </summary>
        public static readonly uint NumIndices = NUM_INDICES;

        /// <summary>
        /// The indices for the patch mesh.
        /// </summary>
        public static readonly uint[] Indices = GetIndices();

        /// <summary>
        /// Northwest quadrant.
        /// </summary>
        public Patch Nw;
        /// <summary>
        /// Northeas quadrant.
        /// </summary>
        public Patch Ne;
        /// <summary>
        /// Southwest quadrant.
        /// </summary>
        public Patch Sw;
        /// <summary>
        /// Southeast quadrant.
        /// </summary>
        public Patch Se;

        /// <summary>
        /// The northwest corner of the patch on the base cube.
        /// </summary>
        public Vector3d Nwc;
        /// <summary>
        /// The northeast corner of the patch on the base cube.
        /// </summary>
        public Vector3d Nec;
        /// <summary>
        /// The southwest corner of the patch on the base cube.
        /// </summary>
        public Vector3d Swc;
        /// <summary>
        /// The northeast corner of the patch on the base cube.
        /// </summary>
        public Vector3d Sec;

        /// <summary>
        /// The side length of the patch on the baase cube.
        /// </summary>
        public double SideLen;
        /// <summary>
        /// THe radius of the planet this patch is part of.
        /// </summary>
        public double PlanetRadius;

        /// <summary>
        /// The patch's position relative to the planet
        /// </summary>
        public Vector3d Offset;
        /// <summary>
        /// The level of this quadrant in the quadtee.
        /// </summary>
        public uint Level;
        /// <summary>
        /// The parent of this node in the quadtree.
        /// </summary>
        public Patch Parent;

        internal Executor Executor;
        internal Buffer Buffer;

        /// <summary>
        /// The mesh data of this patch.
        /// </summary>
        public Vector3[,] MeshData;

        /// <summary>
        /// Whether the patch has children
        /// </summary>
        public bool IsSubdivided
        {
            get 
            {
                return Nw == null;
            }
        }
        /// <summary>
        /// Whether the patch has mesh data.
        /// </summary>
        public bool HasMeshData
        {
            get
            {
                return MeshData != null;
            }
        }
        /// <summary>
        /// Whether the node is the root of the quadtree.
        /// </summary>
        public bool IsRoot
        {
            get
            {
                return Parent != null;
            }
        }

        /// <summary>
        /// Returns all the children of the patch in this order: Nw, Ne, Sw then Se.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Patch> GetSides()
        {
            yield return Nw;
            yield return Ne;
            yield return Sw;
            yield return Se;
        }

        /// <summary>
        /// Checks whether the patch should subdivide
        /// </summary>
        /// <param name="CamPos"> The current camera position. </param>
        /// <returns></returns>
        public bool ShouldSubdivide(Vector3d CamPos)
        {
            double dis = (Offset - CamPos).Length - SideLen;
			return SideLen >= (SIDE_LEN - 1) * PATCH_MULT && dis < SideLen * DIS_MULT;
        }
        /// <summary>
        /// Checks whether the patch should merge it's children
        /// </summary>
        /// <param name="CamPos"> The current camera position. </param>
        /// <returns></returns>
        public bool ShouldMerge(Vector3d CamPos)
        {
            double dis = (Offset - CamPos).Length - SideLen;
            return dis > SideLen * DIS_MULT;
        }

        /// <summary>
        /// Checks whether the patch should merge it's children or subdivide and performs the appropriate action.
        /// </summary>
        /// <param name="CamPos"> The current camera position. </param>
        /// <returns> Whether the tree was modified. </returns>
        public bool CheckAndSubdivide(Vector3d CamPos)
        {
            //If the patch does not exist
			if (this == null)
				//Don't do anything, quick escape
				return false;
			//If the patch is subdivided
			if (IsSubdivided)
			{
				//If the patch is far enough away that it should be merged
				if (ShouldMerge(CamPos))
				{
					//Add the patch to the renderer's list to render
                    Executor.AddPatch(this);
					//Delete the patch's children
                    MergeChildren();
					//Indicate that a change occured
					return true;
				}
				else
				{
					//If the patch should not be merged, continue recursive check
					bool r1 = CheckAndSubdivide(CamPos);
					bool r2 = CheckAndSubdivide(CamPos);
					bool r3 = CheckAndSubdivide(CamPos);
					bool r4 = CheckAndSubdivide(CamPos);

					//Return results
					return r1 || r2 || r3 || r4;
				}
			}
			//If the patch should be subdivided
			else if (ShouldSubdivide(CamPos))
			{
				//Split but don't generate data
				Split();
				//Do that on another thread
				//Also put the patch in the remove list
				Executor.GenMeshData(Nw, Ne, Sw, Se);
				//Indicate somthing happened
				return true;
			}
			//Nothing happened
			return false;
        }

        /// <summary>
        /// Generates the mesh data for this patch.
        /// </summary>
        public void GenData()
        {
            const double INTERP = 1.0 / (SIDE_LEN - 1);

			//Avoid using a atomic value before the array is initialized completely, use temporary instead
            Vector3[,] mesh_data_ptr = new Vector3[NUM_VERTICES, 2];

			for (uint x = 0; x < SIDE_LEN; x++)
			{
				//Calcualte horizontal position
				double interp = INTERP * (double)x;
				dvec3 v1 = dvec3.Lerp(Nwc, Nec, interp);
				dvec3 v2 = dvec3.Lerp(Swc, Sec, interp);
				for (uint y = 0; y < SIDE_LEN; y++)
				{
					//Calculate vertical position
					dvec3 vtx = dvec3.Lerp(v1, v2, INTERP * (double)y);
					dvec3 nvtx = vtx.Normalized();
					//Map to sphere
					vtx = nvtx * PlanetRadius;
					//Assign vertex position
					mesh_data_ptr[x * SIDE_LEN + y, 0] = (fvec3)(vtx - Offset);
					//Texcoord is normal as well, data compactness
					mesh_data_ptr[x * SIDE_LEN + y, 1] = (fvec3)nvtx;
				}
			}

			//Skirt generation code

			/*
				Skirt is the position of the surface, but SKIRT_DEPTH units lower

				Calculate position on the sphere, then subtract SKIRT_DEPTH units
				Texture coordinate is still just normalized position
			*/

			//Vertex normal releative to planet centre
			dvec3 vnrm;
			//Sizeof base surface data
			uint data_size = SIDE_LEN * SIDE_LEN;
			for (uint i = 0; i < SIDE_LEN; i++)
			{
				
				vnrm = dvec3.Lerp(Nwc, Swc, INTERP * (double)i).Normalized();
				mesh_data_ptr[data_size + i, 0] = (fvec3)((vnrm * PlanetRadius - vnrm * SKIRT_DEPTH) - Offset);
				mesh_data_ptr[data_size + i, 1] = (fvec3)vnrm;
			}
			data_size += SIDE_LEN;
			for (uint i = 0; i < SIDE_LEN; i++)
			{
				vnrm = dvec3.Lerp(Swc, Sec, INTERP * (double)i).Normalized();
				mesh_data_ptr[data_size + i, 0] = (fvec3)((vnrm * PlanetRadius - vnrm * SKIRT_DEPTH) - Offset);
				mesh_data_ptr[data_size + i, 1] = (fvec3)vnrm;
			}
			data_size += SIDE_LEN;
			for (uint i = 0; i < SIDE_LEN; i++)
			{
				vnrm = dvec3.Lerp(Nec, Sec, INTERP * (double)i).Normalized();
				mesh_data_ptr[data_size + i, 0] = (fvec3)((vnrm * PlanetRadius - vnrm * SKIRT_DEPTH) - Offset);
				mesh_data_ptr[data_size + i, 1] = (fvec3)vnrm;
			}
			data_size += SIDE_LEN;
			for (uint i = 0; i < SIDE_LEN; i++)
			{
				vnrm = dvec3.Lerp(Nwc, Nec, INTERP * (double)i).Normalized();
				mesh_data_ptr[data_size + i, 0] = (fvec3)((vnrm * PlanetRadius - vnrm * SKIRT_DEPTH) - Offset);
				mesh_data_ptr[data_size + i, 1] = (fvec3)vnrm;
			}

            MeshData = mesh_data_ptr;
            Buffer = new Buffer(mesh_data_ptr, Offset);
        }
        /// <summary>
        /// Splits the patch into 4 quadrants but doesn't generate any mesh data for them.
        /// </summary>
        public void Split()
        {
            //Split, but don't generate mesh data for children
			dvec3 centre = (Nwc + Nec + Swc + Sec) * 0.25;

			Nw = new Patch(Nwc, (Nwc + Nec) * 0.5, (Nwc + Swc) * 0.5, centre, PlanetRadius, SideLen * 0.5, Executor, this, Level + 1);
			Ne = new Patch((Nwc + Nec) * 0.5, Nec, centre, (Nec + Sec) * 0.5, PlanetRadius, SideLen * 0.5, Executor, this, Level + 1);
			Sw = new Patch((Nwc + Swc) * 0.5, centre, Swc, (Swc + Sec) * 0.5, PlanetRadius, SideLen * 0.5, Executor, this, Level + 1);
			Se = new Patch(centre, (Nec + Sec) * 0.5, (Swc + Sec) * 0.5, Sec, PlanetRadius, SideLen * 0.5, Executor, this, Level + 1);
        }
        /// <summary>
        /// Deletes the children of this patch and adds this patch to the rendering system.
        /// </summary>
        public void MergeChildren()
        {
            Nw.OnDelete();
            Ne.OnDelete();
            Sw.OnDelete();
            Se.OnDelete();

            Nw = null;
            Ne = null;
            Sw = null;
            Se = null;

            Executor.AddPatch(this);
        }

        internal Patch(dvec3 Nwc, dvec3 Nec, dvec3 Swc, dvec3 Sec, double pl_r, double side_len, Executor sched, Patch parent, uint level)
        {
            this.Nwc = Nwc;
            this.Nec = Nec;
            this.Swc = Swc;
            this.Sec = Sec;

            this.Nw = this.Ne = this.Sw = this.Se = null;
            this.MeshData = null;

            this.Offset = ((Nwc + Nec + Swc + Sec)).Normalized() * pl_r;

            this.PlanetRadius = pl_r;

            this.SideLen = side_len;

            this.Executor = sched;

            this.Parent = parent;
        }
        internal void OnDelete()
        {
            Executor.DeletePatch(this);
        }

        private static uint[] GetIndices()
        {
            uint[] indices = new uint[NUM_INDICES];
            uint idx = 0;

            for (uint y = 0; y < SIDE_LEN - 1; y++)
			{
				for (uint x = 0; x < SIDE_LEN - 1; x++)
				{
					//First triangle
					indices[idx++] = y * SIDE_LEN + x;
					indices[idx++] = y * SIDE_LEN + x + 1;
					indices[idx++] = (y + 1) * SIDE_LEN + x;

					//Second triangle
					indices[idx++] = y * SIDE_LEN + x + 1;
					indices[idx++] = (y + 1) * SIDE_LEN + x + 1;
					indices[idx++] = (y + 1) * SIDE_LEN + x;
				}
			}

			//Generate indices for skirt

			for (uint i = 0; i < SIDE_LEN - 1; i++)
			{
				//Top side
				indices[idx++] = SIDE_LEN * SIDE_LEN + i;
				indices[idx++] = SIDE_LEN * SIDE_LEN + i + 1;
				indices[idx++] = i;

				indices[idx++] = SIDE_LEN * SIDE_LEN + i + 1;
				indices[idx++] = i + 1;
				indices[idx++] = i;

				//Right side
				indices[idx++] = SIDE_LEN * (i + 1) - 1;
				indices[idx++] = SIDE_LEN * SIDE_LEN + SIDE_LEN + i;
				indices[idx++] = SIDE_LEN * (i + 2) - 1;

				indices[idx++] = SIDE_LEN * SIDE_LEN + SIDE_LEN + i;
				indices[idx++] = SIDE_LEN * SIDE_LEN + SIDE_LEN + i + 1;
				indices[idx++] = SIDE_LEN * (i + 2) - 1;

				//Bottom side
				indices[idx++] = (SIDE_LEN - 1) * SIDE_LEN + i;
				indices[idx++] = (SIDE_LEN - 1) * SIDE_LEN + i + 1;
				indices[idx++] = SIDE_LEN * (SIDE_LEN + 2) + i;

				indices[idx++] = (SIDE_LEN - 1) * SIDE_LEN + i + 1;
				indices[idx++] = SIDE_LEN * (SIDE_LEN + 2) + i + 1;
				indices[idx++] = SIDE_LEN * (SIDE_LEN + 2) + i;

				//Left side
				indices[idx++] = SIDE_LEN * (SIDE_LEN + 3) + i;
				indices[idx++] = SIDE_LEN * i;
				indices[idx++] = SIDE_LEN * (SIDE_LEN + 3) + i + 1;

				indices[idx++] = SIDE_LEN * i;
				indices[idx++] = SIDE_LEN * (i + 1);
				indices[idx++] = SIDE_LEN * (SIDE_LEN + 3) + i + 1;
			}
			return indices;
        }
     
    }
}
