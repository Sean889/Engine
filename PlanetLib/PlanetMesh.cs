using OpenTK;
using System.Collections.Generic;

namespace PlanetLib
{
    /// <summary>
    /// Spherified cube deformed as a planet.
    /// </summary>
    public class PlanetMesh : IEnumerable<Patch>
    {
        /// <summary>
        /// The sides of the base cube.
        /// </summary>
        public readonly Patch[] Sides = new Patch[6];
        /// <summary>
        /// The radius of the planet.
        /// </summary>
        public readonly double Radius;
        
        internal PlanetMesh(double radius, Executor exec)
        {
            Radius = radius;

            Sides[0] = new Patch(new Vector3d(radius, radius, -radius), new Vector3d(-radius, radius, -radius), new Vector3d(radius, radius, radius), new Vector3d(-radius, radius, radius), radius, radius * 2, exec, null, 0);
            Sides[1] = new Patch(new Vector3d(-radius, -radius, radius), new Vector3d(-radius, -radius, -radius), new Vector3d(radius, -radius, radius), new Vector3d(radius, -radius, -radius), radius, radius * 2, exec, null, 0);
            Sides[2] = new Patch(new Vector3d(-radius, -radius, -radius), new Vector3d(-radius, radius, -radius), new Vector3d(radius, -radius, -radius), new Vector3d(radius, radius, -radius), radius, radius * 2, exec, null, 0);
            Sides[3] = new Patch(new Vector3d(radius, radius, radius), new Vector3d(-radius, radius, radius), new Vector3d(radius, -radius, radius), new Vector3d(-radius, -radius, radius), radius, radius * 2, exec, null, 0);
            Sides[4] = new Patch(new Vector3d(-radius, radius, radius), new Vector3d(-radius, radius, -radius), new Vector3d(-radius, -radius, radius), new Vector3d(-radius, -radius, -radius), radius, radius * 2, exec, null, 0);
            Sides[5] = new Patch(new Vector3d(radius, -radius, -radius), new Vector3d(radius, radius, -radius), new Vector3d(radius, -radius, radius), new Vector3d(radius, radius, radius), radius, radius * 2, exec, null, 0);

            foreach(Patch side in Sides)
            {
                side.GenData();
                exec.UploadPatch(side);
            }
        }

        /// <summary>
        /// Subdivides a patch if it is too close and merges a patch if it is too far away.
        /// </summary>
        /// <param name="CamPos"></param>
        /// <returns></returns>
        public bool CheckAndSubdivide(Vector3d CamPos)
        {
            bool ret = false;

            foreach(Patch side in Sides)
            {
                ret = side.CheckAndSubdivide(CamPos) || ret;
            }

            return ret;
        }
        /// <summary>
        /// Iterates over the sides of the mesh.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Patch> GetEnumerator()
        {
 	        yield return Sides[0];
            yield return Sides[1];
            yield return Sides[2];
            yield return Sides[3];
            yield return Sides[4];
            yield return Sides[5];
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
 	        yield return Sides[0];
            yield return Sides[1];
            yield return Sides[2];
            yield return Sides[3];
            yield return Sides[4];
            yield return Sides[5];
        }
    }
}
