#ifndef LIBLOD3D_GPU_TASKEXECUTOR_H
#define LIBLOD3D_GPU_TASKEXECUTOR_H

#include "Math.h"
#include <gcroot.h>

using namespace math;

namespace LibPlanet
{
	public ref struct Camera
	{
	public:
		double aspect;
		double fovy;
		double near_z;
		double far_z;

	internal:
		dvec3 position;
		dquat rotation;

	public:
		property OpenTK::Vector3d Position
		{
			OpenTK::Vector3d get()
			{
				return (OpenTK::Vector3d)position;
			}
			void set(OpenTK::Vector3d value)
			{
				position = dvec3(value);
			}
		}
		property OpenTK::Quaterniond Rotation
		{
			OpenTK::Quaterniond get()
			{
				return (OpenTK::Quaterniond)rotation;
			}
			void set(OpenTK::Quaterniond value)
			{
				rotation = dquat(value);
			}
		}
	};
}

namespace lod3d
{
	using namespace ShaderRuntime;

	namespace gpu
	{
		typedef LibPlanet::Camera Camera;

		class Patch;
		class Planet;

		class Renderer
		{
		private:
			typedef unsigned int GLuint;

			struct draw_data
			{
				dvec3 offset;
				GLuint vbo;

				bool operator ==(GLuint i)
				{
					return vbo == i;
				}

				draw_data(GLuint id, dvec3 pos)
				{
					this->vbo = id;
					this->offset = pos;
				}
				draw_data() { }
			};

			std::vector<draw_data> vbo_ids;	//Vertex buffers (	only accessed from rendering thread. )

			moodycamel::ConcurrentQueue<std::shared_ptr<Patch>> update_queue;	//Buffer creation queue
			moodycamel::ConcurrentQueue<GLuint> delete_queue;					//Buffer deletion queue
			moodycamel::ConcurrentQueue<std::shared_ptr<Patch>> addition_queue;	//Queue to readd patches that have been removed
			moodycamel::ConcurrentQueue<std::shared_ptr<Patch>> remove_queue;	//Buffer removal queue

			moodycamel::ConsumerToken delete_token;		//Internal, for deletion queue
			moodycamel::ConsumerToken update_token;		//Internal, for update queue
			moodycamel::ConsumerToken remove_token;		//Internal, for removal queue
			moodycamel::ConsumerToken addition_token;	//Internal, for addition queue

		public:
			GLuint ibo;			//Index Buffer

			Planet* planet;		//Planet that is being rendered

			gcroot<Camera^> cam;	//Camera attributes

			LIBLOD3D_API Renderer(Camera^ cam);
			LIBLOD3D_API ~Renderer(void);

			//Generates the mesh data on background threads then passes it to the draw thread to create vertex buffers
			LIBLOD3D_API void genMeshData(std::shared_ptr<Patch> parent, std::shared_ptr<Patch> nw, std::shared_ptr<Patch> ne, std::shared_ptr<Patch> sw, std::shared_ptr<Patch> se);

			//Empties all the queues and creates, removes, adds, and deletes vertex buffers
			//ATTENTION: MUST BE CALLED ON THE DRAW THREAD
			LIBLOD3D_API void update(void);
			//Draws all the vertex buffers (i.e. the planet)
			//ATTENTION: MUST BE CALLED ON THE DRAW THREAD
			LIBLOD3D_API void draw(GLShader^ Shader);
			//Removes, but does not delete, the vertex buffer associated with the id
			LIBLOD3D_API void remove_mesh(std::shared_ptr<Patch>);
			//Deletes and removes the vertex buffer associated with the id
			LIBLOD3D_API void delete_mesh(GLuint g_id);
			//Adds a patch with an existing vertex buffer
			LIBLOD3D_API void add_patch(std::shared_ptr<Patch> p);
			//Creates vertex buffer for patch
			LIBLOD3D_API void gen_patch(std::shared_ptr<Patch> p);
			
			//Calls draw, then update
			inline void draw_update(GLShader^ Shader)
			{
				draw(Shader);
				update();
			}

			void _executor(std::shared_ptr<Patch> parent, std::shared_ptr<Patch> nw, std::shared_ptr<Patch> ne, std::shared_ptr<Patch> sw, std::shared_ptr<Patch> se);

		};
	}
}

#endif
