#include "GPURenderer.h"
#include "GPUPatch.h"
#include "GPUPlanet.h"

using namespace System;
using namespace ShaderRuntime;
using namespace OpenTK::Graphics::OpenGL;

typedef unsigned int GLuint;
typedef int GLsizei;
typedef int GLenum;

#define glGenBuffers GL::GenBuffers
#define glBindBuffer GL::BindBuffer
#define glDeleteBuffers GL::DeleteBuffers
#define glBufferData(tgt, size, data, draw) GL::BufferData(tgt, (IntPtr)(void*)(size), (IntPtr)(void*)(data), draw)
#define glDrawElements GL::DrawElements

#define GL_ARRAY_BUFFER BufferTarget::ArrayBuffer
#define GL_ELEMENT_ARRAY_BUFFER BufferTarget::ElementArrayBuffer
#define GL_STATIC_DRAW BufferUsageHint::StaticDraw
#define GL_TRIANGLES PrimitiveType::Triangles

namespace lod3d
{
	namespace gpu
	{
		template<typename _Ty> struct util_type;

#pragma warning (disable:4400)
		template<> struct util_type<unsigned int>
		{
			static const DrawElementsType value = DrawElementsType::UnsignedInt;
		};
		template<> struct util_type<unsigned short>
		{
			static const DrawElementsType value = DrawElementsType::UnsignedShort;
		};
#pragma warning (default:4400)
		
		void __clrcall Executor(std::shared_ptr<Patch> p)
		{
			if (p->mesh_data.load() == nullptr)
			{
				p->gen_data();
			}
			p->executor->gen_patch(p);
		}

		ref struct _Functor
		{
		public:
			std::shared_ptr<Patch>* arg;

			void Invoke()
			{
				Executor(*arg);
				delete arg;
			}

			_Functor(std::shared_ptr<Patch>* a) :
				arg(a)
			{

			}
		};
		ref struct _Functor2
		{
			Renderer* ren;
			std::shared_ptr<Patch>* v0;
			std::shared_ptr<Patch>* v1;
			std::shared_ptr<Patch>* v2;
			std::shared_ptr<Patch>* v3;
			std::shared_ptr<Patch>* v4;

			void Invoke()
			{
				ren->_executor(*v0, *v1, *v2, *v3, *v4);

				delete v0;
				delete v1;
				delete v2;
				delete v3;
				delete v4;
			}

			_Functor2(Renderer* ren, std::shared_ptr<Patch>* v0, std::shared_ptr<Patch>* v1, std::shared_ptr<Patch>* v2, std::shared_ptr<Patch>* v3, std::shared_ptr<Patch>* v4) :
				ren(ren),
				v0(v0),
				v1(v1),
				v2(v2),
				v3(v3),
				v4(v4)
			{

			}
		};

		System::Action^ __clrcall exec_bind(std::shared_ptr<Patch>& p)
		{
			return gcnew System::Action(gcnew _Functor(new std::shared_ptr<Patch>(p)), &_Functor::Invoke);
		}
		System::Action^ __clrcall ren_bind(Renderer* ren, std::shared_ptr<Patch>& v0, std::shared_ptr<Patch>& v1, std::shared_ptr<Patch>& v2, std::shared_ptr<Patch>& v3, std::shared_ptr<Patch>& v4)
		{
			return gcnew System::Action(gcnew _Functor2(ren, new std::shared_ptr<Patch>(v0), new std::shared_ptr<Patch>(v1), new std::shared_ptr<Patch>(v2), new std::shared_ptr<Patch>(v3), new std::shared_ptr<Patch>(v4)), &_Functor2::Invoke);
		}

