using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShaderRuntime
{
    namespace Utility
    {
        public class Counter
        {
            private uint InternalCount;
            private Action DestructAction;

            public uint Count
            {
                get
                {
                    return InternalCount;
                }
            }
            public Action Destructor
            {
                get
                {
                    return DestructAction;
                }
                set
                {
                    DestructAction = value;
                }
            }

            public Counter(Action Destructor)
            {
                DestructAction = Destructor;
            }

            public void Increment()
            {
                InternalCount++;
            }
            public void Decrement()
            {
                if (--InternalCount == 0)
                    DestructAction();
            }

            public static Counter operator++(Counter var)
            {
                var.Increment();
                return var;
            }
            public static Counter operator--(Counter var)
            {
                var.Decrement();
                return var;
            }
        }
    }
    
}
