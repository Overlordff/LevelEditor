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
	/// Класс дополнительная панель
	/// </summary>
	public abstract class Subpanel : IDrawableComponent
	{
		#region Variables

		protected ContentManager content;

		Sprite sprite;
		bool vis = false;
		bool isActive = true;
		List<IComponent> components;
		Screen owner;

		protected MouseState MS, OMS;

		protected static Texture2D mesh = World.Content.Load<Texture2D>(Fnames.MESH);

		#endregion

		/// <summary>
		/// Список элементов IComponent, которые обрабатываются панелью
		/// </summary>
		public List<IComponent> Components {
			get { return components; }
			protected set { components = value; }
		}

		/// <summary>
		/// Экран, к которому привязана дополнительная панель
		/// </summary>
		public Screen Owner { get { return owner; } }

		/// <summary>
		/// Границы панели (только для чтения)
		/// </summary>
		public Rectangle Bounds { get { return sprite.Bounds; } }

		/// <summary>
		/// Цвет, применяемы к панели
		/// </summary>
		public Color Color {
			get { return sprite.Color; }
			set { sprite.Color = value; }
		}
		/// <summary>
		/// Видна ли панель
		/// </summary>
		public virtual bool Visible {
			get { return vis; }
			set {
				bool prevVis = vis;
				vis = value;
				sprite.Visible = vis;
				if (prevVis != vis) {
					if (vis)
						OnEnabled(new EventArgs());
					else
						OnDisabled(new EventArgs());
				}
			}
		}
		public virtual bool IsActive {
			get { return isActive; }
			set { isActive = value; }
		}

		/// <summary>
		/// Событие проиходит, когда панель активируется
		/// </summary>
		public event EventHandler<EventArgs> Enabled;
		/// <summary>
		/// Событие возникает, когда панель деактивируется
		/// </summary>
		public event EventHandler<EventArgs> Disabled;

		/// <summary>
		/// Создает новую дополнительну панель с указанными параметрами
		/// </summary>
		/// <param name="content">Управляющий контентом для этой панели</param>
		/// <param name="sprite">Спрайт, изображение панели</param>
		public Subpanel(Screen owner, Sprite sprite) {
			this.sprite = sprite;
			this.content = owner.Game.Content;
			this.owner = owner;
			components = new List<IComponent>();
		}

		/// <summary>
		/// Обновление панели в текущем кадре
		/// </summary>
		public virtual void Update(GameTime gameTime) {
			if (Visible && IsActive) {
				OMS = MS;
				MS = Mouse.GetState();
				MouseHandler(MS, OMS);
				for (int i = components.Count - 1; i >= 0; i--) {
					if (components[i] != null)
						components[i].Update(gameTime);
				}
			}
		}

		/// <summary>
		/// Прорисовка панели в текущем кадре
		/// </summary>
		public virtual void Draw(SpriteBatch spriteBatch) {
			if (Visible) {
				sprite.Draw(spriteBatch);
				for (int i = 0; i < components.Count; i++) {
					if (components[i] is IDrawableComponent)
						(components[i] as IDrawableComponent).Draw(spriteBatch);
				}
			}
		}

		/// <summary>
		/// Обработка мыши
		/// </summary>
		/// <param name="ms">Текущее состояние мыши</param>
		/// <param name="oms">Предыдущее состояние мыши</param>
		protected virtual void MouseHandler(MouseState ms, MouseState oms) {

		}

		#region Private Methods

		private void OnEnabled(EventArgs e) {
			EventHandler<EventArgs> enabled = Enabled;
			if (enabled != null)
				enabled(this, e);
		}

		private void OnDisabled(EventArgs e) {
			EventHandler<EventArgs> disabled = Disabled;
			if (disabled != null)
				disabled(this, e);
		}

		#endregion
	}
}
