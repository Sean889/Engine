using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngineSystem;

namespace PlanetLib
{
    class PlanetSystem : ISystem
    {
        private static PlanetSystem InternalSystem;
        private static volatile bool Initialized = false;

        public static PlanetSystem CurrentSystem
        {
            get
            {
                return InternalSystem;
            }
            private set
            {
                InternalSystem = value;
            }
        }

        private static void CreateSystem()
        {
            CurrentSystem = new PlanetSystem();
            Engine.CurrentEngine.AddSystem(CurrentSystem);

            Initialized = true;
        }

        public static void AddPlanet(IPlanet Component)
        {
            if (!Initialized)
                CreateSystem();
            CurrentSystem.NumPlanets++;
            CurrentSystem.ToAdd.Enqueue(Component);
        }
        public static void RemovePlanet(IPlanet Component)
        {
            CurrentSystem.ToRemove.Enqueue(Component);
            CurrentSystem.NumPlanets--;

            if (CurrentSystem.NumPlanets == 0)
            {
                CurrentSystem = null;
                Initialized = false;
            }
        }

        void ISystem.Register(Engine Target)
        {
            Target.OnUpdate += OnUpdate;
        }
        void ISystem.Unregister(Engine Target)
        {
            Target.OnUpdate -= OnUpdate;
        }

        private void OnUpdate(Engine Eng, UpdateEventArgs Args)
        {
            {
                IPlanet Planet;
                while(ToAdd.TryDequeue(out Planet))
                {
                    Components.Add(Planet);
                }
                while(ToRemove.TryDequeue(out Planet))
                {
                    Components.Remove(Planet);
                }
            }

            foreach(var Planet in Components)
            {
                Planet.Update();
            }
        }

        private ConcurrentQueue<IPlanet> ToAdd = new ConcurrentQueue<IPlanet>();
        private ConcurrentQueue<IPlanet> ToRemove = new ConcurrentQueue<IPlanet>();
        private List<IPlanet> Components = new List<IPlanet>();
        private volatile uint NumPlanets = 0;
    }
}
