using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineSystem
{
    /// <summary>
    /// Arguments for the update method.
    /// </summary>
    public struct UpdateEventArgs
    {
        public double TimePassed;

        public UpdateEventArgs(double DeltaTime)
        {
            TimePassed = DeltaTime;
        }
    }
}
