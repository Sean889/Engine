using Quatd = OpenTK.Quaterniond;
using Vec3d = OpenTK.Vector3d;

namespace EngineSystem
{
    /// <summary>
    /// A transform with position and rotation
    /// </summary>
    public class Coord
    {
        /// <summary>
        /// The position of the transform
        /// </summary>
        public Vec3d Position;
        /// <summary>
        /// The rotation of the transform
        /// </summary>
        public Quatd Rotation;
        
        /// <summary>
        /// Creates the Coord with the given position and rotation
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        public Coord(Vec3d pos, Quatd rot)
        {
            Position = pos;
            Rotation = rot;
        }
    }
}
