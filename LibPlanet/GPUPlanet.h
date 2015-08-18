#ifndef LIBLOD3D_GPU_PLANET_H
#define LIBLOD3D_GPU_PLANET_H

#include "GPUPatch.h"
#include "Math.h"

namespace lod3d
{
	namespace gpu
	{
		class Planet
		{
		private:
			std::shared_ptr<Patch> sides0;
			std::shared_ptr<Patch> sides1;
			std::shared_ptr<Patch> sides2;
			std::shared_ptr<Patch> sides3;
			std::shared_ptr<Patch> sides4;
			std::shared_ptr<Patch> sides5;

		public:

			dcoord transform;
			double radius;

			Planet(dcoord transform, double radius, Renderer* thread);
			
			void check_and_subdivide(dvec3 cam_pos);
			void subdivide_complete(dvec3 cam_pos);
		};
	}
}

#endif
