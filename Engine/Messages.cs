using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;

#pragma warning disable 1591

namespace EngineSystem.Messaging
{

	/// <summary> 
	/// An Invalid Event. Fired in case of an error in the event passing system.
	/// </summary>
	public class InvalidEvent : IEvent
	{
		public static readonly uint Id = 0;
		public readonly String Error;
		public uint GetID()
		{
			return Id;
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
		public static readonly uint Id = 10;
		public readonly Entity CreatedEntity;
		public uint GetID()
		{
			return Id;
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
		public static readonly uint Id = 11;
		public readonly Entity DestroyedEntity;
		public uint GetID()
		{
			return Id;
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
		public static readonly uint Id = 30;
		public readonly int SizeX;
		public readonly int SizeY;
		public uint GetID()
		{
			return Id;
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
		public static readonly uint Id = 31;
		public readonly System.ComponentModel.CancelEventArgs EventArgs;
		public uint GetID()
		{
			return Id;
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
		public static readonly uint Id = 32;
		public readonly MouseWheelEventArgs EventArgs;
		public uint GetID()
		{
			return Id;
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
		public static readonly uint Id = 33;
		public readonly MouseButtonEventArgs EventArgs;
		public uint GetID()
		{
			return Id;
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
		public static readonly uint Id = 34;
		public readonly MouseButtonEventArgs EventArgs;
		public uint GetID()
		{
			return Id;
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
		public static readonly uint Id = 35;
		public readonly MouseMoveEventArgs EventArgs;
		public uint GetID()
		{
			return Id;
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
		public static readonly uint Id = 36;
		public uint GetID()
		{
			return Id;
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
		public static readonly uint Id = 37;
		public readonly KeyPressEventArgs EventArgs;
		public uint GetID()
		{
			return Id;
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
		public static readonly uint Id = 38;
		public readonly OpenTK.Input.KeyboardKeyEventArgs EventArgs;
		public uint GetID()
		{
			return Id;
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
		public static readonly uint Id = 39;
		public readonly OpenTK.Input.KeyboardKeyEventArgs EventArgs;
		public uint GetID()
		{
			return Id;
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
		public static readonly uint Id = 40;
		public readonly String NewTitle;
		public uint GetID()
		{
			return Id;
		}
		public TitleChangedEvent( String NewTitle)
		{
			this.NewTitle = NewTitle;
		}
	}

	/// <summary> 
	/// Fired when the window is moved.
	/// </summary>
	public class WindowMovedEvent : IEvent
	{
		public static readonly uint Id = 41;
		public readonly int NewPosX;
		public readonly int NewPosY;
		public uint GetID()
		{
			return Id;
		}
		public WindowMovedEvent(int NewPosX, int NewPosY)
		{
			this.NewPosX = NewPosX;
			this.NewPosY = NewPosY;
		}
	}

	/// <summary> 
	/// Fired when the mouse moves off the window.
	/// </summary>
	public class MouseLeaveEvent : IEvent
	{
		public static readonly uint Id = 42;
		public uint GetID()
		{
			return Id;
		}
		public MouseLeaveEvent()
		{
		}
	}
}
