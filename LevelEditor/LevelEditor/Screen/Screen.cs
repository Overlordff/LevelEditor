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
	/// Класс экран
	/// </summary>
	public abstract class Screen
	{
		protected SpriteBatch spriteBatch;
		protected ContentManager Content;
		protected KeyboardState KBS, OKBS;
		protected MouseState MS, OMS;
		protected int Left, Right, Top, Bottom;

		List<IComponent> components;

		protected static Texture2D mesh = World.Content.Load<Texture2D>(Fnames.MESH);

		/// <summary>
		/// Игра, к которой привязан экран (только для чтения)
		/// </summary>
		public Game1 Game { get; private set; }
		/// <summary>
		/// Графический девайс, к которому привязан экран (только для чтения)
		/// </summary>
		public GraphicsDevice GraphicsDevice { get; private set; }
		/// <summary>
		/// Игровое окно, на котором изображен экран (только для чтения)
		/// </summary>
		public GameWindow Window { get; private set; }
		/// <summary>
		/// Список элементов IComponent, которые обрабатываются на экране (защищенная запись)
		/// </summary>
		public List<IComponent> Components {
			get { return components; }
			protected set { components = value; }
		}

		/// <summary>
		/// Создает новый экран, связанный с указанной игрой
		/// </summary>
		/// <param name="game">Игра, с которой связан экран, и из которой он берет Content, GraphicDevice и т.д.</param>
		public Screen(Game1 game) {
			Game = game;
			Content = Game.Content;
			GraphicsDevice = Game.GraphicsDevice;
			spriteBatch = new SpriteBatch(GraphicsDevice);
			Window = Game.Window;
			components = new List<IComponent>();
		}

		/// <summary>
		/// Вызывается до LoadContent()
		/// </summary>
		protected virtual void Initialize() {
			Left = GraphicsDevice.Viewport.X;
			Right = GraphicsDevice.Viewport.Width;
			Top = GraphicsDevice.Viewport.Y;
			Bottom = GraphicsDevice.Viewport.Height;
			LoadContent();
		}		

		/// <summary>
		/// Обновление экрана в текущем кадре
		/// </summary>
		public virtual void Update(GameTime gameTime) {
			for (int i = 0; i < components.Count; i++) {
				components[i].Update(gameTime);
			}
		}

		/// <summary>
		/// Прорисовка экрана в текущем кадре
		/// </summary>
		public virtual void Draw(GameTime gameTime) {
			for (int i = components.Count - 1; i >= 0; i--) {
				if (components[i] is IDrawableComponent)
					(components[i] as IDrawableComponent).Draw(spriteBatch);
			}
		}

		/// <summary>
		/// Загрузка контента (вызывается после Initialize())
		/// </summary>
		protected abstract void LoadContent();
		/// <summary>
		/// Выгрузка контента
		/// </summary>
		protected abstract void UnloadContent();
		/// <summary>
		/// Обработчик мыши
		/// </summary>
		/// <param name="ms">Текущее состояние мыши</param>
		/// <param name="oms">Предыдущее состояние мыши</param>
		/// <param name="state">Состояние MouseStates мыши</param>
		protected abstract void MouseHandler(MouseState ms, MouseState oms, MouseStates state);
		/// <summary>
		/// Обработчик клавиатуры
		/// </summary>
		/// <param name="kbs">Текущее состояние клавиатуры</param>
		/// <param name="okbs">Предыдущее состояние клавиатуры</param>
		protected abstract void KeyboardHandler(KeyboardState kbs, KeyboardState okbs);
	}
}
