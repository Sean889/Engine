using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibPlanet
{
    public class Patch
    {
        internal const int SIDE_LEN = 33;
        internal const int NUM_VERTICES = (SIDE_LEN * SIDE_LEN) + SIDE_LEN * 4;
        internal const int NUM_INDICES = (SIDE_LEN - 1) * (SIDE_LEN - 1) * 6 + 24 * (SIDE_LEN - 1);

        static readonly int SideLen = SIDE_LEN;
        static readonly int Numvertices = NUM_VERTICES;
        static readonly int NumIndices = NUM_INDICES;


    }
}
