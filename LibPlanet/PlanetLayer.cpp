#include "PlanetLayer.h"

#include "GPUPlanet.h"
#include "GPURenderer.h"

using namespace lod3d;
using namespace lod3d::gpu;
using namespace math;
using namespace ShaderRuntime;

namespace LibPlanet
{
	PlanetLayer::PlanetLayer(ShaderRuntime::GLShader^ Shader) :
		Shader(Shader),
		planet(nullptr),
		renderer(nullptr)
	{
	}
	PlanetLayer::~PlanetLayer()
	{
		delete renderer;
		delete planet;
	}

	void PlanetLayer::Init(EngineSystem::Coord^ trans, double radius, Camera^ Cam)
	{
		renderer = new Renderer(Cam);
		planet = new Planet(dcoord(trans), radius, renderer);
		this->Cam = Cam;

		renderer->planet = planet;
	}

	void PlanetLayer::Draw()
	{
		renderer->draw_update(Shader);
	}
	void PlanetLayer::Update()
	{
		planet->check_and_subdivide(Cam->position);
	}
}