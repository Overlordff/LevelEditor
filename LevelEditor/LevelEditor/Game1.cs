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
	public class Game1 : Microsoft.Xna.Framework.Game
	{
		GraphicsDeviceManager graphics;
		internal Screen CurrentScreen;

		public Game1() {
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
			World.Content = Content;
		}

		protected override void Initialize() {
#if DEBUG
			Init.SetResolution(graphics, 1280, 1024);
#else
			Init.FullScreen(graphics);
#endif
			this.IsMouseVisible = true;
			CurrentScreen = new MainMenuScreen(this);
			//CurrentScreen = new EditorScreen(this);
			base.Initialize();
		}

		protected override void LoadContent() {
			
		}

		protected override void UnloadContent() {

		}

		protected override void Update(GameTime gameTime) {
			CurrentScreen.Update(gameTime);
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime) {
			CurrentScreen.Draw(gameTime);

			base.Draw(gameTime);
		} 
	}
}
