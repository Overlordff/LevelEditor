using System;
using System.Collections.Generic;
using System.Linq;
using Graph;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace LevelEditor.Core
{
	/// <summary>
	/// Камера
	/// </summary>
	public struct Camera
	{
		private Rectangle rect;
		private Rectangle field;
		private Rectangle viewport;
		private Rectangle screenBounds;

		/// <summary>
		/// Прямоугольник сцены
		/// </summary>
		public Rectangle Rect {
			get { return rect; }
			internal set {
				if (value.Left < field.Left) value.X = field.Left;
				if (value.Top < field.Top) value.Y = field.Top;
				if (value.Left + value.Width > field.Width) value.X = field.Width - value.Width;
				if (value.Top + value.Height > field.Height) value.Y = field.Height - value.Height;
				rect = value;
			}
		}

		public Rectangle Viewport { get { return viewport; } }

		public bool Contains(Rectangle bounds) {
			return rect.Intersects(bounds) || rect.Contains(bounds);
		}
		public bool Contains(Vector2 v) {
			return rect.Contains((int)v.X, (int)v.Y);
		}

		/// <summary>
		/// Создает камеру на указанном глобальном поле
		/// </summary>
		/// <param name="field">Глобальное поле</param>
		/// <param name="viewport">Прямоугольник, определяющий размеры и положение камеры на экране, также определяет размеры камеры в логическом пространстве</param>
		public Camera(Rectangle field, Rectangle viewport, Rectangle screenBounds) {
			rect = new Rectangle(0, 0, viewport.Width, viewport.Height);
			this.field = field;
			this.viewport = viewport;
			this.screenBounds = screenBounds;
			if (!screenBounds.Contains(viewport))
				throw new ArgumentException("Положение камеры на экране должно быть в пределах экрана", "viewport");
		}

		/// <summary>
		/// Перемещает сцену на указанный вектор
		/// </summary>
		/// <param name="v">Вектор, на который перемещается сцена</param>
		public void MoveOn(Vector2 v) {
			Rect = new Rectangle(Rect.Left + (int)v.X, Rect.Top + (int)v.Y, Rect.Width, Rect.Height);
		}
	}
}
