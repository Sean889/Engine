using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using Shader;

namespace PlanetLib
{
    internal struct TexData<T> where T : struct
    {
        internal T[,] Top;
        internal T[,] Bottom;
        internal T[,] Left;
        internal T[,] Right;
        internal T[,] Front;
        internal T[,] Back;

        internal void Reset()
        {
            Top = Bottom = Left = Right = Front = Back = null;
        }
        internal void Init(int Size)
        {
            Top = new T[Size, Size];
            Bottom = new T[Size, Size];
            Left = new T[Size, Size];
            Right = new T[Size, Size];
            Front = new T[Size, Size];
            Back = new T[Size, Size];
        }
    }

#pragma warning disable 1591

    public class PlanetTexture
    {
        public Texture HeightTexture;
        public Texture ColourTexture;

        /// <summary>
        /// Alias for ColourTexture.
        /// </summary>
        public Texture ColorTexture
        {
            get
            {
                return ColourTexture;
            }
            set
            {
                ColourTexture = value;
            }
        }

        public void UploadTexture()
        {
            int Tex = GL.GenTexture();

            GL.BindTexture(TextureTarget.TextureCubeMap, Tex);

            GL.TexImage2D(TextureTarget.TextureCubeMapPositiveY, 0, PixelInternalFormat.R32f, SideLen, SideLen, 0, PixelFormat.Red, PixelType.Float, Height.Top);
            GL.TexImage2D(TextureTarget.TextureCubeMapNegativeY, 0, PixelInternalFormat.R32f, SideLen, SideLen, 0, PixelFormat.Red, PixelType.Float, Height.Bottom);
            GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX, 0, PixelInternalFormat.R32f, SideLen, SideLen, 0, PixelFormat.Red, PixelType.Float, Height.Right);
            GL.TexImage2D(TextureTarget.TextureCubeMapNegativeX, 0, PixelInternalFormat.R32f, SideLen, SideLen, 0, PixelFormat.Red, PixelType.Float, Height.Left);
            GL.TexImage2D(TextureTarget.TextureCubeMapPositiveZ, 0, PixelInternalFormat.R32f, SideLen, SideLen, 0, PixelFormat.Red, PixelType.Float, Height.Front);
            GL.TexImage2D(TextureTarget.TextureCubeMapNegativeZ, 0, PixelInternalFormat.R32f, SideLen, SideLen, 0, PixelFormat.Red, PixelType.Float, Height.Back);

            HeightTexture = new Texture(Tex, TextureTarget.TextureCubeMap);

            Tex = GL.GenTexture();

            GL.BindTexture(TextureTarget.TextureCubeMap, Tex);

            GL.TexImage2D(TextureTarget.TextureCubeMapPositiveY, 0, PixelInternalFormat.Rgb32f, SideLen, SideLen, 0, PixelFormat.Rgb, PixelType.Float, Colour.Top);
            GL.TexImage2D(TextureTarget.TextureCubeMapNegativeY, 0, PixelInternalFormat.Rgb32f, SideLen, SideLen, 0, PixelFormat.Rgb, PixelType.Float, Colour.Bottom);
            GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX, 0, PixelInternalFormat.Rgb32f, SideLen, SideLen, 0, PixelFormat.Rgb, PixelType.Float, Colour.Right);
            GL.TexImage2D(TextureTarget.TextureCubeMapNegativeX, 0, PixelInternalFormat.Rgb32f, SideLen, SideLen, 0, PixelFormat.Rgb, PixelType.Float, Colour.Left);
            GL.TexImage2D(TextureTarget.TextureCubeMapPositiveZ, 0, PixelInternalFormat.Rgb32f, SideLen, SideLen, 0, PixelFormat.Rgb, PixelType.Float, Colour.Front);
            GL.TexImage2D(TextureTarget.TextureCubeMapNegativeZ, 0, PixelInternalFormat.Rgb32f, SideLen, SideLen, 0, PixelFormat.Rgb, PixelType.Float, Colour.Back);

            ColourTexture = new Texture(Tex, TextureTarget.TextureCubeMap);

