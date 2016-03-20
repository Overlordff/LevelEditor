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
using LevelEditor.Core;

namespace LevelEditor
{
	public class Minimap
	{
		#region Variables

		Sprite[,] tiles;
		Texture2D back;
		Rectangle map;
		Rectangle field;
		Vector2 scaleFactor;

		Camera camera;

		int countY;
		int countX;
		int tileW;

		MouseState ms, oms;
		bool lb, rb;

		SpriteBatch spriteBatch;

		static Texture2D texture = World.Content.Load<Texture2D>(Fnames.MESH);
		static Texture2D rect = World.Content.Load<Texture2D>(Fnames.MINIMAP_CAMERA_RECT);

		#endregion

		public GraphicsDevice GraphicsDevice { get; set; }

		/// <summary>
		/// Событие щелчка мыши
		/// </summary>
		public event EventHandler<MinimapMouseEventArgs> Click;

		public Minimap(Game game, EditorScreen screen, Rectangle field, Rectangle map, Sprite[,] sprites, int tileW) {
			GraphicsDevice = game.GraphicsDevice;
			this.map = map;
			this.field = field;
			scaleFactor = new Vector2((float)map.Width / (float)field.Width, (float)map.Height / (float)field.Height);
			countY = sprites.GetLength(0);
			countX = sprites.GetLength(1);
			this.tiles = sprites;
			screen.SpriteChanged += new EventHandler<SpriteChangedEventArgs>(screen_SpriteChanged);
			this.tileW = tileW;
			LoadContent();
		}

		protected void LoadContent() {
			spriteBatch = new SpriteBatch(GraphicsDevice);
			Color[,] mask = new Color[tiles.GetLength(0), tiles.GetLength(1)];
			for (int i = 0; i < tiles.GetLength(0); i++) {
				for (int j = 0; j < tiles.GetLength(1); j++) {
					mask[i, j] = tiles[i, j].Color;
				}
			}
			RenderMinimap(tiles, mask, tileW);
		}

		public void Update(GameTime gameTime, Camera camera) {
			this.camera = camera;
			oms = ms;
			ms = Mouse.GetState();
			Vector2 pos = new Vector2(ms.X - map.X, ms.Y - map.Y);
			if (ms.LeftButton == ButtonState.Pressed && oms.LeftButton == ButtonState.Released && map.Contains(new Point(ms.X, ms.Y))) {
				lb = true;
				pos *= new Vector2(field.Width / map.Width, field.Height / map.Height);
				OnClick(new MinimapMouseEventArgs(pos, ms));
			}
			if (ms.RightButton == ButtonState.Pressed && oms.RightButton == ButtonState.Released && map.Contains(new Point(ms.X, ms.Y))) {
				rb = true;
			}
			if (ms.LeftButton == ButtonState.Released && oms.LeftButton == ButtonState.Pressed) {
				lb = false;
			}
			if (ms.RightButton == ButtonState.Released && oms.RightButton == ButtonState.Pressed) {
				rb = false;
			}
			if (lb && oms.LeftButton == ButtonState.Pressed) {
				if (map.Contains(new Point(ms.X, ms.Y))) {
					pos *= new Vector2(field.Width / map.Width, field.Height / map.Height);
					OnClick(new MinimapMouseEventArgs(pos, ms));
				}
			}
		}

		public void Draw(SpriteBatch spriteBatch) {
			if (back != null)
				spriteBatch.Draw(back, map, Color.White);
			spriteBatch.Draw(rect, new Rectangle((int)(map.X + camera.Rect.X * scaleFactor.X), (int)(map.Y + camera.Rect.Y * scaleFactor.Y), map.Width * camera.Rect.Width / field.Width + 1, map.Height * camera.Rect.Height / field.Height + 1), new Color(20, 20, 20));
		}

		public void RenderMinimap(Sprite[,] sprites, Color[,] colorMask, int tileW) {
			back = RenderInOneTexture(sprites, colorMask, tileW);
		}

		public Camera SetCamera(Vector2 position) {
			Camera result = new Camera(World.Field, camera.Viewport, GraphicsDevice.Viewport.Bounds);
			result.Rect = new Rectangle((int)position.X - result.Rect.Width / 2, (int)position.Y - result.Rect.Height / 2, result.Rect.Width, result.Rect.Height);
			return result;
		}

		#region Private Methods

