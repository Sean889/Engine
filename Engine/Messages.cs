using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;

namespace EngineSystem.Messaging
{

	/// <summary> 
	/// An Invalid Event. Fired in case of an error in the event passing system.
	/// </summary>
	public class InvalidEvent : IEvent
	{
		public static readonly uint _Id = 0;
		public String Error;
		public uint GetID()
		{
			return _Id;
		}
		public InvalidEvent( String Error)
		{
			this.Error = Error;
		}
	}

	/// <summary> 
	/// Fired when an Entity is added to the EntitySystem.
	/// </summary>
	public class EntityCreatedEvent : IEvent
	{
		public static readonly uint _Id = 10;
		public Entity CreatedEntity;
		public uint GetID()
		{
			return _Id;
		}
		public EntityCreatedEvent( Entity CreatedEntity)
		{
			this.CreatedEntity = CreatedEntity;
		}
	}

	/// <summary> 
	/// Fired when an Entity is removed from the EntitySystem.
	/// </summary>
	public class EntityDestroyedEvent : IEvent
	{
		public static readonly uint _Id = 11;
		public Entity DestroyedEntity;
		public uint GetID()
		{
			return _Id;
		}
		public EntityDestroyedEvent( Entity DestroyedEntity)
		{
			this.DestroyedEntity = DestroyedEntity;
		}
	}

	/// <summary> 
	/// Fired by the rendering system when the window is resized.
	/// </summary>
	public class WindowResizeEvent : IEvent
	{
		public static readonly uint _Id = 30;
		public int SizeX;
		public int SizeY;
		public uint GetID()
		{
			return _Id;
		}
		public WindowResizeEvent(int SizeX, int SizeY)
		{
			this.SizeX = SizeX;
			this.SizeY = SizeY;
		}
	}

	/// <summary> 
	/// Fired by the rendering system when the rendering window is being closed.
	/// </summary>
	public class WindowClosingEvent : IEvent
	{
		public static readonly uint _Id = 31;
		public System.ComponentModel.CancelEventArgs EventArgs;
		public uint GetID()
		{
			return _Id;
		}
		public WindowClosingEvent( System.ComponentModel.CancelEventArgs EventArgs)
		{
			this.EventArgs = EventArgs;
		}
	}

	/// <summary> 
	/// Fired when the mouse wheel is moved.
	/// </summary>
	public class MouseWheelEvent : IEvent
	{
		public static readonly uint _Id = 32;
		public MouseWheelEventArgs EventArgs;
		public uint GetID()
		{
			return _Id;
		}
		public MouseWheelEvent( MouseWheelEventArgs EventArgs)
		{
			this.EventArgs = EventArgs;
		}
	}

	/// <summary> 
	/// Fired when a mouse button is pressed.
	/// </summary>
	public class MouseDownEvent : IEvent
	{
		public static readonly uint _Id = 33;
		public MouseButtonEventArgs EventArgs;
		public uint GetID()
		{
			return _Id;
		}
		public MouseDownEvent( MouseButtonEventArgs EventArgs)
		{
			this.EventArgs = EventArgs;
		}
	}

	/// <summary> 
	/// Fired when the mouse button is released.
	/// </summary>
	public class MouseUpEvent : IEvent
	{
		public static readonly uint _Id = 34;
		public MouseButtonEventArgs EventArgs;
		public uint GetID()
		{
			return _Id;
		}
		public MouseUpEvent( MouseButtonEventArgs EventArgs)
		{
			this.EventArgs = EventArgs;
		}
	}

	/// <summary> 
	/// Fired when the mouse is moved.
	/// </summary>
	public class MouseMoveEvent : IEvent
	{
		public static readonly uint _Id = 35;
		public MouseMoveEventArgs EventArgs;
		public uint GetID()
		{
			return _Id;
		}
		public MouseMoveEvent( MouseMoveEventArgs EventArgs)
		{
			this.EventArgs = EventArgs;
		}
	}

	/// <summary> 
	/// Fired when the mouse moves over the window.
	/// </summary>
	public class MouseEnterEvent : IEvent
	{
		public static readonly uint _Id = 36;
		public uint GetID()
		{
			return _Id;
		}
		public MouseEnterEvent()
		{
		}
	}

	/// <summary> 
	/// Fired when a key is pressed.
	/// </summary>
	public class KeyPressedEvent : IEvent
	{
		public static readonly uint _Id = 37;
		public KeyPressEventArgs EventArgs;
		public uint GetID()
		{
			return _Id;
		}
		public KeyPressedEvent( KeyPressEventArgs EventArgs)
		{
			this.EventArgs = EventArgs;
		}
	}

	/// <summary> 
	/// Fired when a key is held down.
	/// </summary>
	public class KeyDownEvent : IEvent
	{
		public static readonly uint _Id = 38;
		public OpenTK.Input.KeyboardKeyEventArgs EventArgs;
		public uint GetID()
		{
			return _Id;
		}
		public KeyDownEvent( OpenTK.Input.KeyboardKeyEventArgs EventArgs)
		{
			this.EventArgs = EventArgs;
		}
	}

	/// <summary> 
	/// Fired when a key is released.
	/// </summary>
	public class KeyUpEvent : IEvent
	{
		public static readonly uint _Id = 39;
		public OpenTK.Input.KeyboardKeyEventArgs EventArgs;
		public uint GetID()
		{
			return _Id;
		}
		public KeyUpEvent( OpenTK.Input.KeyboardKeyEventArgs EventArgs)
		{
			this.EventArgs = EventArgs;
		}
	}

	/// <summary> 
	/// Fired when the title changed on the window.
	/// </summary>
	public class TitleChangedEvent : IEvent
	{
		public static readonly uint _Id = 40;
		public String NewTitle;
		public uint GetID()
		{
			return _Id;
		}
		public TitleChangedEvent( String NewTitle)
		{
			this.NewTitle = NewTitle;
		}
	}
}
