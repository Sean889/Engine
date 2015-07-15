using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace Shader
{
    /// <summary>
    /// Represents an OpenGL texture. It does not manage it in any way.
    /// </summary>
    public struct Texture
    {
        /// <summary>
        /// The OpenGL texture id.
        /// </summary>
        public int TextureID;
        /// <summary>
        /// The OpenGL texture target.
        /// </summary>
        public TextureTarget Target;

        /// <summary>
        /// Creates the texture with the given target and OpenGL ID.
        /// </summary>
        /// <param name="Tex"> The OpenGL texture ID. </param>
        /// <param name="Tgt"> The texture target. </param>
        public Texture(int Tex, TextureTarget Tgt)
        {
            TextureID = Tex;
            Target = Tgt;
        }

        /// <summary>
        /// Whether this texture is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return TextureID != 0;
            }
        }
    }
}