            Height.Reset();
            Colour.Reset();
        }

        internal TexData<float> Height;
        internal TexData<Vector3> Colour;
        internal int SideLen;

        internal PlanetTexture(int Size)
        {
            SideLen = Size;

            Height.Init(SideLen);
            Colour.Init(SideLen);
        }
    }

    /*
     *            ______
     *           |      |
     *           | Top  |
     *    _______|______|______ ______
     *   |       |      |      |      |
     *   | Left  |Front |Right | Back |
     *   |_______|______|______|______|
     *           |      |
     *           |Bottom|
     *           |______|
     */

    public class PlanetGenerator
    {
        private static Vector3 ToSphere(Vector3 vPosition)
        {

#if CUBE_TO_SPHERE_FAST

#else
            double x2 = vPosition.X * vPosition.X; 
            double y2 = vPosition.Y * vPosition.Y; 
            double z2 = vPosition.Z * vPosition.Z;

            return new Vector3(
                vPosition.X * (float)Math.Sqrt(1.0f - (y2 * 0.5f) - (z2 * 0.5f) + ((y2 * z2) / 3.0f)),
                vPosition.Y * (float)Math.Sqrt(1.0f - (z2 * 0.5f) - (x2 * 0.5f) + ((z2 * x2) / 3.0f)),
                vPosition.Z * (float)Math.Sqrt(1.0f - (x2 * 0.5f) - (y2 * 0.5f) + ((x2 * y2) / 3.0f)));
#endif
        }

        public static PlanetTexture GenerateTexture(Func<Vector3, float> HeightFunc, Func<float, Vector3, Vector3> ColourFunc, int TexSize)
        {
            PlanetTexture tex = new PlanetTexture(TexSize);
            float Interp = 1.0f / (float)TexSize;

            // Generate Height Texture and Colour Texture
            {
                // Top Right, Top Left, Bottom Right, Bottom Left
                Vector3 TR, TL, BR, BL;

                //Top Side
                TR = new Vector3(1, 1, 1);
                TL = new Vector3(-1, 1, -1);
                BR = new Vector3(1, 1, -1);
                BL = new Vector3(-1, 1, -1);

                for(int x = 0; x < TexSize; x++)
                {
                    //Interpolated Top and Bottom positions
                    Vector3 T, B;

                    T = Vector3.Lerp(TL, TR, x * Interp);
                    B = Vector3.Lerp(BL, BR, x * Interp);

                    for(int y = 0; y < TexSize; y++)
                    {
                        Vector3 Pos = ToSphere(Vector3.Lerp(T, B, y * Interp));

                        float HeightValue = HeightFunc(Pos);
                        Vector3 ColourValue = ColourFunc(HeightValue, Pos);

                        tex.Height.Top[x, y] = HeightValue;
                        tex.Colour.Top[x, y] = ColourValue;
                    }
                }

                //Bottom side
                TR = new Vector3(-1, -1, 1);
                TL = new Vector3(-1, -1, -1);
                BR = new Vector3(1, -1, 1);
                BL = new Vector3(-1, -1, 1);

                for (int x = 0; x < TexSize; x++)
                {
                    //Interpolated Top and Bottom positions
                    Vector3 T, B;

                    T = Vector3.Lerp(TL, TR, x * Interp);
                    B = Vector3.Lerp(BL, BR, x * Interp);

                    for (int y = 0; y < TexSize; y++)
                    {
                        Vector3 Pos = ToSphere(Vector3.Lerp(T, B, y * Interp));

                        float HeightValue = HeightFunc(Pos);
                        Vector3 ColourValue = ColourFunc(HeightValue, Pos);

                        tex.Height.Bottom[x, y] = HeightValue;
                        tex.Colour.Bottom[x, y] = ColourValue;
                    }
                }

                //Left side
                TR = new Vector3(1, 1, -1);
                TL = new Vector3(1, 1, 1);
                BR = new Vector3(1, -1, -1);
                BL = new Vector3(1, -1, 1);

                for (int x = 0; x < TexSize; x++)
                {
                    //Interpolated Top and Bottom positions
                    Vector3 T, B;

                    T = Vector3.Lerp(TL, TR, x * Interp);
                    B = Vector3.Lerp(BL, BR, x * Interp);

                    for (int y = 0; y < TexSize; y++)
                    {
                        Vector3 Pos = ToSphere(Vector3.Lerp(T, B, y * Interp));

                        float HeightValue = HeightFunc(Pos);
                        Vector3 ColourValue = ColourFunc(HeightValue, Pos);

                        tex.Height.Left[x, y] = HeightValue;
                        tex.Colour.Left[x, y] = ColourValue;
                    }
                }
                
                //Right side
                TR = new Vector3(1, 1, -1);
                TL = new Vector3(1, 1, 1);
                BR = new Vector3(1, -1, -1);
                BL = new Vector3(1, -1, 1);

                for (int x = 0; x < TexSize; x++)
                {
                    //Interpolated Top and Bottom positions
                    Vector3 T, B;

                    T = Vector3.Lerp(TL, TR, x * Interp);
                    B = Vector3.Lerp(BL, BR, x * Interp);

                    for (int y = 0; y < TexSize; y++)
                    {
                        Vector3 Pos = ToSphere(Vector3.Lerp(T, B, y * Interp));

                        float HeightValue = HeightFunc(Pos);
                        Vector3 ColourValue = ColourFunc(HeightValue, Pos);

                        tex.Height.Right[x, y] = HeightValue;
                        tex.Colour.Right[x, y] = ColourValue;
                    }
                }

                //Front Side
                TR = new Vector3(-1, 1, -1);
                TL = new Vector3(1, 1, -1);
                BR = new Vector3(-1, -1, -1);
                BL = new Vector3(1, -1, -1);

                for (int x = 0; x < TexSize; x++)
                {
                    //Interpolated Top and Bottom positions
                    Vector3 T, B;

                    T = Vector3.Lerp(TL, TR, x * Interp);
                    B = Vector3.Lerp(BL, BR, x * Interp);

                    for (int y = 0; y < TexSize; y++)
                    {
                        Vector3 Pos = ToSphere(Vector3.Lerp(T, B, y * Interp));

                        float HeightValue = HeightFunc(Pos);
                        Vector3 ColourValue = ColourFunc(HeightValue, Pos);

                        tex.Height.Front[x, y] = HeightValue;
                        tex.Colour.Front[x, y] = ColourValue;
                    }
                }
                
                //Back Side
                TL = new Vector3(1, 1, 1);
                TR = new Vector3(-1, 1, 1);
                BL = new Vector3(1, -1, 1);
                BR = new Vector3(-1, -1, 1);

                for (int x = 0; x < TexSize; x++)
                {
                    //Interpolated Top and Bottom positions
                    Vector3 T, B;

                    T = Vector3.Lerp(TL, TR, x * Interp);
                    B = Vector3.Lerp(BL, BR, x * Interp);

                    for (int y = 0; y < TexSize; y++)
                    {
                        Vector3 Pos = ToSphere(Vector3.Lerp(T, B, y * Interp));

                        float HeightValue = HeightFunc(Pos);
                        Vector3 ColourValue = ColourFunc(HeightValue, Pos);

                        tex.Height.Back[x, y] = HeightValue;
                        tex.Colour.Back[x, y] = ColourValue;
                    }
                }
            }

            return tex;
        }
    }
}
