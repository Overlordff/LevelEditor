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
	public class MapInitPanel : Subpanel
	{

		struct InitMap
		{
			public Rectangle Field;
			public int TileW;
		}

		#region Variables

		InitMap map;

		List<Button> buttons = new List<Button>();
		List<TextboxULong> textboxes = new List<TextboxULong>();
		MessagePanel msgPanel;

		SpriteFont TextFont;
		SpriteFont NumFont;

		TextboxULong TileWidthBox, FieldWidthBox, FieldHeightBox;

		#endregion

		public event EventHandler<MapInitEventArgs> CreateCalled;

		public MapInitPanel(Screen owner, Sprite sprite)
			: base(owner, sprite) {
			TextFont = content.Load<SpriteFont>(Fnames.TEXT_FONT);
			NumFont = content.Load<SpriteFont>(Fnames.SEGOE_UI);

			#region Buttons

			Button Create = new Button(MultiSprite.CreateSprite(content, Fnames.CREATE_B, new Vector2(Bounds.Right - Bounds.Width / 1000f * 250, Bounds.Top + Bounds.Height / 1000f * 700), new Vector2(120, 41), Vector2.One));
			Create.AnimationMode = AnimationStates.Small;
			Create.Click += new EventHandler<MouseElementEventArgs>(Create_Click);
			Create.MouseMove += new EventHandler<MouseElementEventArgs>(Button_MouseMove);
			Create.MouseMoveOut += new EventHandler<MouseElementEventArgs>(Button_MouseMoveOut);

			Button Cancel = new Button(MultiSprite.CreateSprite(content, Fnames.CANCEL_B, new Vector2(Bounds.Right - Bounds.Width / 1000f * 250, Bounds.Top + Bounds.Height / 1000f * 800), new Vector2(120, 41), Vector2.One));
			Cancel.AnimationMode = AnimationStates.Small;
			Cancel.Click += new EventHandler<MouseElementEventArgs>(Cancel_Click);
			Cancel.MouseMove += new EventHandler<MouseElementEventArgs>(Button_MouseMove);
			Cancel.MouseMoveOut += new EventHandler<MouseElementEventArgs>(Button_MouseMoveOut);

			buttons.Add(Create);
			buttons.Add(Cancel);

			#endregion

			#region Textboxes

			TileWidthBox = new TextboxULong(MultiSprite.CreateSprite(content, Fnames.TEXTBOX1, new Vector2(Bounds.Left + Bounds.Width / 1000f * 125, Bounds.Top + Bounds.Height / 1000f * 150).Round(), new Vector2(160, 31), Vector2.One),
									   NumFont,
									   owner.GraphicsDevice);
			TileWidthBox.ValueChanged += new EventHandler<ValueChangedEventArgs<ulong>>(TileWidthBox_ValueChanged);
			TileWidthBox.MaxTextLength = 2;
			TileWidthBox.Value = 64;

			FieldWidthBox = new TextboxULong(MultiSprite.CreateSprite(content, Fnames.TEXTBOX1, new Vector2(Bounds.Left + Bounds.Width / 1000f * 125, Bounds.Top + Bounds.Height / 1000f * 280).Round(), new Vector2(160, 31), Vector2.One),
									   NumFont,
									   owner.GraphicsDevice);
			FieldWidthBox.ValueChanged += new EventHandler<ValueChangedEventArgs<ulong>>(FieldWidthBox_ValueChanged);
			FieldWidthBox.MaxTextLength = 4;
			FieldWidthBox.Value = 5000;

			FieldHeightBox = new TextboxULong(MultiSprite.CreateSprite(content, Fnames.TEXTBOX1, new Vector2(Bounds.Left + Bounds.Width / 1000f * 125, Bounds.Top + Bounds.Height / 1000f * 380).Round(), new Vector2(160, 31), Vector2.One),
									   NumFont,
									   owner.GraphicsDevice);
			FieldHeightBox.ValueChanged += new EventHandler<ValueChangedEventArgs<ulong>>(FieldHeightBox_ValueChanged);
			FieldHeightBox.MaxTextLength = 4;
			FieldHeightBox.Value = 4000;

			textboxes.Add(TileWidthBox);
			textboxes.Add(FieldWidthBox);
			textboxes.Add(FieldHeightBox);

			#endregion

			msgPanel = new MessagePanel(owner, Sprite.CreateSprite(content, Fnames.PANEL2, new Vector2(450, 350), new Vector2(350, 250)), " ", " ");
			msgPanel.Enabled += new EventHandler<EventArgs>(msgPanel_Enabled);
			msgPanel.Disabled += new EventHandler<EventArgs>(msgPanel_Disabled);

			Components.AddRange(buttons);
			Components.AddRange(textboxes);
			Components.Add(msgPanel);
		}

		public override void Update(GameTime gameTime) {
			base.Update(gameTime);
			msgPanel.Update(gameTime);
		}

		public override void Draw(SpriteBatch spriteBatch) {
			base.Draw(spriteBatch);
			if (Visible) {
				string text1 = "Tile Width";
				spriteBatch.DrawString(TextFont, text1, new Vector2(TileWidthBox.Position.X + TileWidthBox.Size.X + 10, TileWidthBox.Position.Y + (TileWidthBox.Size.Y - TextFont.MeasureString(text1).Y) / 2f).Round(), Color.White);
				string text2 = "Field Width";
				spriteBatch.DrawString(TextFont, text2, new Vector2(FieldWidthBox.Position.X + FieldWidthBox.Size.X + 10, FieldWidthBox.Position.Y + 2).Round(), Color.White);
				string text3 = "Field Height";
				spriteBatch.DrawString(TextFont, text3, new Vector2(FieldHeightBox.Position.X + FieldHeightBox.Size.X + 10, FieldHeightBox.Position.Y + (FieldHeightBox.Size.Y - TextFont.MeasureString(text3).Y) / 2f).Round(), Color.White);

				msgPanel.Draw(spriteBatch);
			}
		}

		#region Private Methods

		private void TileWidthBox_ValueChanged(object sender, ValueChangedEventArgs<ulong> e) {
			map.TileW = (int)e.Value;
		}

		private void FieldWidthBox_ValueChanged(object sender, ValueChangedEventArgs<ulong> e) {
			map.Field.Width = (int)e.Value;
		}

		private void FieldHeightBox_ValueChanged(object sender, ValueChangedEventArgs<ulong> e) {
			map.Field.Height = (int)e.Value;
		}

		private void Create_Click(object sender, MouseElementEventArgs e) {
			if (map.TileW < 16) {
				Error("Too small tile width", "Error");
				return;
			}
			if (map.Field.Width < 1280) {
				Error("Width can't be less than 1280", "Error");
				return;
			}
			if (map.Field.Height < 1024) {
				Error("Height can't be less than 1024", "Error");
				return;
			}
			OnCreateCalled(new MapInitEventArgs(map.TileW, map.Field));
		}

		private void Cancel_Click(object sender, MouseElementEventArgs e) {
			Visible = false;
		}

		private void Button_MouseMove(object sender, MouseElementEventArgs e) {
			(sender as Button).Animation = true;
		}

		private void Button_MouseMoveOut(object sender, MouseElementEventArgs e) {
			(sender as Button).Animation = false;
		}

		private void msgPanel_Disabled(object sender, EventArgs e) {
			IsActive = true;
		}

		private void msgPanel_Enabled(object sender, EventArgs e) {
			IsActive = false;
		}

		private void Error(string text, string capture) {
			msgPanel.Text = text;
			msgPanel.Capture = capture;
			msgPanel.Visible = true;
			buttons.ForEach(x => x.ToNormal());
		}

		private void OnCreateCalled(MapInitEventArgs e) {
			EventHandler<MapInitEventArgs> createCalled = CreateCalled;
			if (createCalled != null)
				createCalled(this, e);
		}

		#endregion
	}
}
