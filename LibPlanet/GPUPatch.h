#ifndef LIBLOD3D_GPU_PATCH_H
#define LIBLOD3D_GPU_PATCH_H

#include "Math.h"
#include "GPURenderer.h"

namespace lod3d
{
	using namespace math;

	namespace gpu
	{
		namespace detail
		{
			const unsigned int* get_indices32(void);
			const unsigned short* get_indices16(void);

			template<bool _Cond, typename _Tt, typename _Ft> struct tmp_if;

			template<typename _Tt, typename _Ft> struct tmp_if<true, _Tt, _Ft>
			{
				typedef _Tt type;
				static const bool value = true;
			};
			template<typename _Tt, typename _Ft> struct tmp_if<false, _Tt, _Ft>
			{
				typedef _Ft type;
				static const bool value = false;
			};

			template<size_t SIZE, typename _Ty = tmp_if<(SIZE < (1 << (16))), unsigned short, unsigned int>::type> struct util_type;

			template<size_t SIZE> struct util_type<SIZE, unsigned short>
			{
				typedef unsigned short type;

				inline static const type* get_indices(void)
				{
					return get_indices16();
				}
			};
			template<size_t SIZE> struct util_type<SIZE, unsigned int>
			{
				typedef unsigned int type;

				inline static const type* get_indices(void)
				{
					return get_indices32();
				}
			};
		}

		/*
		Quadrant info

		 _______________
		|nwc	|		|nec
		|	0	|	1	|
		|_______|_______|
		|		|		|
		|	2	|	3	|
		|_______|_______|
		swc				 sec
		*/

		class Patch
		{
		public:
			//Number of vertices to a side
			static const size_t SIDE_LEN = 33;
			//Depth of the skirt below the surface
			static const size_t SKIRT_DEPTH = 5000;
			//Number of vertices in the mesh
			static const size_t NUM_VERTICES = (SIDE_LEN * SIDE_LEN + SIDE_LEN * 4);
			//Number of indices in the index array returned by get_indices()
			static const size_t NUM_INDICES = (SIDE_LEN - 1) * (SIDE_LEN - 1) * 6 + 24 * (SIDE_LEN - 1);
			
			//Will be either unsigned int and unsigned short depending on the number of vertices in the mesh
			typedef detail::util_type<NUM_VERTICES>::type vert_type;

			struct Vertex
			{
				//Actual vertex
				fvec3 vertex;
				//Texcoord and normal should be the same for cube maps
				fvec3 texcoord;
			};

			//Returns and array with num indices elements and will have the type of vert_type
			static const vert_type* get_indices()
			{
				return detail::util_type<NUM_VERTICES>::get_indices();
			}

			//Corners of the patch on the base cube
			dvec3 nwc, nec, swc, sec;

			dvec3 pos;	//Position on the sphere

			std::shared_ptr<Patch> nw;	//Quadrant 0
			std::shared_ptr<Patch> ne;	//Quadrant 1
			std::shared_ptr<Patch> sw;	//Quadrant 2
			std::shared_ptr<Patch> se;	//Quadrant 3

			double planet_radius;			//Radius of the planet this patch is part of
			double side_len;				//Side length of the patch
			unsigned int level;				//Level within the quadtree
			volatile unsigned int g_id;		//Graphics identifier

			Renderer* executor;			//Task executor
			Patch* parent;	//Parent node in the quadtree

			std::atomic<Vertex*> mesh_data;		//Mesh data for the patch

			//Checks whether the patch is subdivided
			inline bool is_subdivided(void)
			{
				return nw != nullptr;
			}

			LIBLOD3D_API Patch(dvec3 nwc, dvec3 nec, dvec3 swc, dvec3 sec, double planet_radius, double side_len, Renderer* exec, Patch* parent = nullptr, unsigned int level = 0);
			LIBLOD3D_API ~Patch(void);

			//Generates the data for the patch
			LIBLOD3D_API void gen_data(void);

			//NOT THREAD SAFE
			//Fully expands the mesh to the optimal level
			LIBLOD3D_API void force_subdivide(dvec3& cam_pos);

			//Merges the children of this node
			LIBLOD3D_API void merge_children(void);

			//Subdivides and generates mesh data
			LIBLOD3D_API void subdivide(void);

			//Just splits the quadtree node and does not generate mesh data
			LIBLOD3D_API void split(void);

			//Runs a check and subdivide operation
			LIBLOD3D_API static bool check_and_subdivide(std::shared_ptr<Patch> patch, dvec3& cam_pos);
		};
	}
}

#endif
