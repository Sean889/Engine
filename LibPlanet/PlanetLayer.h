#pragma once

namespace lod3d
{
	namespace gpu
	{
		class Renderer;
		class Planet;
	}
	
}

namespace LibPlanet
{
	ref struct Camera;
	public ref class PlanetLayer
	{
	private:
		lod3d::gpu::Renderer* renderer;
		lod3d::gpu::Planet* planet;
		Camera^ Cam;

	public:
		ShaderRuntime::GLShader^ Shader;

		PlanetLayer(ShaderRuntime::GLShader^ Shader);
		~PlanetLayer();

		void Init(EngineSystem::Coord^ Transform, double PlanetRadius, Camera^ Cam);

		//Draws the planet.
		//Must be called on the rendering thread.
		void Draw();
		//Updates the surface mesh of the planet.
		void Update();
	};
}