		void Renderer::_executor(std::shared_ptr<Patch> parent, std::shared_ptr<Patch> nw, std::shared_ptr<Patch> ne, std::shared_ptr<Patch> sw, std::shared_ptr<Patch> se)
		{
			//Generate data
			nw->gen_data();
			ne->gen_data();
			sw->gen_data();
			se->gen_data();

			//Add to the queue for the mesh data to be created
			update_queue.enqueue(nw);
			update_queue.enqueue(ne);
			update_queue.enqueue(sw);
			update_queue.enqueue(se);

			remove_queue.enqueue(parent);
		}

		void Renderer::genMeshData(std::shared_ptr<Patch> parent, std::shared_ptr<Patch> nw, std::shared_ptr<Patch> ne, std::shared_ptr<Patch> sw, std::shared_ptr<Patch> se)
		{
			//Delegate execution to a thread pool
			ThreadPool::ThreadPoolManager::QueueAutoTask(ren_bind(this, parent, nw, ne, sw, se));
		}

		void Renderer::update(void)
		{
			//Meshes that need a vertex buffer to be created
			std::shared_ptr<Patch> patch;
			while (update_queue.try_dequeue(update_token, patch))
			{
				if (patch->g_id == 0u)
				{
					GLuint vbo;
					//Get mesh data
					Patch::Vertex* mdata = patch->mesh_data.load();
					//Create VBO
					glGenBuffers(1, &vbo);
					glBindBuffer(GL_ARRAY_BUFFER, vbo);
					//Send buffer data
					glBufferData(GL_ARRAY_BUFFER, Patch::NUM_VERTICES * sizeof(Patch::Vertex), mdata, GL_STATIC_DRAW);
					//Set id of patch
					patch->g_id = vbo;

					//Create data for drawing
					draw_data data(vbo, patch->pos);

					//Add to rendering list
					vbo_ids.push_back(data);
				}
				else
				{
					addition_queue.enqueue(patch);
				}
			}

			{
				//For items where the id has not been updated yet
				std::vector<std::shared_ptr<Patch>> failed;
				//Pop data off queue if present
				while (addition_queue.try_dequeue(addition_token, patch))
				{
					if (patch->g_id == 0u)
					{
						failed.push_back(patch);
					}
					else if (std::find(vbo_ids.begin(), vbo_ids.end(), patch->g_id) == vbo_ids.end())
					{
						//Add to rendering list
						vbo_ids.push_back(draw_data(patch->g_id, patch->pos));
					}
				}
				size_t size = failed.size();
				for (size_t i = 0; i < size; i++)
				{
					ThreadPool::ThreadPoolManager::QueueAutoTask(exec_bind(failed.at(i)));
				}
			}

			GLuint id;
			//Pop data off queue
			while (delete_queue.try_dequeue(delete_token, id))
			{
				//Find the item with the same id
				std::vector<draw_data>::iterator it = std::find(vbo_ids.begin(), vbo_ids.end(), id);
				//If the item exists in the rendering list
				if (it != vbo_ids.end())
				{
					//Delete and remove from the list
					glDeleteBuffers(1, &it->vbo);
					vbo_ids.erase(it);
				}
			}

			//Get data from removal queue
			while (remove_queue.try_dequeue(remove_token, patch))
			{
				//Find item with same id
				std::vector<draw_data>::iterator it = std::find(vbo_ids.begin(), vbo_ids.end(), patch->g_id);
				//Check if the item exists in the rendering list
				if (it != vbo_ids.end())
				{
					//Remove from the rendering list
					vbo_ids.erase(it);
				}
			}
		}
		void Renderer::draw(GLShader^ Shader)
		{
			//Bind the shader program
			//glUseProgram(program);
			Shader->UseShader();

			//Get number of entries
			size_t size = vbo_ids.size();

			//Create necessary matrices
			dmat4 temp = rotate(planet->transform.rotation);
			dmat4 trans = project(cam->near_z, cam->far_z, cam->fovy, cam->aspect) * rotate(cam->rotation).inverse() * translate(cam->position).inverse() * (scale(planet->transform.scale) * temp * translate(planet->transform.position));
			fmat4 rtm = (fmat4)temp;

			//Bind indices
			glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, ibo);