		/// <summary>
		/// Переводит текстуры из массива в двумерный набор усредненных цветов, затем
		/// рендерит этот набор цветов на текстуру back
		/// </summary>
		private Texture2D RenderInOneTexture(Sprite[,] sprites, Color[,] colorMask, int tileW) {
			if (sprites.Length == 0)
				throw new ArgumentException("Длина массива спрайта должна быть больше 0");
			if (sprites.GetLength(0) != colorMask.GetLength(0) || sprites.GetLength(1) != colorMask.GetLength(1))
				throw new ArgumentException("Сетка спрайтов и цветовая маска должны совпадать по размерам во всех измерениях.");
			int w = tileW;
			int h = tileW;
			int m = field.Width / w + 1; //"+ 1" can be bug
			int n = field.Height / h + 1; //"+ 1" can be bug
			Vector2 scale = new Vector2((float)map.Width / (float)field.Width, (float)map.Height / (float)field.Height);
			Color[,] newColors = new Color[m, n];
			for (int i = 0; i < m; i++) {
				for (int j = 0; j < n; j++) {
					Color[] c = new Color[sprites[0, 0].Width * sprites[0, 0].Height];
					sprites[i, j].GetTexture().GetData<Color>(c);
					newColors[i, j] = AverrageColor(c);
					newColors[i, j] = TintTo(newColors[i, j], colorMask[i, j]);
				}
			}
			RenderTarget2D resultTexture = new RenderTarget2D(GraphicsDevice, map.Width, map.Height);
			GraphicsDevice.SetRenderTarget(resultTexture);
			GraphicsDevice.Clear(Color.Black);
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			for (int i = 0; i < m; i++) {
				for (int j = 0; j < n; j++) {
					spriteBatch.Draw(texture, new Vector2(i * w * scale.X, j * h * scale.Y), null, newColors[i, j], 0, Vector2.Zero, scale * World.TileW, SpriteEffects.None, 0);
				}
			}
			spriteBatch.End();
			GraphicsDevice.SetRenderTarget(null);
			return resultTexture;
		}

		private Color AverrageColor(Color[] colors) {
			float r, g, b;
			r = g = b = 0;
			int l = colors.Length;
			for (int i = 0; i < l; i++) {
				r += colors[i].R;
				g += colors[i].G;
				b += colors[i].B;
			}
			r /= l;
			g /= l;
			b /= l;
			return new Color((int)r, (int)g, (int)b);
		}

		private Color TintTo(Color src, Color tint) {
			return new Color(src.R * tint.R / 255, src.G * tint.G / 255, src.B * tint.B / 255, src.A);
		}

		private void screen_SpriteChanged(object sender, SpriteChangedEventArgs e) {
			RenderTarget2D render = new RenderTarget2D(GraphicsDevice, map.Width, map.Height);
			GraphicsDevice.SetRenderTarget(render);
			GraphicsDevice.Clear(Color.Black);
			spriteBatch.Begin();
			if (back != null)
				spriteBatch.Draw(back, new Rectangle(0, 0, map.Width, map.Height), Color.White);
			//int w = e.Sprite.Width;
			//int h = e.Sprite.Height;
			int w = tileW;
			int h = tileW;
			Color[] c = new Color[e.Sprite.Width * e.Sprite.Height];
			e.Sprite.GetTexture().GetData<Color>(c);
			Color newColor = AverrageColor(c);
			newColor = TintTo(newColor, e.Sprite.Color);
			spriteBatch.Draw(texture, new Vector2(e.Index.X * w * scaleFactor.X, e.Index.Y * h * scaleFactor.Y), null, newColor, 0, Vector2.Zero, scaleFactor * World.TileW, SpriteEffects.None, 0);
			spriteBatch.End();
			GraphicsDevice.SetRenderTarget(null);
			back = render;
		}

		/// <summary>
		/// Позиция мыши относительно мира, а не левого верхенго угла окна
		/// </summary>
		/// <param name="ms">Текущее состояние мыши</param>
		private Point MousePosition(MouseState ms) {
			return new Point(ms.X - camera.Viewport.X + camera.Rect.X, ms.Y - camera.Viewport.Y + camera.Rect.Y);
		}

		private void OnClick(MinimapMouseEventArgs e) {
			EventHandler<MinimapMouseEventArgs> click = Click;
			if (click != null)
				click(this, e);
		}

		#endregion
	}
}
