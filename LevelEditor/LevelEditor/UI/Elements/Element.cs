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

namespace LevelEditor
{
	/// <summary>
	/// Базовый класс для элементов управления
	/// </summary>
	public abstract class Element : IDrawableComponent, ICloneable
	{
		#region Variables

		MultiSprite image;
		Vector2 position;
		Vector2 size;
		bool vis;
		Rectangle bounds;
		MouseState MS, OMS;

		protected static Texture2D mesh = World.Content.Load<Texture2D>(Fnames.MESH);

		#endregion

		/// <summary>
		/// Изображение элемента управления (только для чтения)
		/// </summary>
		public MultiSprite Image { get { return image; } }
		/// <summary>
		/// Позиция элемента управления относительно левого верхнего угла экрана
		/// </summary>
		public virtual Vector2 Position {
			get { return position; }
			set {
				position = value;
				image.Position = position;
			}
		}
		/// <summary>
		/// Размер элемента управления
		/// </summary>
		public virtual Vector2 Size {
			get { return size; }
			set {
				size = value;
				image.Size = size;
			}
		}
		/// <summary>
		/// Прямоугольник, определяющий границы элемента управления (только для чтения)
		/// </summary>
		public Rectangle Bounds { get { return bounds; } }
		/// <summary>
		/// Виден ли элемент управления
		/// </summary>
		public virtual bool Visible {
			get { return vis; }
			set {
				vis = value;
				image.Visible = vis;
			}
		}

		/// <summary>
		/// Событие щелчка мыши по элементу управления
		/// </summary>
		public event EventHandler<MouseElementEventArgs> Click;
		/// <summary>
		/// События перемещение мыши по элементу управления
		/// </summary>
		public event EventHandler<MouseElementEventArgs> MouseMove;
		/// <summary>
		/// Событие перемещения мыши внутрь границ элемента управления извне
		/// </summary>
		public event EventHandler<MouseElementEventArgs> MouseMoveIn;
		/// <summary>
		/// Событие перемещения мыши за границы элемента управления изнутри
		/// </summary>
		public event EventHandler<MouseElementEventArgs> MouseMoveOut;

		/// <summary>
		/// Создает новый элемент управления с указанным изображением
		/// </summary>
		/// <param name="image">Изображение для элемента управления</param>
		public Element(MultiSprite image) {
			this.position = image.Position;
			this.size = image.Size;
			this.image = image;
			bounds = new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y);
			vis = true;
		}
		public Element(Element g) {
			position = g.position;
			size = g.size;
			image = (MultiSprite)g.image.Clone();
			bounds = g.bounds;
			vis = g.vis;
		}

		/// <summary>
		/// Обновление элемента управления в текущем кадре
		/// </summary>
		public virtual void Update(GameTime gameTime) {
			OMS = MS;
			MS = Mouse.GetState();
			MouseHandler(MS, OMS);
		}

		/// <summary>
		/// Прорисовка элемента управления
		/// </summary>
		public virtual void Draw(SpriteBatch spriteBatch) {
			if (Visible)
				image.Draw(spriteBatch);
		}

		public abstract object Clone();

		#region Protected Methods

		/// <summary>
		/// Обработчик мыши для элемента управления
		/// </summary>
		/// <param name="ms">Текущее состояние мыши</param>
		/// <param name="oms">Предыдущее состояние мыши</param>
		protected virtual void MouseHandler(MouseState ms, MouseState oms) {
			if (Visible) {
				if (image.Bounds.Contains(new Point(ms.X, ms.Y))) {
					OnMouseMove(new MouseElementEventArgs(new Vector2(ms.X, ms.Y), ms));
					if (ms.LeftButton == ButtonState.Pressed && oms.LeftButton == ButtonState.Released) {
						OnClick(new MouseElementEventArgs(new Vector2(ms.X, ms.Y), ms));
					}
				}
				if (image.Bounds.Contains(new Point(oms.X, oms.Y)) && !image.Bounds.Contains(ms.X, ms.Y))
					OnMouseMoveOut(new MouseElementEventArgs(new Vector2(ms.X, ms.Y), ms));
				if (image.Bounds.Contains(new Point(ms.X, ms.Y)) && !image.Bounds.Contains(oms.X, oms.Y))
					OnMouseMoveIn(new MouseElementEventArgs(new Vector2(ms.X, ms.Y), ms));
			}
		}

		#endregion

		#region Private Methods

		private void OnClick(MouseElementEventArgs e) {
			EventHandler<MouseElementEventArgs> click = Click;
			if (click != null)
				click(this, e);
		}

		private void OnMouseMove(MouseElementEventArgs e) {
			EventHandler<MouseElementEventArgs> mouseMove = MouseMove;
			if (mouseMove != null)
				mouseMove(this, e);
		}

		private void OnMouseMoveOut(MouseElementEventArgs e) {
			EventHandler<MouseElementEventArgs> mouseMoveOut = MouseMoveOut;
			if (mouseMoveOut != null)
				mouseMoveOut(this, e);
		}

		private void OnMouseMoveIn(MouseElementEventArgs e) {
			EventHandler<MouseElementEventArgs> mouseMoveIn = MouseMoveIn;
			if (mouseMoveIn != null)
				mouseMoveIn(this, e);
		}

		#endregion
	}
}