			/* Done by GLShader
			
			//Setup textures
			glActiveTexture(GL_TEXTURE0);
			glBindTexture(GL_TEXTURE_CUBE_MAP, texture);

			glActiveTexture(GL_TEXTURE1);
			glBindTexture(GL_TEXTURE_CUBE_MAP, heightmap);

			glActiveTexture(GL_TEXTURE2);
			glBindTexture(GL_TEXTURE_CUBE_MAP, normalmap);

			//Enable vertex attributes
			glEnableVertexAttribArray(0);
			glEnableVertexAttribArray(1);*/

			for (size_t i = 0; i < size; i++)
			{
				//Get data for drawing
				draw_data data = vbo_ids.at(i);
				//Calculate final mvp matrix
				temp = trans * translate(data.offset);

				//Bind vertex data
				glBindBuffer(GL_ARRAY_BUFFER, data.vbo);

				//Setup vertex attributes
				GL::VertexAttribPointer(Shader->GetParameterLocation("Vertex"), 3, VertexAttribPointerType::Float, false, sizeof(Patch::Vertex), (IntPtr)(void*)0);
				GL::VertexAttribPointer(Shader->GetParameterLocation("Texcoord"), 3, VertexAttribPointerType::Float, true, sizeof(Patch::Vertex), (IntPtr)(void*)sizeof(fvec3));

				/* Done by GLShader
				//Pass matrices
				glUniformMatrix4fv(0, 1, GL_TRUE, ((fmat4)temp).m);
				glUniformMatrix4fv(4, 1, GL_TRUE, rtm.m);

				//Maximum mesh deformation
				glUniform1f(9, 4096);
				//Texture ids
				glUniform1i(10, 0);
				glUniform1i(11, 1);
				glUniform1i(12, 2);

				//Light direction
				glUniform3f(15, light_dir.x, light_dir.y, light_dir.z);

				//Eye position
				glUniform3fv(20, 1, ((fvec3)cam->position).m);*/

				//Set ModelViewPosition matrix
				Shader->SetParameter("MVP", (OpenTK::Matrix4)temp);
				//Set rotation matrix for light
				Shader->SetParameter("LightRot", (OpenTK::Matrix4)rtm);

				Shader->PassUniforms();

				//Draw call
				glDrawElements(GL_TRIANGLES, Patch::NUM_INDICES, util_type<Patch::vert_type>::value, NULL);
			}
		}

		void Renderer::remove_mesh(std::shared_ptr<Patch> p)
		{
			//Queue a removal operation
			remove_queue.enqueue(p);
		}
		void Renderer::delete_mesh(GLuint id)
		{
			//Enqueue a delete operation
			delete_queue.enqueue(id);
		}
		void Renderer::add_patch(std::shared_ptr<Patch> p)
		{
			//Enqueue an data addition operation
			addition_queue.enqueue(p);
		}
		void Renderer::gen_patch(std::shared_ptr<Patch> p)
		{
			//Add to buffer creation queue
			update_queue.enqueue(p);
		}

		Renderer::Renderer(Camera^ cam) :
			cam(cam),
			update_token(update_queue),
			delete_token(delete_queue),
			remove_token(remove_queue),
			addition_token(addition_queue)
		{
			//Create index buffer
			glGenBuffers(1, &ibo);
			glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, ibo);
			const Patch::vert_type* indices = Patch::get_indices();
			glBufferData(GL_ELEMENT_ARRAY_BUFFER, Patch::NUM_INDICES * sizeof(Patch::vert_type), indices, GL_STATIC_DRAW);
		}
		Renderer::~Renderer(void)
		{
			//Delete index buffer
			glDeleteBuffers(1, &ibo);

			//Delete leftover vertex buffers
			size_t size = vbo_ids.size();
			for (size_t i = 0; i < size; i++)
			{
				glDeleteBuffers(1, &vbo_ids.at(i).vbo);
			}
		}
	}
}