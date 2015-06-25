using Quatd = OpenTK.Quaterniond;
using Vec3d = OpenTK.Vector3d;

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
