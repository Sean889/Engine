using BulletSharp;
using EngineSystem;
using OpenTK;

namespace PhysicsSystem
{
    /// <summary>
    /// A physics component.
    /// The factory method CreateComponent is in BulletPhysicsSystem
    /// </summary>
    public class BulletPhysicsComponent : MotionState, IEntityComponent
    {
        /// <summary>
        /// The ID returned by this class.
        /// As a const variable it should only be used in the assembly if it could ever be changed.
        /// </summary>
        internal const uint IID = 10;

        /// <summary>
        /// The rigidbody that this component contains.
        /// </summary>
        internal RigidBody Body;
        /// <summary>
        /// The parent system.
        /// </summary>
        private BulletPhysicsSystem System;
        /// <summary>
        /// The Entity this component is attached to.
        /// </summary>
        private Entity Parent;
        /// <summary>
        /// A Matrix containing the rotation and translation.
        /// </summary>
        private Matrix4d Transform;

        /// <summary>
        /// Static access to the ID.
        /// </summary>
        public static uint ID
        {
            get
            {
                return IID;
            }
        }

        /// <summary>
        /// Gets the hash ID of this component type.
        /// </summary>
        /// <returns></returns>
        public uint GetID()
        {
            return IID;
        }
        /// <summary>
        /// Stores the given Entity as the parent Entity.
        /// This should only be called when the component is being attached to an Entity.
        /// </summary>
        /// <param name="e"></param>
        void IEntityComponent.OnCreate(Entity e)
        {
            Parent = e;
        }
        /// <summary>
        /// Called when the component is removed from an Entity.
        /// </summary>
        /// <param name="e"></param>
        void IEntityComponent.OnRemove(Entity e)
        {
            System.RemoveObject(this);
        }

        /// <summary>
        /// Called to allow position changes to sync
        /// </summary>
        public void OnUpdate()
        {
            Transform = Matrix4d.CreateFromQuaternion(Parent.Transform.Rotation) * Matrix4d.CreateTranslation(Parent.Transform.Position);
        }

        public override Matrix4d WorldTransform
        {
            get
            {
                return Transform;
            }
            set
            {
                Transform = value;

                Parent.SetTransform(new Coord(value.ExtractTranslation(), value.ExtractRotation()), 0u);
            }
        }

        /// <summary>
        /// Creates the component.
        /// </summary>
        /// <param name="Body"> The rigidbody that this component represents. </param>
        /// <param name="ParentSys"> The system it is connected to. </param>
        internal BulletPhysicsComponent(RigidBody Body, BulletPhysicsSystem ParentSys)
        {
            this.Body = Body;
            System = ParentSys;

            Body.MotionState = this;
        }
    }
}
