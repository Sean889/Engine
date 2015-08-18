#include "GPUPatch.h"

#pragma warning (disable:4244)

namespace lod3d
{
	namespace gpu
	{
		namespace detail
		{
			//Creates a single instance array, then returns that array each time the function is called

#pragma warning (disable:4242)
			//Values using unsigned short
			const unsigned short* get_indices16(void)
			{
				struct temp
				{
					static unsigned short* get_indices16_internal(void)
					{
						unsigned short* indices = new unsigned short[Patch::NUM_INDICES];

						size_t idx = 0;

						for (size_t y = 0; y < Patch::SIDE_LEN - 1; y++)
						{
							for (size_t x = 0; x < Patch::SIDE_LEN - 1; x++)
							{
								//First triangle
								indices[idx++] = y * Patch::SIDE_LEN + x;
								indices[idx++] = y * Patch::SIDE_LEN + x + 1;
								indices[idx++] = (y + 1) * Patch::SIDE_LEN + x;

								//Second triangle
								indices[idx++] = y * Patch::SIDE_LEN + x + 1;
								indices[idx++] = (y + 1) * Patch::SIDE_LEN + x + 1;
								indices[idx++] = (y + 1) * Patch::SIDE_LEN + x;
							}
						}

						//Generate indices for skirt

						for (size_t i = 0; i < Patch::SIDE_LEN - 1; i++)
						{
							//Top side
							indices[idx++] = Patch::SIDE_LEN * Patch::SIDE_LEN + i;
							indices[idx++] = Patch::SIDE_LEN * Patch::SIDE_LEN + i + 1;
							indices[idx++] = i;

							indices[idx++] = Patch::SIDE_LEN * Patch::SIDE_LEN + i + 1;
							indices[idx++] = i + 1;
							indices[idx++] = i;

							//Right side
							indices[idx++] = Patch::SIDE_LEN * (i + 1) - 1;
							indices[idx++] = Patch::SIDE_LEN * Patch::SIDE_LEN + Patch::SIDE_LEN + i;
							indices[idx++] = Patch::SIDE_LEN * (i + 2) - 1;

							indices[idx++] = Patch::SIDE_LEN * Patch::SIDE_LEN + Patch::SIDE_LEN + i;
							indices[idx++] = Patch::SIDE_LEN * Patch::SIDE_LEN + Patch::SIDE_LEN + i + 1;
							indices[idx++] = Patch::SIDE_LEN * (i + 2) - 1;

							//Bottom side
							indices[idx++] = (Patch::SIDE_LEN - 1) * Patch::SIDE_LEN + i;
							indices[idx++] = (Patch::SIDE_LEN - 1) * Patch::SIDE_LEN + i + 1;
							indices[idx++] = Patch::SIDE_LEN * (Patch::SIDE_LEN + 2) + i;

							indices[idx++] = (Patch::SIDE_LEN - 1) * Patch::SIDE_LEN + i + 1;
							indices[idx++] = Patch::SIDE_LEN * (Patch::SIDE_LEN + 2) + i + 1;
							indices[idx++] = Patch::SIDE_LEN * (Patch::SIDE_LEN + 2) + i;

							//Left side
							indices[idx++] = Patch::SIDE_LEN * (Patch::SIDE_LEN + 3) + i;
							indices[idx++] = Patch::SIDE_LEN * i;
							indices[idx++] = Patch::SIDE_LEN * (Patch::SIDE_LEN + 3) + i + 1;

							indices[idx++] = Patch::SIDE_LEN * i;
							indices[idx++] = Patch::SIDE_LEN * (i + 1);
							indices[idx++] = Patch::SIDE_LEN * (Patch::SIDE_LEN + 3) + i + 1;
						}
						return indices;
					}
				};

				static const unsigned short* indices = temp::get_indices16_internal();
				return indices;
			}
			//Values using unsigned int
			const unsigned int* get_indices32(void)
			{
				struct temp
				{
					static unsigned int* get_indices32_internal(void)
					{
						unsigned int* indices = new unsigned int[Patch::NUM_INDICES];

						size_t idx = 0;

						for (size_t y = 0; y < Patch::SIDE_LEN - 1; y++)
						{
							for (size_t x = 0; x < Patch::SIDE_LEN - 1; x++)
							{
								//First triangle
								indices[idx++] = y * Patch::SIDE_LEN + x;
								indices[idx++] = y * Patch::SIDE_LEN + x + 1;
								indices[idx++] = (y + 1) * Patch::SIDE_LEN + x;

								//Second triangle
								indices[idx++] = y * Patch::SIDE_LEN + x + 1;
								indices[idx++] = (y + 1) * Patch::SIDE_LEN + x + 1;
								indices[idx++] = (y + 1) * Patch::SIDE_LEN + x;
							}
						}

						//Generate indices for skirt

						for (size_t i = 0; i < Patch::SIDE_LEN - 1; i++)
						{
							//Top side
							indices[idx++] = Patch::SIDE_LEN * Patch::SIDE_LEN + i;
							indices[idx++] = Patch::SIDE_LEN * Patch::SIDE_LEN + i + 1;
							indices[idx++] = i;

							indices[idx++] = Patch::SIDE_LEN * Patch::SIDE_LEN + i + 1;
							indices[idx++] = i + 1;
							indices[idx++] = i;

							//Right side
							indices[idx++] = Patch::SIDE_LEN * (i + 1) - 1;
							indices[idx++] = Patch::SIDE_LEN * Patch::SIDE_LEN + Patch::SIDE_LEN + i;
							indices[idx++] = Patch::SIDE_LEN * (i + 2) - 1;

							indices[idx++] = Patch::SIDE_LEN * Patch::SIDE_LEN + Patch::SIDE_LEN + i;
							indices[idx++] = Patch::SIDE_LEN * Patch::SIDE_LEN + Patch::SIDE_LEN + i + 1;
							indices[idx++] = Patch::SIDE_LEN * (i + 2) - 1;

							//Bottom side
							indices[idx++] = (Patch::SIDE_LEN - 1) * Patch::SIDE_LEN + i;
							indices[idx++] = (Patch::SIDE_LEN - 1) * Patch::SIDE_LEN + i + 1;
							indices[idx++] = Patch::SIDE_LEN * (Patch::SIDE_LEN + 2) + i;

							indices[idx++] = (Patch::SIDE_LEN - 1) * Patch::SIDE_LEN + i + 1;
							indices[idx++] = Patch::SIDE_LEN * (Patch::SIDE_LEN + 2) + i + 1;
							indices[idx++] = Patch::SIDE_LEN * (Patch::SIDE_LEN + 2) + i;

							//Left side
							indices[idx++] = Patch::SIDE_LEN * (Patch::SIDE_LEN + 3) + i;
							indices[idx++] = Patch::SIDE_LEN * i;
							indices[idx++] = Patch::SIDE_LEN * (Patch::SIDE_LEN + 3) + i + 1;

							indices[idx++] = Patch::SIDE_LEN * i;
							indices[idx++] = Patch::SIDE_LEN * (i + 1);
							indices[idx++] = Patch::SIDE_LEN * (Patch::SIDE_LEN + 3) + i + 1;
						}
						return indices;
					}
				};

				static const unsigned int* indices = temp::get_indices32_internal();
				return indices;
			}
#pragma warning (default:4242)
		}

