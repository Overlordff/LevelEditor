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
	/// Класс ползунок
	/// </summary>
	public class Slider : Element
	{
		#region Variables

		Sprite image_;
		float value;
		TextSprite text;

		#endregion

		/// <summary>
		/// Изображение каретки ползунка (только для чтения)
		/// </summary>
		public Sprite Image_ { get { return image_; } }
		/// <summary>
		/// Значение ползунка [0;100]
		/// </summary>
		public decimal Value {
			get { return (decimal)value; }
			set {
				float prev = this.value;
				if (value < 0) value = 0;
				if (value > 100) value = 100;
				this.value = (float)value;
				image_.Position = new Vector2(Position.X + Image.Size.X * (this.value / 100) - image_.Size.X / 2, Position.Y);
				image_.MoveOn(new Vector2(0, Image.Size.Y));
				if (this.value == 0.9933333f)
					this.value = 100f;
				if (prev != this.value)
					OnValueChanged(new ValueChangedEventArgs<float>(this.value));
			}
		}

		public event EventHandler<ValueChangedEventArgs<float>> ValueChanged;

		/// <summary>
		/// Создает новый ползунок с указанными параметрами
		/// </summary>
		/// <param name="image">Изображение тела ползунка</param>
		/// <param name="image_">Изображение каретки ползунка</param>
		public Slider(MultiSprite image, Sprite image_, ContentManager content, string text = "") : base(image) {
			this.image_ = image_;
			this.image_.Color = Color.Black;
			this.text = TextSprite.CreateSprite(content, Fnames.TEXT_FONT, new Vector2(Image.Bounds.Left, Image.Bounds.Top), text);
			this.text.Position = new Vector2(Image.Bounds.Left + (Image.Bounds.Width - this.text.Size.X) / 2, Image.Bounds.Top - this.text.Size.Y);
			this.MouseMove += new EventHandler<MouseElementEventArgs>(Slider_MouseMove);

			Value = 100;
		}
		public Slider(MultiSprite image, Sprite image_, Color color_, ContentManager content, string text = "")
			: base(image) {
			this.image_ = image_;
			this.image_.Color = color_;
			this.text = TextSprite.CreateSprite(content, Fnames.TEXT_FONT, new Vector2(Image.Bounds.Left, Image.Bounds.Top), text);
			this.text.Position = new Vector2(Image.Bounds.Left + (Image.Bounds.Width - this.text.Size.X) / 2, Image.Bounds.Top - this.text.Size.Y);
			this.MouseMove += new EventHandler<MouseElementEventArgs>(Slider_MouseMove);

			Value = 100;
		}
		public Slider(Slider g)
			: base(g) {
			image_ = (Sprite)g.image_.Clone();
			value = g.value;
			text = (TextSprite)g.text.Clone();
		}

		/// <summary>
		/// Прорисовка полхунка в текущем кадре
		/// </summary>
		public override void Draw(SpriteBatch spriteBatch) {
			if (Visible) {
				base.Draw(spriteBatch);
				image_.Draw(spriteBatch);
				text.Draw(spriteBatch);
			}
		}

		public override object Clone() {
			return new Slider(this);
		}

		#region Private Methods


		private void Slider_MouseMove(object sender, MouseElementEventArgs e) {
			if (e.MouseState.LeftButton == ButtonState.Pressed) {
				image_.Position = new Vector2(e.Position.X - image_.Size.X / 2, image_.Position.Y);
				if (image_.Position.X + image_.Size.X / 2 > Image.Position.X + Image.Size.X)
					image_.Position = new Vector2(Image.Position.X + Image.Size.X - image_.Size.X / 2, image_.Position.Y);
				if (image_.Position.X + image_.Size.X / 2 < Image.Position.X)
					image_.Position = new Vector2(Image.Position.X - image_.Size.X / 2, image_.Position.Y);
				Value = (decimal)((image_.Position.X - Image.Position.X + image_.Size.X / 2) / Image.Size.X * 100);
			}
		}

		private void OnValueChanged(ValueChangedEventArgs<float> e) {
			EventHandler<ValueChangedEventArgs<float>> valueChanged = ValueChanged;
			if (valueChanged != null)
				valueChanged(this, e);
		}

		#endregion
	}
}
