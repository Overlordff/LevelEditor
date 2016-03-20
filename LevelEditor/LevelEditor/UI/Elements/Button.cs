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
	/// Класс кнопка
	/// </summary>
	public class Button : Element
	{
		#region Variables

		Vector2 initPos, initSize;

		AnimationStates animationMode = AnimationStates.Default;
		int animationCount = 0;
		int maxAnimCount;

		#endregion

		/// <summary>
		/// Анимируется ли кнопка
		/// </summary>
		public bool Animation { get; set; }
		public AnimationStates AnimationMode {
			get { return animationMode; }
			set {
				animationMode = value;
				maxAnimCount = (int)animationMode;
			}
		}
		/// <summary>
		/// Создает кнопку с указанным изображением
		/// </summary>
		public Button(MultiSprite image)
			: base(image) {
			Animation = false;
			maxAnimCount = (int)animationMode;
			initPos = Position;
			initSize = Size;
		}
		public Button(Button g)
			: base(g) {
			Animation = false;
			animationCount = g.animationCount;
			initPos = g.initPos;
			initSize = g.initSize;
		}

		/// <summary>
		/// Возвращает кнопку в начальное состояние, заданное при создании
		/// </summary>
		public void ToNormal() {
			Position = initPos;
			Size = initSize;
			animationCount = 0;
			Animation = false;
		}

		/// <summary>
		/// Прорисовка кнопки в текущем кадре
		/// </summary>
		public override void Draw(SpriteBatch spriteBatch) {
			if (Visible) {
				if (Animation) {
					Position = (animationCount < maxAnimCount) ? Position - new Vector2(2.5f, 2.5f * Image.Size.Y / Image.Size.X) : Position;
					Size = (animationCount < maxAnimCount) ? Size + new Vector2(5, 5 * Image.Size.Y / Image.Size.X) : Size;
					animationCount++;
					if (animationCount > maxAnimCount)
						animationCount = maxAnimCount;
				} else {
					Position = (animationCount > 0) ? Position + new Vector2(2.5f, 2.5f * Image.Size.Y / Image.Size.X) : Position;
					Size = (animationCount > 0) ? Size - new Vector2(5, 5 * Image.Size.Y / Image.Size.X) : Size;
					animationCount--;
					if (animationCount < 0)
						animationCount = 0;
				}
				base.Draw(spriteBatch);
			}
		}

		public override object Clone() {
			return new Button(this);
		}

		#region Private Methods



		#endregion
	}

	public enum AnimationStates
	{
		Small = 3,
		Default = 7,
		Large = 11,
	}
}