		//Distance in between vertices at maximum subdivision level
		#define PATCH_MULT 16
		//Inverse distance multiplier for camera distance, affects general priorities
		#define DIS_MULT 1

		//Returns whether the patch should be subdivided
		inline bool patch_should_subdivide(Patch* p, dvec3& cam_pos)
		{
			//Deal with trivial case first, then do actual calculation
			double dis = (p->pos).distance(cam_pos) - p->side_len;
			return p->side_len >= (Patch::SIDE_LEN - 1) * PATCH_MULT && dis < p->side_len * DIS_MULT;

		}
		//Returns whether the patch should be merged
		inline bool patch_should_merge(Patch* p, dvec3& cam_pos)
		{
			//Deal with trivial case first, then do heavy calculation
			double dis = (p->pos).distance(cam_pos) - p->side_len;
			return dis > p->side_len * DIS_MULT;
		}

		void Patch::gen_data(void)
		{
			//Interpolation constant
			double INTERP = 1. / (SIDE_LEN - 1);

			//Avoid using a atomic value before the array is initialized completely, use temporary instead
			Vertex* mesh_data_ptr = new Vertex[NUM_VERTICES];
			//Initialize to 0
			memset(mesh_data_ptr, 0, sizeof(Vertex) * NUM_VERTICES);

			for (size_t x = 0; x < SIDE_LEN; x++)
			{
				//Calcualte horizontal position
				double interp = INTERP * (double)x;
				dvec3 v1 = nwc.lerp(nec, interp);
				dvec3 v2 = swc.lerp(sec, interp);
				for (size_t y = 0; y < SIDE_LEN; y++)
				{
					//Calculate vertical position
					dvec3 vtx = v1.lerp(v2, INTERP * (double)y);
					dvec3 nvtx = vtx.normalized();
					//Map to sphere
					vtx = nvtx * planet_radius;
					//Assign vertex position
					mesh_data_ptr[x * SIDE_LEN + y].vertex = (fvec3)(vtx - pos);
					//Texcoord is normal as well, data compactness
					mesh_data_ptr[x * SIDE_LEN + y].texcoord = (fvec3)nvtx;
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
			size_t data_size = SIDE_LEN * SIDE_LEN;
			for (size_t i = 0; i < SIDE_LEN; i++)
			{
				
				vnrm = nwc.lerp(swc, INTERP * (double)i).normalized();
				mesh_data_ptr[data_size + i].vertex = (fvec3)((vnrm * planet_radius - vnrm * SKIRT_DEPTH) - pos);
				mesh_data_ptr[data_size + i].texcoord = (fvec3)vnrm;
			}
			data_size += SIDE_LEN;
			for (size_t i = 0; i < SIDE_LEN; i++)
			{
				vnrm = swc.lerp(sec, INTERP * (double)i).normalized();
				mesh_data_ptr[data_size + i].vertex = (fvec3)((vnrm * planet_radius - vnrm * SKIRT_DEPTH) - pos);
				mesh_data_ptr[data_size + i].texcoord = (fvec3)vnrm;
			}
			data_size += SIDE_LEN;
			for (size_t i = 0; i < SIDE_LEN; i++)
			{
				vnrm = nec.lerp(sec, INTERP * (double)i).normalized();
				mesh_data_ptr[data_size + i].vertex = (fvec3)((vnrm * planet_radius - vnrm * SKIRT_DEPTH) - pos);
				mesh_data_ptr[data_size + i].texcoord = (fvec3)vnrm;
			}
			data_size += SIDE_LEN;
			for (size_t i = 0; i < SIDE_LEN; i++)
			{
				vnrm = nwc.lerp(nec, INTERP * (double)i).normalized();
				mesh_data_ptr[data_size + i].vertex = (fvec3)((vnrm * planet_radius - vnrm * SKIRT_DEPTH) - pos);
				mesh_data_ptr[data_size + i].texcoord = (fvec3)vnrm;
			}
			
			//Store in atomic pointer
			delete mesh_data.exchange(mesh_data_ptr);
		}

		bool force_sub_r(std::shared_ptr<Patch> p, dvec3& cam_pos)
		{
			if (p == nullptr)
				return false;
			if (p->is_subdivided())
			{
				if (patch_should_merge(p.get(), cam_pos))
				{
					p->merge_children();
					return true;
				}
				else
				{
					bool r1 = force_sub_r(p->nw, cam_pos);
					bool r2 = force_sub_r(p->ne, cam_pos);
					bool r3 = force_sub_r(p->sw, cam_pos);
					bool r4 = force_sub_r(p->se, cam_pos);

					return r1 || r2 || r3 || r4;
				}
			}
			else if (patch_should_subdivide(p.get(), cam_pos))
			{
				p->split();

				force_sub_r(p->nw, cam_pos);
				force_sub_r(p->ne, cam_pos);
				force_sub_r(p->sw, cam_pos);
				force_sub_r(p->se, cam_pos);

				return true;
			}

			if (p->mesh_data.load() == nullptr)
				p->gen_data();

			return false;
		}

		bool Patch::check_and_subdivide(std::shared_ptr<Patch> p, dvec3& cam_pos)
		{
			//If the patch does not exist
			if (p == nullptr)
				//Don't do anything, quick escape
				return false;
			//If the patch is subdivided
			if (p->is_subdivided())
			{
				//If the patch is far enough away that it should be merged
				if (patch_should_merge(p.get(), cam_pos))
				{
					//Add the patch to the renderer's list to render
					p->executor->add_patch(p);
					//Delete the patch's children
					p->merge_children();
					//Indicate that a change occured
					return true;
				}
				else
				{
					//If the patch should not be merged, continue recursive check
#if 1
					bool r1 = check_and_subdivide(std::atomic_load(&p->nw), cam_pos);
					bool r2 = check_and_subdivide(std::atomic_load(&p->ne), cam_pos);
					bool r3 = check_and_subdivide(std::atomic_load(&p->sw), cam_pos);
					bool r4 = check_and_subdivide(std::atomic_load(&p->se), cam_pos);

					//Return results
					return r1 || r2 || r3 || r4;
#else
					return check_and_subdivide(std::atomic_load(&p->nw), cam_pos)
						|| check_and_subdivide(std::atomic_load(&p->ne), cam_pos)
						|| check_and_subdivide(std::atomic_load(&p->sw), cam_pos)
						|| check_and_subdivide(std::atomic_load(&p->se), cam_pos);
#endif
				}
			}
			//If the patch should be subdivided
			else if (patch_should_subdivide(p.get(), cam_pos))
			{
				//Split but don't generate data
				p->split();
				//Do that on another thread
				//Also put the patch in the remove list
				p->executor->genMeshData(p, std::atomic_load(&p->nw), std::atomic_load(&p->ne), std::atomic_load(&p->sw), std::atomic_load(&p->se));
				//Indicate somthing happened
				return true;
			}
			//Nothing happened
			return false;
		}
		void Patch::force_subdivide(dvec3& cam_pos)
		{
			force_sub_r(nw, cam_pos);
			force_sub_r(ne, cam_pos);
			force_sub_r(sw, cam_pos);
			force_sub_r(se, cam_pos);

			if (mesh_data.load() == nullptr)
				gen_data();
		}

		void Patch::split(void)
		{
			//Split, but don't generate mesh data for children
			dvec3 centre = (nwc + nec + swc + sec) * 0.25;

			std::atomic_store(&nw, std::shared_ptr<Patch>(new Patch(nwc, (nwc + nec) * 0.5, (nwc + swc) * 0.5, centre, planet_radius, side_len * 0.5, executor, this, level + 1)));
			std::atomic_store(&ne, std::shared_ptr<Patch>(new Patch((nwc + nec) * 0.5, nec, centre, (nec + sec) * 0.5, planet_radius, side_len * 0.5, executor, this, level + 1)));
			std::atomic_store(&sw, std::shared_ptr<Patch>(new Patch((nwc + swc) * 0.5, centre, swc, (swc + sec) * 0.5, planet_radius, side_len * 0.5, executor, this, level + 1)));
			std::atomic_store(&se, std::shared_ptr<Patch>(new Patch(centre, (nec + sec) * 0.5, (swc + sec) * 0.5, sec, planet_radius, side_len * 0.5, executor, this, level + 1)));
		}
		void Patch::subdivide(void)
		{
			//Split, and generate mesh data for children
			std::shared_ptr<Patch> nnw, nne, nsw, nse;

			dvec3 centre = (nwc + nec + swc + sec) * 0.25;

			//Create children
			nnw = std::shared_ptr<Patch>(new Patch(nwc, (nwc + nec) * 0.5, (nwc + swc) * 0.5, centre, planet_radius, side_len * 0.5, executor, this, level + 1));
			nne = std::shared_ptr<Patch>(new Patch((nwc + nec) * 0.5, nec, centre, (nec + sec) * 0.5, planet_radius, side_len * 0.5, executor, this, level + 1));
			nsw = std::shared_ptr<Patch>(new Patch((nwc + swc) * 0.5, centre, swc, (swc + sec) * 0.5, planet_radius, side_len * 0.5, executor, this, level + 1));
			nse = std::shared_ptr<Patch>(new Patch(centre, (nec + sec) * 0.5, (swc + sec) * 0.5, sec, planet_radius, side_len * 0.5, executor, this, level + 1));

			//Generate mesh data
			nnw->gen_data();
			nne->gen_data();
			nsw->gen_data();
			nse->gen_data();

			//Store new pointers
			std::atomic_store(&nw, nnw);
			std::atomic_store(&ne, nne);
			std::atomic_store(&sw, nsw);
			std::atomic_store(&se, nse);
		}
		void Patch::merge_children(void)
		{
			//Delete children, let smart pointer take care of the rest
			std::atomic_store(&nw, std::shared_ptr<Patch>(nullptr));
			std::atomic_store(&ne, std::shared_ptr<Patch>(nullptr));
			std::atomic_store(&sw, std::shared_ptr<Patch>(nullptr));
			std::atomic_store(&se, std::shared_ptr<Patch>(nullptr));
		}

		Patch::Patch(dvec3 nwc, dvec3 nec, dvec3 swc, dvec3 sec, double pl_r, double side_len, Renderer* sched, Patch* parent, unsigned int level)
			:
			parent(parent),
			g_id(0),
			mesh_data((Patch::Vertex*)0)
		{		
			this->nwc = (nwc);
			this->nec = (nec);
			this->swc = (swc);
			this->sec = (sec);
			this->nw = (decltype(nw))(nullptr);
			this->ne = (decltype(ne))(nullptr);
			this->sw = (decltype(sw))(nullptr);
			this->se = (decltype(se))(nullptr);
			this->pos = (((nwc + nec + swc + sec) * 0.25).normalized() * pl_r);
			this->level = (level);
			this->planet_radius = (pl_r);
			this->executor = (sched);
			this->side_len = (side_len);

		}
		Patch::~Patch(void)
		{
			//Delete vertex buffer associated with this mesh
			executor->delete_mesh(g_id);

			//Delete mesh data
			delete mesh_data.load();
		}
	}
}