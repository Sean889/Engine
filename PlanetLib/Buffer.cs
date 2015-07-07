using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace PlanetLib
{
    internal sealed class Buffer
    {
        internal volatile int VboID;
        internal Vector3d Offset;
        internal Vector3[,] Verts;
        
        internal Buffer(Vector3[,] Verts, Vector3d Offset)
        {
            VboID = 0;
            this.Verts = Verts;
            this.Offset = Offset;
        }
    }
}
