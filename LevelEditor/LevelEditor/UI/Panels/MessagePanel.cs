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
	public class MessagePanel : Subpanel
	{
		string text;
		Vector2 textPos;
		string capture;
		Vector2 captPos;
		SpriteFont font;

		public string Text {
			get { return text; }
			set { text = value; }
		}

		public string Capture {
			get { return capture; }
			set { capture = value; }
		}

		public MessagePanel(Screen owner, Sprite sprite, string text, string capture)
			: base(owner, sprite) {
			this.capture = capture;
			this.text = text;
			font = content.Load<SpriteFont>(Fnames.TEXT_FONT);

			Button Ok = new Button(MultiSprite.CreateSprite(content, Fnames.OK_B, new Vector2(Bounds.Left + Bounds.Width / 1000f * 450, Bounds.Top + Bounds.Height / 1000f * 800), new Vector2(120, 41), Vector2.One));
			Ok.Click += new EventHandler<MouseElementEventArgs>(Ok_Click);

			Components.Add(Ok);
		}

		public override void Draw(SpriteBatch spriteBatch) {
			base.Draw(spriteBatch);
			if (Visible) {
				textPos = new Vector2(Bounds.Left + (Bounds.Width - font.MeasureString(text).X) / 2, Bounds.Top + Bounds.Height / 2);
				captPos = new Vector2(Bounds.Left + 5, Bounds.Top + 2);
				spriteBatch.DrawString(font, text, textPos, Color.White);
				spriteBatch.DrawString(font, capture, captPos, Color.White);
			}
		}

		private void Ok_Click(object sender, MouseElementEventArgs e) {
			Visible = false;
		}
	}
}
