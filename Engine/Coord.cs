using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vec3d = OpenTK.Vector3d;
using Quatd = OpenTK.Quaterniond;

namespace EngineSystem
{
    public class Coord
    {
        public Vec3d Position;
        public Quatd Rotation;

        public Coord(Vec3d pos, Quatd rot)
        {
            Position = pos;
            Rotation = rot;
        }
    }
}
