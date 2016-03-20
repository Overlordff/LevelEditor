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
	public class MainMenuScreen : Screen
	{
		#region Variables

		List<Button> buttons = new List<Button>();
		AnimationManager backGround;

		MapInitPanel mapInitPanel;

		#endregion

		public MainMenuScreen(Game1 game) : base(game) {
			Initialize();
		}

		protected override void Initialize() {
			backGround = new AnimationManager(GraphicsDevice.Viewport.Bounds);
			backGround.TimeToNextFrame = 500;
			backGround.ColorStep = 2;
			base.Initialize();
		}

		protected override void LoadContent() {

			#region Background

			Texture2D f1 = Content.Load<Texture2D>(Fnames.BACKGROUND_FRAME1);
			Texture2D f2 = Content.Load<Texture2D>(Fnames.BACKGROUND_FRAME2);
			Texture2D f3 = Content.Load<Texture2D>(Fnames.BACKGROUND_FRAME3);
			Texture2D f4 = Content.Load<Texture2D>(Fnames.BACKGROUND_FRAME4);

			backGround.AddFrame(f1);
			backGround.AddFrame(f2);
			backGround.AddFrame(f3);
			backGround.AddFrame(f4);

			#endregion

			#region Buttons

			int start = 350;
			int step = 65;
			int count = 0;

			Button StartEdit = new Button(MultiSprite.CreateSprite(Content, Fnames.START_EDIT_B, new Vector2(Window.ClientBounds.Width / 2f - 80, Window.ClientBounds.Height / 1000f * (start + step * count++)), new Vector2(160, 55), Vector2.One));
			StartEdit.Click += new EventHandler<MouseElementEventArgs>(StartEdit_Click);
			StartEdit.AnimationMode = AnimationStates.Small;
			StartEdit.MouseMove += new EventHandler<MouseElementEventArgs>(Button_MouseMove);
			StartEdit.MouseMoveOut += new EventHandler<MouseElementEventArgs>(Button_MouseMoveOut);

			Button Exit = new Button(MultiSprite.CreateSprite(Content, Fnames.EXIT_B2, new Vector2(Window.ClientBounds.Width / 2f - 80, Window.ClientBounds.Height / 1000f * (start + step * count++)), new Vector2(160, 55), Vector2.One));
			Exit.AnimationMode = AnimationStates.Small;
			Exit.Click += new EventHandler<MouseElementEventArgs>(Exit_Click);
			Exit.MouseMove += new EventHandler<MouseElementEventArgs>(Button_MouseMove);
			Exit.MouseMoveOut += new EventHandler<MouseElementEventArgs>(Button_MouseMoveOut);

			buttons.Add(StartEdit);
			buttons.Add(Exit);

			#endregion

			#region Panels

			mapInitPanel = new MapInitPanel(this, Sprite.CreateSprite(Content, Fnames.PANEL1, new Vector2(Window.ClientBounds.Width / 1000f * 220, Window.ClientBounds.Height / 1000f * 250), new Vector2(730, 500)));
			mapInitPanel.Enabled += new EventHandler<EventArgs>(panel_Enabled);
			mapInitPanel.Disabled += new EventHandler<EventArgs>(panel_Disabled);
			mapInitPanel.CreateCalled += new EventHandler<MapInitEventArgs>(mapInitPanel_CreateCalled);

			#endregion

			Components.AddRange(buttons);
			Components.Add(mapInitPanel);
		}

		protected override void UnloadContent() {
			
		}

		public override void Update(GameTime gameTime) {
			backGround.Update(gameTime);
			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime) {
			GraphicsDevice.Clear(Color.CornflowerBlue);

			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			backGround.Draw(spriteBatch);
			base.Draw(gameTime);

			spriteBatch.End();
		}

		protected override void KeyboardHandler(KeyboardState kbs, KeyboardState okbs) {
			throw new NotImplementedException();
		}

		protected override void MouseHandler(MouseState ms, MouseState oms, MouseStates state) {
			throw new NotImplementedException();
		}

		#region Private Methods

		private void StartEdit_Click(object sender, MouseElementEventArgs e) {
			mapInitPanel.Visible = true;
			//Game.CurrentScreen = new EditorScreen(Game, 64, new Rectangle(0, 0, 5000, 4000));
		}

		private void Exit_Click(object sender, MouseElementEventArgs e) {
			Game.Exit();
		}

		private void Button_MouseMove(object sender, MouseElementEventArgs e) {
			(sender as Button).Animation = true;
		}

		private void Button_MouseMoveOut(object sender, MouseElementEventArgs e) {
			(sender as Button).Animation = false;
		}

		private void mapInitPanel_CreateCalled(object sender, MapInitEventArgs e) {
			Game.CurrentScreen = new EditorScreen(Game, e.TileW, e.Field);
		}

		private void panel_Enabled(object sender, EventArgs e) {
			for (int i = buttons.Count - 1; i >= 0; i--) {
				buttons[i].Visible = false;
			}
		}

		private void panel_Disabled(object sender, EventArgs e) {
			for (int i = buttons.Count - 1; i >= 0; i--) {
				buttons[i].Visible = true;
				buttons[i].ToNormal();
			}
		}

		#endregion
	}
}
