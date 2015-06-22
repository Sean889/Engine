using EngineSystem;
using BulletSharp;
using OpenTK;

namespace PhysicsSystem
{
    public class BulletPhysicsComponent : MotionState, IEntityComponent
    {
        internal const uint IID = 10;

        internal RigidBody Body;
        internal BulletPhysicsSystem System;
        /// <summary>
        /// The Entity this component is attached to.
        /// </summary>
        internal Entity Parent;
        /// <summary>
        /// A Matrix containing the rotation and translation.
        /// </summary>
        private Matrix4d Transform;

        public static uint ID
        {
            get
            {
                return IID;
            }
        }

        public uint GetID()
        {
            return IID;
        }
        public void OnCreate(Entity e)
        {
            Parent = e;
        }
        public void OnRemove(Entity e)
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

        internal BulletPhysicsComponent(RigidBody Body, BulletPhysicsSystem ParentSys)
        {
            this.Body = Body;
            System = ParentSys;

            Body.MotionState = this;
        }
    }
}
