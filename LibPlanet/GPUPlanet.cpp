#include "GPUPlanet.h"

namespace lod3d
{
	namespace gpu
	{
		Planet::Planet(dcoord transform, double radius, Renderer* renderer)
		{
			this->radius = (radius);
			this->transform = (transform);
			//Create sides
			sides0 = std::shared_ptr<Patch>(new Patch(dvec3(radius, radius, -radius), dvec3(-radius, radius, -radius), dvec3(radius, radius, radius), dvec3(-radius, radius, radius),		radius, radius * 2, renderer));
			sides1 = std::shared_ptr<Patch>(new Patch(dvec3(-radius, -radius, radius), dvec3(-radius, -radius, -radius), dvec3(radius, -radius, radius), dvec3(radius, -radius, -radius),	radius, radius * 2, renderer));
			sides2 = std::shared_ptr<Patch>(new Patch(dvec3(-radius, -radius, -radius), dvec3(-radius, radius, -radius), dvec3(radius, -radius, -radius), dvec3(radius, radius, -radius),	radius, radius * 2, renderer));
			sides3 = std::shared_ptr<Patch>(new Patch(dvec3(radius, radius, radius), dvec3(-radius, radius, radius), dvec3(radius, -radius, radius), dvec3(-radius, -radius, radius),		radius, radius * 2, renderer));
			sides4 = std::shared_ptr<Patch>(new Patch(dvec3(-radius, radius, radius), dvec3(-radius, radius, -radius), dvec3(-radius, -radius, radius), dvec3(-radius, -radius, -radius), radius, radius * 2, renderer));
			sides5 = std::shared_ptr<Patch>(new Patch(dvec3(radius, -radius, -radius), dvec3(radius, radius, -radius), dvec3(radius, -radius, radius), dvec3(radius, radius, radius),		radius, radius * 2, renderer));

			//Initialize side mesh levels
#define OP(i)  sides##i->gen_data(); sides##i->executor->gen_patch(sides##i)

			OP(0);
			OP(1);
			OP(2);
			OP(3);
			OP(4);
			OP(5);

#undef OP
		}

		void Planet::check_and_subdivide(dvec3 cam_pos)
		{
			dvec3 object_cam_pos = (translate(transform.position).inverse() * rotate(transform.rotation).inverse()) * cam_pos;
			//Run a check and subdivide for each of the mesh levels
#define OP(i) Patch::check_and_subdivide(sides##i, object_cam_pos)

			OP(0);
			OP(1);
			OP(2);
			OP(3);
			OP(4);
			OP(5);

#undef OP
		}
		void Planet::subdivide_complete(dvec3 cam_pos)
		{
			//Run a forced subdivision for each of the sides
#define OP(i) sides##i->force_subdivide(cam_pos)

			OP(0);
			OP(1);
			OP(2);
			OP(3);
			OP(4);
			OP(5);

#undef OP
		}
	}
}