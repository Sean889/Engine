using BulletSharp;
using EngineSystem;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PhysicsSystem
{
    /// <summary>
    /// A system that manages all BulletPhysicsComponents.
    /// </summary>
    public class BulletPhysicsSystem : ISystem, IDisposable
    {
        private DynamicsWorld World;
        private List<BulletPhysicsComponent> Components = new List<BulletPhysicsComponent>();
        private ConcurrentQueue<BulletPhysicsComponent> ToAdd = new ConcurrentQueue<BulletPhysicsComponent>();
        private ConcurrentQueue<BulletPhysicsComponent> ToRemove = new ConcurrentQueue<BulletPhysicsComponent>();
        private ConcurrentQueue<Action<DynamicsWorld>> Actions = new ConcurrentQueue<Action<DynamicsWorld>>();
        
        /// <summary>
        /// Checks whether the system has been disposed.
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                if (World != null)
                    return World.IsDisposed;
                return true;
            }
        }

        /// <summary>
        /// Registers the appropriate callbacks with the engine.
        /// </summary>
        /// <param name="Target"></param>
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
        private void UpdateEnd(Engine Sender, EventArgs Args)
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

            Action<DynamicsWorld> Func;
            while(Actions.TryDequeue(out Func))
            {
                //Execute the function
                Func(World);
            }
        }

        /// <summary>
        /// Creates the System with a default DiscreteDynamicsWorld
        /// </summary>
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
        public void ExecuteAction(Action<DynamicsWorld> Action)
        {
            Actions.Enqueue(Action);
        }

        /// <summary>
        /// Factory method for creating a BulletPhysicsComponent.
        /// </summary>
        /// <param name="Body"> The RigidBody to use for the component. </param>
        /// <returns> A physics component. </returns>
        public BulletPhysicsComponent CreateComponent(RigidBody Body)
        {
            return new BulletPhysicsComponent(Body, this);
        }

        /// <summary>
        /// Disposes the system.
        /// </summary>
        public void Dispose()
        {
            if (World != null)
                World.Dispose();

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the system if it hasn't been handled yet.
        /// </summary>
        ~BulletPhysicsSystem()
        {
            World.Dispose();
        }
    }
}
