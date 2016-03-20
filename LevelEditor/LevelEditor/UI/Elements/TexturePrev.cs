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
	public class TexturePrev : Element
	{
		Tiles tileType;
		bool focused = false;
		TextSprite text;

		public override Vector2 Position {
			get {
				return base.Position;
			}
			set {
				Vector2 prev = Position;
				base.Position = value;
				text.MoveOn(value - prev);
			}
		}

		public override Vector2 Size {
			get {
				return base.Size;
			}
			set {
				base.Size = value;
				text.Position = new Vector2(Image.Bounds.Left + (Image.Bounds.Width - text.Size.X) / 2, Image.Bounds.Bottom);
			}
		}

		public Tiles TileType {
			get { return tileType; }
		}

		public bool IsFocused {
			get { return focused; }
			set {
				focused = value;
				if (focused)
					OnFocusedChanged(new EventArgs());
			}
		}

		public bool IsTextVisible {
			get { return text.Visible; }
			set { text.Visible = value; }
		}

		public event EventHandler<EventArgs> FocusChanged;

		public TexturePrev(MultiSprite image, Tiles tileType, ContentManager content)
			: base(image) {
			this.tileType = tileType;
			text = TextSprite.CreateSprite(content, Fnames.TEXT_FONT, new Vector2(image.Bounds.Left, image.Bounds.Bottom), tileType.ToString());
			text.Position = new Vector2(image.Bounds.Left + (image.Bounds.Width - text.Size.X) / 2, image.Bounds.Bottom);
		}
		public TexturePrev(TexturePrev g)
			: base(g) {
			tileType = g.tileType;
			text = (TextSprite)g.text.Clone();			
		}

		public override void Draw(SpriteBatch spriteBatch) {
			if (Visible && focused) {
				Rectangle rect = new Rectangle(Image.Bounds.X - 3, Image.Bounds.Y - 3, Image.Bounds.Width + 6, Image.Bounds.Height + 6);
				spriteBatch.Draw(mesh, rect, Color.White);
			}
			base.Draw(spriteBatch);
			text.Draw(spriteBatch);
		}

		public override object Clone() {
			return new TexturePrev(this);
		}

		#region Private Methods

		private void OnFocusedChanged(EventArgs e) {
			EventHandler<EventArgs> focusedChanged = FocusChanged;
			if (focusedChanged != null)
				focusedChanged(this, e);
		}

		#endregion
	}
}
