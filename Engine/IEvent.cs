using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineSystem.Messaging
{
    public interface IEvent
    {
        uint GetID();
    }
}
