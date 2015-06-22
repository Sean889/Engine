using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngineSystem;
using BulletSharp;
using OpenTK;

namespace PhysicsSystem
{
    public class BulletPhysicsSystem : ISystem
    {
        private DiscreteDynamicsWorld World;
        private List<BulletPhysicsComponent> Components = new List<BulletPhysicsComponent>();
        private ConcurrentQueue<BulletPhysicsComponent> ToAdd = new ConcurrentQueue<BulletPhysicsComponent>();
        private ConcurrentQueue<BulletPhysicsComponent> ToRemove = new ConcurrentQueue<BulletPhysicsComponent>();
        private ConcurrentQueue<Action<DiscreteDynamicsWorld>> Actions = new ConcurrentQueue<Action<DiscreteDynamicsWorld>>();

        public void Register(Engine Target)
        {
            Target.OnUpdate += Update;
            Target.OnUpdateEnd += UpdateEnd;
        }

        private void Update(Engine Sender, UpdateEventArgs Args)
        {
            foreach(BulletPhysicsComponent Component in Components)
            {
                //Have the component update itself
                Component.OnUpdate();
            }

            //Move the simulation ahead
            World.StepSimulation(Args.TimePassed, 5);
        }
        private void UpdateEnd(Engine Sender, Null Args)
        {
            BulletPhysicsComponent Component;
            while(ToAdd.TryDequeue(out Component))
            {
                //Add the component's RigidBody
                World.AddRigidBody(Component.Body);
                //Remove the component
                Components.Add(Component);
            }

            while(ToRemove.TryDequeue(out Component))
            {
                //Remove the component's RigidBody
                World.RemoveRigidBody(Component.Body);
                //Remove the component
                Components.Remove(Component);
            }

            Action<DiscreteDynamicsWorld> Func;
            while(Actions.TryDequeue(out Func))
            {
                //Execute the function
                Func(World);
            }
        }

        public BulletPhysicsSystem()
        {
            CollisionConfiguration Config = new DefaultCollisionConfiguration();
            Dispatcher Disp = new CollisionDispatcher(Config);
            BroadphaseInterface Interface = new DbvtBroadphase();
            ConstraintSolver Solver = new SequentialImpulseConstraintSolver();

            World = new DiscreteDynamicsWorld(Disp, Interface, Solver, Config);
        }

        /// <summary>
        /// Adds the object to the physics world at the end of the update cycle.
        /// </summary>
        /// <param name="Object"> The object to be added. </param>
        public void AddObject(BulletPhysicsComponent Object)
        {
            ToAdd.Enqueue(Object);
        }
        /// <summary>
        /// Removes an object from the physics world at the end of the update cycle.
        /// </summary>
        /// <param name="Object"> The object to be removed. </param>
        public void RemoveObject(BulletPhysicsComponent Object)
        {
            ToRemove.Enqueue(Object);
        }
        /// <summary>
        /// Adds an action that will be executed at the end of the update cycle.
        /// This will allow the action to interface with the DiscreteDynamicsWorld safely.
        /// </summary>
        /// <param name="Action"> The function to be executed. </param>
        public void ExecuteAction(Action<DiscreteDynamicsWorld> Action)
        {
            Actions.Enqueue(Action);
        }
    }
}
