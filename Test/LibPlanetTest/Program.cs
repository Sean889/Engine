using System;
using System.Collections.Generic;
using EngineSystem;
using OpenTK;
using ShaderRuntime;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using EngineSystem.Messaging;
using LibPlanet;

namespace LibPlanetTest
{
    class CubemapData
    {
        public Bitmap[] Sides;

        public IEnumerable<BitmapData> GetDatas()
        {
            for(int i = 0; i < Sides.Length; i++)
            {
                BitmapData Data;
                Bitmap Map = Sides[i];
                Data = Map.LockBits(new Rectangle(0, 0, Map.Width, Map.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                yield return Data;
                Map.UnlockBits(Data);
                Sides[i] = null;
            }
        }

        public CubemapData(params string[] paths)
        {
            Sides = new Bitmap[paths.Length];

            for(int i = 0; i < paths.Length; i++)
            {
                Sides[i] = new Bitmap(paths[i]);
            }
        }
    }

    class Program
    {
        static void OnWindowClosed(object Sender, IEvent e)
        {
            if(e.GetID() == WindowClosingEvent.Id)
            {
                WindowClosingEvent Event = e as WindowClosingEvent;
            }
        }
        
        static readonly string BumpPath = "D:\\Programs\\Images\\1\\surface_bump_";
        static readonly string NormalPath = "D:\\Programs\\Images\\1\\surface_norm_";
        static readonly string ColourPath = "D:\\Programs\\Images\\1\\surface_diff_";
        static readonly TextureTarget[] Targets = 
            { 
                TextureTarget.TextureCubeMapPositiveX, 
                TextureTarget.TextureCubeMapNegativeX,
                TextureTarget.TextureCubeMapPositiveY,
                TextureTarget.TextureCubeMapNegativeY,
                TextureTarget.TextureCubeMapPositiveZ,
                TextureTarget.TextureCubeMapNegativeZ
            };

        static GLShader CreateShader()
        {
            Texture Colour = new Texture();
            Texture Normal = new Texture();
            Texture Bump = new Texture();

            Colour.Target = TextureTarget.TextureCubeMap;
            Colour.TextureID = GL.GenTexture();
            GL.BindTexture(Colour.Target, Colour.TextureID);

            Shaders.PlanetSurfaceShader Shader = new Shaders.PlanetSurfaceShader();

            Shader.uniform_colour_texture = Colour;
            Shader.uniform_heightmap = Bump;
            Shader.uniform_normal_texture = Normal;

            Shader.uniform_max_height = 2048;
            Shader.uniform_light_dir = new OpenTK.Vector3(0, 1, 0);

            Shader.Compile();

            int i = 0;
            CubemapData ColourData = new CubemapData(
                ColourPath + "pos_x.png",
                ColourPath + "neg_x.png",
                ColourPath + "pos_y.png",
                ColourPath + "neg_y.png",
                ColourPath + "pos_z.png",
                ColourPath + "neg_z.png");
            foreach(BitmapData Data in ColourData.GetDatas())
            {
                GC.AddMemoryPressure(Data.Height * Data.Width * sizeof(float) * 4);
                GL.TexImage2D(Targets[i++], 0, PixelInternalFormat.Rgba, Data.Width, Data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, Data.Scan0);
            }

            Normal.Target = TextureTarget.TextureCubeMap;
            Normal.TextureID = GL.GenTexture();
            GL.BindTexture(Normal.Target, Normal.TextureID);

            GC.Collect();

            i = 0;
            CubemapData NormalData = new CubemapData(
                NormalPath + "pos_x.png",
                NormalPath + "neg_x.png",
                NormalPath + "pos_y.png",
                NormalPath + "neg_y.png",
                NormalPath + "pos_z.png",
                NormalPath + "neg_z.png");
            foreach(BitmapData Data in NormalData.GetDatas())
            {
                GC.AddMemoryPressure(Data.Height * Data.Width * sizeof(float) * 3);
                GL.TexImage2D(Targets[i++], 0, PixelInternalFormat.Rgb, Data.Width, Data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, Data.Scan0);
            }

            Bump.Target = TextureTarget.TextureCubeMap;
            Bump.TextureID = GL.GenTexture();
            GL.BindTexture(Bump.Target, Bump.TextureID);

            GC.Collect();

            i = 0;
            CubemapData BumpData = new CubemapData(
                BumpPath + "pos_x.png",
                BumpPath + "neg_x.png",
                BumpPath + "pos_y.png",
                BumpPath + "neg_y.png",
                BumpPath + "pos_z.png",
                BumpPath + "neg_z.png");
            foreach (BitmapData Data in BumpData.GetDatas())
            {
                GC.AddMemoryPressure(Data.Height * Data.Width * sizeof(float));
                GL.TexImage2D(Targets[i++], 0, PixelInternalFormat.R32f, Data.Width, Data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, Data.Scan0);
            }

            return Shader;
        }

        static void Main(string[] args)
        {
            GameWindow Win = new GameWindow(1080, 720, OpenTK.Graphics.GraphicsMode.Default, "Test Window", GameWindowFlags.Default, DisplayDevice.Default, 4, 3, OpenTK.Graphics.GraphicsContextFlags.Debug);

            using (Win)
            {
                Win.MakeCurrent();

                Win.Visible = true;
                PlanetLayer Layer = new PlanetLayer(CreateShader());

            LibPlanet.Camera Cam = new Camera();

                Cam.aspect = 4.0/3.0;
                Cam.far_z = 1000;
                Cam.near_z = 1;
                Cam.fovy = 60;

                Cam.Position = new Vector3d(0.0, 0.0, -200);

                Layer.Init(new Coord(new Vector3d(0, 0, 0), new Quaterniond(0, 0, 0, 1)), 100, Cam);

                Win.RenderFrame += (sender, e) =>
                    {
                        Layer.Draw();
                    };

                Win.UpdateFrame += (sender, e) =>
                    {
                        Layer.Update();
                    };

                Win.Run();

                Win.Closing += (sender, e) =>
                    {
                        Layer.Dispose();
                    };
            }
        }
    }
}
