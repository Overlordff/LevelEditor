using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Graph;
using LESL;

namespace LevelEditor
{
	/// <summary>
	/// Класс, содержащий информацию о событии, связанном с использованием мыши на элементе
	/// </summary>
	public class MouseElementEventArgs : EventArgs
	{
		/// <summary>
		/// Позиция мыши в момент события
		/// </summary>
		public Vector2 Position { get; private set; }
		/// <summary>
		/// Текущее состояние мыши в момент события
		/// </summary>
		public MouseState MouseState { get; private set; }

		/// <summary>
		/// Создает новый экземпляр MouseButtonEventArgs с указанными параметрами
		/// </summary>
		public MouseElementEventArgs(Vector2 position, MouseState state) {
			Position = position;
			MouseState = state;
		}
	}

	internal class ChangePrevEventArgs : EventArgs
	{
		internal TexturePrev Prev { get; private set; }

		internal ChangePrevEventArgs(TexturePrev prev) {
			Prev = prev;
		}
	}

	public class ValueChangedEventArgs<T> : EventArgs
	{
		public T Value { get; private set; }

		public ValueChangedEventArgs(T value) {
			Value = value;
		}
	}

	public class TileEventArgs : EventArgs
	{
		public Tile? Tile { get; private set; }

		public TileEventArgs(Tile? tile) {
			Tile = tile;
		}
	}

	/// <summary>
	/// Данные о событии, связанном с миникартой и мышью
	/// </summary>
	public class MinimapMouseEventArgs : EventArgs
	{
		/// <summary>
		/// Позиция мыши
		/// </summary>
		public Vector2 Position { get; private set; }
		/// <summary>
		/// Состояние мыши
		/// </summary>
		public MouseState MouseState { get; private set; }

		/// <summary>
		/// Создает новый экземпляр MinimapMouseEventArgs с указанными паарметрами
		/// </summary>
		public MinimapMouseEventArgs(Vector2 position, MouseState mouseState) {
			Position = position;
			MouseState = mouseState;
		}
	}

	public class SpriteChangedEventArgs : EventArgs
	{
		public Point Index { get; private set; }
		public Sprite Sprite { get; private set; }

		public SpriteChangedEventArgs(Point index, Sprite sprite) {
			Index = index;
			Sprite = sprite;
		}
	}

	public class MapInitEventArgs : EventArgs
	{
		public int TileW { get; private set; }
		public Rectangle Field { get; private set; }

		public MapInitEventArgs(int tileW, Rectangle field) {
			TileW = tileW;
			Field = field;
		}
	}
}
