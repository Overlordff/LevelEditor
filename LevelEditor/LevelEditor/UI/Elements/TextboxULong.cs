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
	public class TextboxULong : Element
	{
		const string DEFAULT = "0";

		#region Variables

		KeyboardState KBS, OKBS;
		SpriteFont font;
		string text = DEFAULT;
		RenderTarget2D render;
		GraphicsDevice device;
		SpriteBatch spriteBatch;

		bool focused = false;      //Set True in Click handler
		int maxLength = 19;

		#endregion

		public override Vector2 Size {
			get {
				return base.Size;
			}
			set {
				base.Size = value;
				render = new RenderTarget2D(device, (int)value.X - 10, (int)value.Y);
			}
		}

		/// <summary>
		/// [0,19]
		/// </summary>
		public int MaxTextLength {
			get { return maxLength; }
			set {
				if (value < 0)
					value = 0;
				if (value > 19)
					value = 19;
				maxLength = value;
				if (text.Length > maxLength) {
					text = text.Remove(maxLength);
					if (text == "")
						text = DEFAULT;
				}
			}
		}

		public ulong Value {
			get { return ulong.Parse(text); }
			set {
				text = value.ToString();
				MaxTextLength = MaxTextLength;
				OnValueChanged(new ValueChangedEventArgs<ulong>(value));
			}
		}

		public event EventHandler<ValueChangedEventArgs<ulong>> ValueChanged;

		public TextboxULong(MultiSprite image, SpriteFont font, GraphicsDevice device)
			: base(image) {
			this.font = font;
			this.device = device;
			spriteBatch = new SpriteBatch(device);
			render = new RenderTarget2D(device, image.Width - 5, image.Height);
			this.Click += new EventHandler<MouseElementEventArgs>(Textbox_Click);
		}

		public TextboxULong(TextboxULong g)
			: base(g) {
			g.font = font;
			g.device = device;
			g.spriteBatch = new SpriteBatch(g.device);
			g.text = text;
			g.focused = false;
			g.maxLength = maxLength;
			g.render = new RenderTarget2D(g.device, g.Image.Width - 5, g.Image.Height);
			g.Click += new EventHandler<MouseElementEventArgs>(Textbox_Click);
		}

		public override object Clone() {
			return new TextboxULong(this);
		}

		public override void Update(GameTime gameTime) {
			OKBS = KBS;
			KBS = Keyboard.GetState();
			KeyboardHandler(KBS, OKBS);
			base.Update(gameTime);
		}

		public override void Draw(SpriteBatch spriteBatch) {
			device.SetRenderTarget(render);
			device.Clear(Color.Transparent);
			this.spriteBatch.Begin();
			this.spriteBatch.DrawString(font, text, new Vector2(5, 0), Color.White);
			this.spriteBatch.End();
			device.SetRenderTarget(null);

			Rectangle rect;
			rect = new Rectangle(Bounds.Left - 1, Bounds.Top - 1, Bounds.Width + 2, Bounds.Height + 2);
			if (focused)
				spriteBatch.Draw(mesh, rect, Color.White);
			base.Draw(spriteBatch);
			rect = new Rectangle(Bounds.Left, Bounds.Top, Bounds.Width, Bounds.Height);
			spriteBatch.Draw(render, rect, Color.White);
		}

		protected override void MouseHandler(MouseState ms, MouseState oms) {
			base.MouseHandler(ms, oms);
			Point point = new Point(ms.X, ms.Y);
			if (ms.LeftButton == ButtonState.Pressed && oms.LeftButton == ButtonState.Released && !Bounds.Contains(point)) {
				focused = false;
			}
		}

		protected void KeyboardHandler(KeyboardState kbs, KeyboardState okbs) {
			var buttons = kbs.GetPressedKeys();
			var prevButtons = okbs.GetPressedKeys();
			if (buttons.Length == 1 && prevButtons.Length == 0) {
				if (focused && isDigitOrBack(buttons[0])) {
					char c;
					if (ToDigitChar(buttons[0], out c))
						if (c == '\b') {
							if (text.Length > 0) {
								text = text.Remove(text.Length - 1);
								if (text == "")
									text = DEFAULT;
								OnValueChanged(new ValueChangedEventArgs<ulong>(ulong.Parse(text)));
							}
						} else if (text.Length < maxLength) {
							if (text == DEFAULT)
								text = "";
							text += c;
							OnValueChanged(new ValueChangedEventArgs<ulong>(ulong.Parse(text)));
						}
				}
			}
		}

		#region Private Methods

		private void Textbox_Click(object sender, MouseElementEventArgs e) {
			focused = true;
		}

		private bool isDigitOrBack(Keys key) {
			switch (key) {
				case Keys.D0:
					return true;
				case Keys.D1:
					return true;
				case Keys.D2:
					return true;
				case Keys.D3:
					return true;
				case Keys.D4:
					return true;
				case Keys.D5:
					return true;
				case Keys.D6:
					return true;
				case Keys.D7:
					return true;
				case Keys.D8:
					return true;
				case Keys.D9:
					return true;
				case Keys.Back:
					return true;
				default:
					return false;
			}
		}

		private bool ToDigitChar(Keys key, out char c) {
			switch (key) {
				case Keys.D0:
					c = '0';
					return true;
				case Keys.D1:
					c = '1';
					return true;
				case Keys.D2:
					c = '2';
					return true;
				case Keys.D3:
					c = '3';
					return true;
				case Keys.D4:
					c = '4';
					return true;
				case Keys.D5:
					c = '5';
					return true;
				case Keys.D6:
					c = '6';
					return true;
				case Keys.D7:
					c = '7';
					return true;
				case Keys.D8:
					c = '8';
					return true;
				case Keys.D9:
					c = '9';
					return true;
				case Keys.Back:
					c = '\b';
					return true;
				default:
					c = ' ';
					return false;
			}
		}

		private void OnValueChanged(ValueChangedEventArgs<ulong> e) {
			EventHandler<ValueChangedEventArgs<ulong>> valueChanged = ValueChanged;
			if (valueChanged != null)
				valueChanged(this, e);
		}

		#endregion
	}
}
