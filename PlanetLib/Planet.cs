using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngineSystem;
using RenderSystem;
using OpenTK;
using Shader;
using OpenTK.Graphics.OpenGL;
using Shaders;

namespace PlanetLib
{
    /// <summary>
    /// Component that manages a planet.
    /// This makes the entity that the component was attached to a planet.
    /// </summary>
    public class PlanetComponent : GraphicsComponent
    {
        /// <summary>
        /// The entity this component is attached to.
        /// </summary>
        public Entity ManagedEntity;
        /// <summary>
        /// The mesh of the planet.
        /// </summary>
        public PlanetMesh Mesh;

        private Executor Executor;
        private double Radius;
        private float MaxDeform;
        private Texture ColourTexture;
        private Texture BumpTexture;
        private Texture NormalTexture;

        /// <summary>
        /// Returns the ID for this component.
        /// </summary>
        /// <returns> 60. </returns>
        public override uint GetID()
        {
            return 60;
        }

        /// <summary>
        /// Callback for when the component is attached to an entity.
        /// </summary>
        /// <param name="e"> The entity the component is attached to. </param>
        public override void OnCreate(Entity e)
        {
            ManagedEntity = e;
        }
        
        /// <summary>
        /// Callback for when the component is removed from it's parent entity.
        /// </summary>
        /// <param name="e"> The entity that the PlanetComponent is being removed from. </param>
        public override void OnRemove(Entity e)
        {
            ManagedEntity = null;
        }

        /// <summary>
        /// Instantiates the mesh and planet renderer.
        /// </summary>
        /// <param name="Sys"></param>
        protected override void OnRenderAdd(GraphicsSystem Sys)
        {
            PlanetSurfaceShader Shader = new PlanetSurfaceShader();
                        
            Shader.uniform_ColourTexture = ColourTexture;
            Shader.uniform_BumpMap =  BumpTexture;
            Shader.uniform_NormalTexture = NormalTexture;
            Shader.uniform_MaxDeform = MaxDeform;

            Sys.GraphicsThread.ScheduleEssentialRenderTask(new Action(delegate { Shader.Compile(); }));

            Executor = new Executor(Sys, Shader);

            Mesh = new PlanetMesh(Radius, Executor);
        }

        /// <summary>
        /// Returns a function that will be executed on the render thread.
        /// </summary>
        /// <returns></returns>
        public override Action GetDrawAction()
        {
            ICamera Cam = ParentSystem.Camera;

            Coord PlanetTrans = ManagedEntity.Transform;
            Vector3d CamPos = Cam.GetTransform().Position;

            Matrix4d CamMat = Cam.GetProjMat() * Matrix4d.Rotate(Cam.GetTransform().Rotation).Inverted() * Matrix4d.CreateTranslation(CamPos).Inverted();

            return new Action(delegate
            {
                Executor.DrawPlanet(PlanetTrans, CamMat, CamPos, new Vector3d(0, 0, 1));
            });
        }

        /// <summary>
        /// Creates a planet.
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="max_deform"></param>
        /// <param name="colour_texture"></param>
        /// <param name="bump_texture"></param>
        /// <param name="normal_texture"></param>
        public PlanetComponent(double radius, float max_deform, Texture colour_texture, Texture bump_texture, Texture normal_texture)
        {
            Radius = radius;
            MaxDeform = (float)max_deform;
            ColourTexture = colour_texture;
            BumpTexture = bump_texture;
            NormalTexture = normal_texture;
        }
    }
}
