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
	/// Список элементов TexturePrev с возможностью переключения между измерениями,
	/// добавлением TexturePrev в указанное измерение и т.д.
	/// </summary>
	public class PrevList : Element
	{
		/// <summary>
		/// Максимальное кол-во измерений
		/// </summary>
		public const int MAX_DIMENSIONS = 9;

		#region Variables

		MultiSprite image2;

		List<List<TexturePrev>> list = new List<List<TexturePrev>>();
		List<TexturePrev> prevs = new List<TexturePrev>();

		TextSprite currentListNumberText;
		int currentListNumber = -1;
		ushort prevsInOneDim;

		Action<TexturePrev> formatMethod;

		#endregion

		/// <summary>
		/// Позиция элемента
		/// </summary>
		public override Vector2 Position {
			get {
				return base.Position;
			}
			set {
				Vector2 prev = base.Position;
				base.Position = value;
				if (prev != value) {
					image2.MoveOn(value - prev);
					currentListNumberText.MoveOn(value - prev);
				}
			}
		}
		/// <summary>
		/// Размер элемента
		/// </summary>
		public override Vector2 Size {
			get {
				return base.Size;
			}
			set {
				Vector2 prev = base.Size;
				base.Size = value;
				if (prev != value) {
					image2.Size = value;
					currentListNumberText.Size *= value / prev;
					currentListNumberText.Position = new Vector2(Image.Bounds.Right + 1, Image.Bounds.Top);
					image2.Position = new Vector2(Image.Position.X + Image.Size.X + currentListNumberText.Size.X + 2, Image.Position.Y);
				}
			}
		}
		/// <summary>
		/// Номер текущего списка элементов TexturePrev
		/// </summary>
		public int CurrentListNumber {
			get { return currentListNumber; }
			private set {
				if (value < 0)
					value = 0;
				if (value > list.Count - 1)
					value = list.Count - 1;
				currentListNumber = value;
				currentListNumberText.Text = (currentListNumber + 1).ToString();
			}
		}
		/// <summary>
		/// Общее кол-во списков элементов TexturePrev
		/// </summary>
		public int ListNumber { get { return list.Count; } }
		/// <summary>
		/// Максимальное кол-во элементов TexturePrev в одном измерении (всегда >= 1)
		/// </summary>
		public ushort PrevsInOneDim {
			get { return prevsInOneDim; }
			set {
				if (value < 1)
					value = 1;
				prevsInOneDim = value;
			}
		}
		public Action<TexturePrev> FormatMethod {
			get { return formatMethod; }
			set {
				formatMethod = value;
			}
		}

		/// <summary>
		/// Событие клика мышью по правой части элемента
		/// </summary>
		public event EventHandler<MouseElementEventArgs> ClickRight;
		/// <summary>
		/// Событие перемещения мыши по правой части элемента
		/// </summary>
		public event EventHandler<MouseElementEventArgs> MouseMoveRight;
		/// <summary>
		/// Событие выхода мыши за пределы правой части элемента
		/// </summary>
		public event EventHandler<MouseElementEventArgs> MouseMoveOutRight;
		/// <summary>
		/// Событие входа мыши в пределы правой части элемента
		/// </summary>
		public event EventHandler<MouseElementEventArgs> MouseMoveInRight;

		/// <summary>
		/// Создает новый элемента PrevList с указанными параметрами. 
		/// Правая часть скопируется с левой и отразится по горизонтали
		/// </summary>
		/// <param name="image">Изображение левой части элемента</param>
		/// <param name="content">Управляющий контентом</param>
		/// <param name="prevsInOneDim">Максимальное кол-во элементов TexturePrev в одном измерении</param>
		/// <param name="formatMethod">Метод, применяемый к каждому элементу TexturePrev при горизонтальном форматировании (вертикальное форматирование встроено)</param>
		public PrevList(MultiSprite image, ContentManager content, ushort prevsInOneDim, Action<TexturePrev> formatMethod)
			: base(image) {
			currentListNumberText = TextSprite.CreateSprite(content, Fnames.TEXT_FONT, new Vector2(image.Bounds.Right + 1, image.Bounds.Top), "");
			currentListNumberText.Position = new Vector2(image.Bounds.Right + 1, image.Bounds.Top);
			image2 = (MultiSprite)image.Clone();
			image2.SpriteEffect = SpriteEffects.FlipHorizontally;
			image2.MoveOn(new Vector2(image.Size.X + currentListNumberText.Font.MeasureString("0").X + 2, 0));
			list.Add(new List<TexturePrev>());
			CurrentListNumber = 0;
			PrevsInOneDim = prevsInOneDim;
			FormatMethod = formatMethod;

			this.Click += new EventHandler<MouseElementEventArgs>(PrevList_Click);
			this.ClickRight += new EventHandler<MouseElementEventArgs>(PrevList_ClickRight);
		}
		public PrevList(PrevList g)
			: base(g) {
			formatMethod = g.formatMethod;
			image2 = (MultiSprite)g.image2.Clone();
			currentListNumber = g.currentListNumber;
			currentListNumberText = (TextSprite)g.currentListNumberText.Clone();
			list = new List<List<TexturePrev>>();
			for (int i = 0; i < g.list.Count; i++) {
				AddDimension();
				foreach (var prev in g.list[i]) {
					list[i].Add((TexturePrev)prev.Clone());
				}
			}
			prevs = new List<TexturePrev>();
			foreach (var prev in g.prevs) {
				prevs.Add((TexturePrev)prev.Clone());
			}
			prevsInOneDim = g.prevsInOneDim;
			currentListNumber = g.currentListNumber;
		}

		public void AddPrev(TexturePrev prev) {
			if (MAX_DIMENSIONS * PrevsInOneDim == prevs.Count)
				throw new InvalidOperationException("Попытка превысить предел кол-ва элементов TexturePrev");
			prevs.Add(prev);
		}

		public void RemovePrev(TexturePrev prev) {
			prevs.Remove(prev);
		}
		public void RemovePrev() {
			if (prevs.Count > 0)
				prevs.RemoveAt(prevs.Count - 1);
		}

		/// <summary>
		/// Обновление элемента в текущем кадре
		/// </summary>
		public override void Update(GameTime gameTime) {
			Format();
			if (currentListNumber >= list.Count)
				CurrentListNumber = list.Count - 1;
			foreach (var dim in list) {
				foreach (var prev in dim) {
					prev.Visible = false;
				}
			}
			if (list.Count > 0)
				foreach (var prev in list[currentListNumber]) {
					prev.Visible = true;
				}
			foreach (var dim in list) {
				foreach (var prev in dim) {
					prev.Update(gameTime);
				}
			}

			base.Update(gameTime);
		}

		/// <summary>
		/// Визуализация элемента в текущем кадре
		/// </summary>
		public override void Draw(SpriteBatch spriteBatch) {
			if (Visible) {
				base.Draw(spriteBatch);
				currentListNumberText.Draw(spriteBatch);
				image2.Draw(spriteBatch);
				foreach (var prev in list[currentListNumber]) { //MAYBE TODO корректная работа при удалении всех превов в рантайме (приводит к удалению всех измерений)
					prev.Draw(spriteBatch);
				}
			}
		}

		public override object Clone() {
			return new PrevList(this);
		}

		protected override void MouseHandler(MouseState ms, MouseState oms) {
			base.MouseHandler(ms, oms);
			if (Visible) {
				if (image2.Bounds.Contains(new Point(ms.X, ms.Y))) {
					OnMouseMoveRight(new MouseElementEventArgs(new Vector2(ms.X, ms.Y), ms));
					if (ms.LeftButton == ButtonState.Pressed && oms.LeftButton == ButtonState.Released) {
						OnClickRight(new MouseElementEventArgs(new Vector2(ms.X, ms.Y), ms));
					}
				}
				if (image2.Bounds.Contains(new Point(oms.X, oms.Y)) && !image2.Bounds.Contains(ms.X, ms.Y))
					OnMouseMoveOutRight(new MouseElementEventArgs(new Vector2(ms.X, ms.Y), ms));
				if (image2.Bounds.Contains(new Point(ms.X, ms.Y)) && !image2.Bounds.Contains(oms.X, oms.Y))
					OnMouseMoveInRight(new MouseElementEventArgs(new Vector2(ms.X, ms.Y), ms));
			}
		}

		#region Private Methods

		/// <summary>
		/// Добавляет новое измерение. Если кол-во измерений до добавления == MAX_DIMENSIONS,
		/// то выбросит исключение InvalidOperationException
		/// </summary>
		private void AddDimension() {
			if (list.Count == MAX_DIMENSIONS)
				throw new InvalidOperationException("Попытка превысить допустимое кол-во измерений");
			list.Add(new List<TexturePrev>());
		}

		private void RemoveDimension(int index) {
			if (index < 0 || index >= list.Count)
				throw new ArgumentOutOfRangeException("Индекс находится вне диапазона допустимых значений (попытка удалить несуществующее измерение)");
			list.RemoveAt(index);
		}
		private void RemoveDimension() {
			RemoveDimension(list.Count - 1);
		}

		private void Format() {
			int count = 0;
			foreach (var dim in list) {
				dim.Clear();
			}
			list.Clear();
			AddDimension();
			for (int i = 0; i < prevs.Count; i++) {
				if (count++ > prevsInOneDim - 1) {
					count = 1;
					AddDimension();
				}
				list[list.Count - 1].Add(prevs[i]);
				formatMethod(prevs[i]);
				FormatY(prevs[i], count);
			}
			for (int i = list.Count - 1; i >= 0; i--) {
				if (list[i].Count == 0)
					RemoveDimension();
			}
		}

		private void FormatY(TexturePrev prev, int count) {
			prev.Position = new Vector2(prev.Position.X, list[0][count - 1].Position.Y);
		}

		private void PrevList_Click(object sender, MouseElementEventArgs e) {
			CurrentListNumber--;
		}

		private void PrevList_ClickRight(object sender, MouseElementEventArgs e) {
			CurrentListNumber++;
		}

		private void OnClickRight(MouseElementEventArgs e) {
			EventHandler<MouseElementEventArgs> clickRight = ClickRight;
			if (clickRight != null)
				clickRight(this, e);
		}

		private void OnMouseMoveRight(MouseElementEventArgs e) {
			EventHandler<MouseElementEventArgs> mouseMoveRight = MouseMoveRight;
			if (mouseMoveRight != null)
				mouseMoveRight(this, e);
		}

		private void OnMouseMoveInRight(MouseElementEventArgs e) {
			EventHandler<MouseElementEventArgs> mouseMoveInRight = MouseMoveInRight;
			if (mouseMoveInRight != null)
				mouseMoveInRight(this, e);
		}

		private void OnMouseMoveOutRight(MouseElementEventArgs e) {
			EventHandler<MouseElementEventArgs> mouseMoveOutRight = MouseMoveOutRight;
			if (mouseMoveOutRight != null)
				mouseMoveOutRight(this, e);
		}

		#endregion
	}
}
