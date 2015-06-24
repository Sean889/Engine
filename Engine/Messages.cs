using System;
using OpenTK;
using OpenTK.Graphics;

namespace EngineSystem.Messaging
{

    /// <summary> 
    //  An Invalid Event. Fired in case of an error in the event passing system. 
    /// </summary>
    public class InvalidEvent : IEvent
    {
        public String Error;
        public uint GetID()
        {
            return 0;
        }
        public InvalidEvent(
            String Error
            )
        {
            this.Error = Error;
        }
    }

    /// <summary> 
    //  Fired when an Entity is added to the EntitySystem. 
    /// </summary>
    public class EntityCreateEvent : IEvent
    {
        public Entity CreatedEntity;
        public uint GetID()
        {
            return 10;
        }
        public EntityCreateEvent(
            Entity CreatedEntity
            )
        {
            this.CreatedEntity = CreatedEntity;
        }
    }

    /// <summary> 
    //  Fired when an Entity is removed from the EntitySystem. 
    /// </summary>
    public class EntityDestroyEvent : IEvent
    {
        public Entity DestroyedEntity;
        public uint GetID()
        {
            return 11;
        }
        public EntityDestroyEvent(
            Entity DestroyedEntity
            )
        {
            this.DestroyedEntity = DestroyedEntity;
        }
    }

    /// <summary> 
    //  Fired when a key is pressed. 
    /// </summary>
    public class KeyPressedEvent : IEvent
    {
        public KeyEventArgs Args;
        public uint GetID()
        {
            return 20;
        }
        public KeyPressedEvent(
            KeyEventArgs Args
            )
        {
            this.Args = Args;
        }
    }

    /// <summary> 
    //  Fired when a key is released. 
    /// </summary>
    public class KeyReleasedEvent : IEvent
    {
        public uint GetID()
        {
            return 21;
        }
        public KeyReleasedEvent(
            )
        {
        }
    }

    /// <summary> 
    //  Fired when a key repeats due to being pressed down for longer. 
    /// </summary>
    public class KeyRepeatEvent : IEvent
    {
        public uint GetID()
        {
            return 22;
        }
        public KeyRepeatEvent(
            )
        {
        }
    }

    /// <summary> 
    //  Fired by the rendering system when the window is resized. 
    /// </summary>
    public class WindowResizeEvent : IEvent
    {
        public uint GetID()
        {
            return 30;
        }
        public WindowResizeEvent(
            )
        {
        }
    }
}
