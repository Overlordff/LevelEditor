using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Graph;
using LevelEditor.Core;
using LESL;

namespace LevelEditor
{
	public class EditorScreen : Screen
	{
		#region Structures

		struct Backup
		{
			public Point[] indexes;
			public Sprite[] sprites;
			public Tile?[] tiles;
		}

		struct BufferBackup
		{
			public List<Point> indexes;
			public List<Sprite> sprites;
			public List<Tile?> tiles;

			public bool IsDisposed {
				get {
					return indexes == null || sprites == null || tiles == null;
				}
			}

			public bool IsChanged { get; private set; }

			public void Initialize() {
				indexes = new List<Point>();
				sprites = new List<Sprite>();
				tiles = new List<Tile?>();
				IsChanged = false;
			}

			public void AddChange(Point index, Tile? tile, Sprite sprite) {
				indexes.Add(index);
				sprites.Add(sprite);
				tiles.Add(tile);
				IsChanged = true;
			}

			public void Clear() {
				indexes.Clear();
				sprites.Clear();
				tiles.Clear();
				IsChanged = false;
			}
		}

		#endregion

		const int MAX_CHANGES = 10;

		#region Variables

		Tile?[,] tiles;
		Tile? CurrentTile;

		Stack<Backup> backups = new Stack<Backup>(MAX_CHANGES);
		BufferBackup bufferBackup;

		MouseStates State = MouseStates.Normal;
		bool lb, rb;
		bool ctrl;

		UIPanel panel;
		Minimap minimap;
		Rectangle mainPanelRect;
		Sprite mainPanelSprite;
		Ground ground;
		Camera camera;
		int cameraSpeed = World.CameraSpeed;
		RenderTarget2D render;		

		SpriteFont font;
		
		#endregion

		public event EventHandler<SpriteChangedEventArgs> SpriteChanged;

		public EditorScreen(Game1 game, int tileW, Rectangle field)
			: base(game) {
			CheckField(ref field);
			CheckTileW(ref tileW);
			World.Field = new Rectangle(0, 0, field.Width, field.Height);
			World.TileW = tileW;
			Initialize();
		}

		protected override void Initialize() {
			mainPanelRect = new Rectangle(18, 18, 1055, 780);
			bufferBackup.Initialize();
			base.Initialize();
		}

		protected override void LoadContent() {
			font = Content.Load<SpriteFont>(Fnames.TEXT_FONT);

			mainPanelSprite = Sprite.CreateSprite(Content, Fnames.MAIN_PANEL, new Vector2(mainPanelRect.X, mainPanelRect.Y), new Vector2(mainPanelRect.Width, mainPanelRect.Height));
			ground = new Ground(World.Field, World.TileW);
			camera = new Camera(World.Field, mainPanelRect, GraphicsDevice.Viewport.Bounds);
			render = new RenderTarget2D(Game.GraphicsDevice, camera.Viewport.Width, camera.Viewport.Height);
			tiles = new Tile?[ground.CountX, ground.CountY];
			tiles.Initialize();

			panel = new UIPanel(this, Sprite.CreateSprite(Content, Fnames.PANEL, Vector2.Zero, new Vector2(1280, 1024)));
			panel.Visible = true;
			panel.ChoseTile += new EventHandler<TileEventArgs>(panel_ChoseTile);
			panel.SaveCalled += new EventHandler<EventArgs>(panel_SaveCalled);
			panel.LoadCalled += new EventHandler<EventArgs>(panel_LoadCalled);
			panel.ClearCalled += new EventHandler<EventArgs>(panel_ClearCalled);
			panel.FillCalled += new EventHandler<TileEventArgs>(panel_FillCalled);
			panel.ExitCalled += new EventHandler<EventArgs>(panel_ExitCalled);

			minimap = new Minimap(Game, this, World.Field, panel.MapRect, ground.Sprites, World.TileW);
			minimap.Click += new EventHandler<MinimapMouseEventArgs>(minimap_Click);

			Components.Add(panel);
		}

		protected override void UnloadContent() {

		}

		public override void Update(GameTime gameTime) {
			OMS = MS;
			MS = Mouse.GetState();
			OKBS = KBS;
			KBS = Keyboard.GetState();
			MouseHandler(MS, OMS, State);
			KeyboardHandler(KBS, OKBS);
			minimap.Update(gameTime, camera);
			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime) {
			Game.GraphicsDevice.SetRenderTarget(render);
			Game.GraphicsDevice.Clear(Color.CornflowerBlue);
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			ground.Draw(spriteBatch, camera);
			spriteBatch.End();
			Game.GraphicsDevice.SetRenderTarget(null);

			GraphicsDevice.Clear(Color.Gray);

			spriteBatch.Begin();
			spriteBatch.Draw(render, camera.Viewport, Color.White);
			mainPanelSprite.Draw(spriteBatch);
			base.Draw(gameTime);
			minimap.Draw(spriteBatch);

			spriteBatch.End();
		}

		protected override void KeyboardHandler(KeyboardState kbs, KeyboardState okbs) {
			if (kbs.IsKeyDown(Keys.Enter) && !okbs.IsKeyDown(Keys.Enter)) {
				GC.Collect();
			}
			if (kbs.IsKeyDown(Keys.Left)) camera.MoveOn(new Vector2(-cameraSpeed, 0));
			if (kbs.IsKeyDown(Keys.Right)) camera.MoveOn(new Vector2(cameraSpeed, 0));
			if (kbs.IsKeyDown(Keys.Up)) camera.MoveOn(new Vector2(0, -cameraSpeed));
			if (kbs.IsKeyDown(Keys.Down)) camera.MoveOn(new Vector2(0, cameraSpeed));

			if (kbs.IsKeyDown(Keys.LeftControl)) ctrl = true;
			if (kbs.IsKeyUp(Keys.LeftControl)) ctrl = false;

			if (ctrl) {
				if (kbs.IsKeyDown(Keys.Z) && !okbs.IsKeyDown(Keys.Z))
					if (backups.Count != 0)
						BackUp(backups.Pop());
			}
			if (kbs.IsKeyDown(Keys.R) && !okbs.IsKeyDown(Keys.R)) {
				RenderMinimap(ground.Sprites);
			}
			if (kbs.IsKeyDown(Keys.Space) && !okbs.IsKeyDown(Keys.Space)) {
				Point index = FromPosToIndex(MousePosition(MS), World.TileW);
				panel.SetCurrentPrev(tiles[index.X, index.Y]);
			}

			#region Сохранение выбранного тайла в .png с добавлением оттенка. Путь = "Customs\*.png". (DEBUG only)

#if DEBUG	//ctrl + S
			if (ctrl) {
				if (kbs.IsKeyDown(Keys.S) && !okbs.IsKeyDown(Keys.S)) {
					if (CurrentTile != null && CurrentTile.HasValue) {
						Tile ctile = CurrentTile.Value;
						Texture2D texture = new Texture2D(GraphicsDevice, World.TileW, World.TileW);
						Color[] data = new Color[World.TileW * World.TileW];
						Sprite sprite = new Sprite(Content);
						Ground.CastSprite(ctile, sprite);
						sprite.GetTexture().GetData<Color>(data);
						Color tint = ctile.Color;
						long fnameGen = 0; //Имя генерируется исходя из цвета текстуры
						for (int i = 0; i < data.Length; i++) {
							data[i] = new Color(data[i].R * tint.R / 255, data[i].G * tint.G / 255, data[i].B * tint.B / 255, data[i].A);
							fnameGen += (long)data[i].R + (long)data[i].G + (long)data[i].B;
						}
						texture.SetData<Color>(data);
						string dir = @"Customs";
						string fname = fnameGen.ToString() + ".png";
						if (!Directory.Exists(dir))
							Directory.CreateDirectory(dir);
						if (File.Exists(Path.Combine(dir, fname)))
							throw new InvalidOperationException("Имя совпало");
						using (FileStream fs = File.Create(Path.Combine(dir,fname))) {
							texture.SaveAsPng(fs, World.TileW, World.TileW);
						}
					}
				}			
			}
#endif

			#endregion

		}

		protected override void MouseHandler(MouseState ms, MouseState oms, MouseStates state) {
			if (ms.LeftButton == ButtonState.Pressed && oms.LeftButton == ButtonState.Released && mainPanelRect.Contains(new Point(ms.X, ms.Y)) && !rb) {
				lb = true;
				bufferBackup.Clear();
			}
			if (ms.RightButton == ButtonState.Pressed && oms.RightButton == ButtonState.Released && mainPanelRect.Contains(new Point(ms.X, ms.Y)) && !lb) {
				rb = true;
				bufferBackup.Clear();
			}
			if (ms.LeftButton == ButtonState.Released && oms.LeftButton == ButtonState.Pressed) {
				lb = false;
				if (mainPanelRect.Contains(new Point(ms.X, ms.Y))) {
					if (!bufferBackup.IsDisposed && bufferBackup.IsChanged) {
						AddBackup(bufferBackup);
					}
				}
			}
			if (ms.RightButton == ButtonState.Released && oms.RightButton == ButtonState.Pressed) {
				rb = false;
				if (mainPanelRect.Contains(new Point(ms.X, ms.Y))) {
					if (!bufferBackup.IsDisposed && bufferBackup.IsChanged) {
						AddBackup(bufferBackup);
					}
				}
			}
			if (lb && !rb) {
				if (ms.LeftButton == ButtonState.Pressed && mainPanelRect.Contains(new Point(ms.X, ms.Y))) {
					if (CurrentTile != null) {
						Point _index;  //}
						Tile? tile;    //}previous values
						Sprite sprite; //}

						Point index = FromPosToIndex(MousePosition(ms), World.TileW);
						Sprite temp = ground.GetSprite(index);
						Sync(CurrentTile, temp, index, out tile, out sprite, out _index);

						if (tile.HasValue != CurrentTile.HasValue || tile.HasValue && CurrentTile.HasValue && !tile.Value.Equals(CurrentTile.Value))
							bufferBackup.AddChange(_index, tile, sprite);
					}
				}
			}
			if (rb && !lb) {
				if (ms.RightButton == ButtonState.Pressed && mainPanelRect.Contains(new Point(ms.X, ms.Y))) {
					Point _index;  //}
					Tile? tile;    //}previous values
					Sprite sprite; //}

					Point index = FromPosToIndex(MousePosition(ms), World.TileW);
					Sync(index, out tile, out sprite, out _index);

					if (tile != null && tiles[index.X, index.Y] == null)
						bufferBackup.AddChange(_index, tile, sprite);
				}
			}
		}

		#region Private Methods

		#region Sync

		private void Sync(Tile? tile, Sprite sprite, Point index, out Tile? prevTile, out Sprite prevSprite, out Point prevIndex) {
			prevSprite = sprite;
			prevTile = tiles[index.X, index.Y];
			prevIndex = index;
			if (tile == null) {
				Sync(index, out prevTile, out prevSprite, out prevIndex);
				return;
			}
			//prevSprite = sprite;
			//prevTile = tiles[index.X, index.Y];
			//prevIndex = index;

			tiles[index.X, index.Y] = tile;
			Ground.CastSprite(tile.Value, sprite);
			OnSpriteChanged(new SpriteChangedEventArgs(index, sprite));
		}
		private void Sync(Point index, out Tile? prevTile, out Sprite prevSprite, out Point prevIndex) {
			prevSprite = ground.GetSprite(index);
			prevTile = tiles[index.X, index.Y];
			prevIndex = index;

			tiles[index.X, index.Y] = null;
			ground.ClearSprite(index);
			OnSpriteChanged(new SpriteChangedEventArgs(index, ground.GetSprite(index)));
		}

		private void SyncWithoutBackup(Tile? tile, Sprite sprite, Point index) {
			if (tile == null) {
				SyncWithoutBackup(index);
				return;
			}
			tiles[index.X, index.Y] = tile;
			Ground.CastSprite(tile.Value, sprite);
			OnSpriteChanged(new SpriteChangedEventArgs(index, sprite));
		}
		private void SyncWithoutBackup(Point index) {
			tiles[index.X, index.Y] = null;
			ground.ClearSprite(index);
			OnSpriteChanged(new SpriteChangedEventArgs(index, ground.GetSprite(index)));
		}

		#endregion

		#region Backup Methods

		private void BackUp(Backup backup) {
			for (int i = 0; i < backup.indexes.Length; i++) {
				SyncWithoutBackup(backup.tiles[i], backup.sprites[i], backup.indexes[i]);
			}
		}

		private void AddBackup() {
			int countX = tiles.GetLength(0);
			int countY = tiles.GetLength(1);
			bufferBackup.Initialize();
			//Point[] prevIndexes = new Point[countX * countY];
			//Tile?[] prevTiles = new Tile?[countX * countY];
			//Sprite[] prevSprites = new Sprite[countX * countY];
			int count = 0;
			for (int i = 0; i < tiles.GetLength(0); i++) {
				for (int j = 0; j < tiles.GetLength(1); j++) {
					//prevIndexes[count] = new Point(i, j);
					//prevTiles[count] = tiles[i, j];
					//prevSprites[count] = ground.GetSprite(new Point(i, j));
					bufferBackup.AddChange(new Point(i, j), tiles[i, j], ground.GetSprite(new Point(i, j)));
					count++;
				}
			}
			//AddBackup(prevIndexes, prevSprites, prevTiles);
			AddBackup(bufferBackup);
		}
		private void AddBackup(Point[] indexes, Sprite[] sprites, Tile?[] tiles) {
			if (indexes.Length != sprites.Length || indexes.Length != tiles.Length || sprites.Length != tiles.Length)
				throw new ArgumentException("Длина всех трех массивов должна быть одинакова");
			Backup backup;
			backup.indexes = indexes;
			backup.sprites = sprites;
			backup.tiles = tiles;
			backups.Push(backup);
			CheckStackOfBackups(MAX_CHANGES);
		}
		private void AddBackup(BufferBackup buffer) {
			Point[] indexes = buffer.indexes.ToArray();
			Tile?[] tiles = buffer.tiles.ToArray();
			Sprite[] sprites = buffer.sprites.ToArray();
			AddBackup(indexes, sprites, tiles);
		}

		private void DisposeBackup(Backup backup) {
			backup.indexes = null;
			backup.tiles = null;
			backup.sprites = null;
		}

		/// <summary>
		/// Return True if stack of backups need to be cut and do it
		/// </summary>
		/// <param name="length">Max length that stack must be</param>
		private bool CheckStackOfBackups(int length) {
			if (backups.Count <= length || length < 0)
				return false;
			Stack<Backup> temp = new Stack<Backup>(length);
			for (int i = 0; i < length; i++) {
				temp.Push(backups.Pop());
			}
			backups.Clear();
			backups = new Stack<Backup>(length);
			for (int i = 0; i < length; i++) {
				backups.Push(temp.Pop());
			}
			return true;
		}

		private void ClearBackups() {
			backups = new Stack<Backup>();
			bufferBackup.Initialize();
		}

		#endregion

		#region Panel Handlers

		private void panel_ChoseTile(object sender, TileEventArgs e) {
			CurrentTile = e.Tile;
		}

		private void panel_ExitCalled(object sender, EventArgs e) {
			this.Game.Exit();
		}

		private void panel_SaveCalled(object sender, EventArgs e) {
			LESaver.SaveMap(new MapInfo(World.Field, tiles, World.TileW));
		}

		private void panel_LoadCalled(object sender, EventArgs e) {
			ClearBackups();
			MapInfo map;
			if (LELoader.Load(LoadModes.Top, out map) == 0) {
				World.Field = map.Field;
				World.TileW = map.TileW;
				ground = new Ground(map.Field, map.TileW);
				tiles = map.Tiles;
				ground.LoadMap(tiles);
				minimap = new Minimap(Game, this, map.Field, panel.MapRect, ground.Sprites, map.TileW);
				minimap.Click += new EventHandler<MinimapMouseEventArgs>(minimap_Click);
				RenderMinimap(ground.Sprites);
			} else {
				//TODO Обработка кода ошибки
			}
		}

		private void panel_ClearCalled(object sender, EventArgs e) {
			int countX = tiles.GetLength(0);
			int countY = tiles.GetLength(1);
			AddBackup(); //TODO или сделать нормальный бекап или сделать отчистку стека бекапов и буфера
			//ClearBackups(); //
			for (int i = 0; i < countX; i++) {
				for (int j = 0; j < countY; j++) {
					tiles[i, j] = null;
				}
			}
			ground.LoadMap(tiles);
			RenderMinimap(ground.Sprites);
		}

		private void panel_FillCalled(object sender, TileEventArgs e) {
			if (e.Tile.HasValue) {
				AddBackup(); //TODO или сделать нормальный бекап или сделать отчистку стека бекапов и буфера
				//ClearBackups(); //
				Tile tile = e.Tile.Value;
				for (int i = 0; i < tiles.GetLength(0); i++) {
					for (int j = 0; j < tiles.GetLength(1); j++) {
						tiles[i, j] = tile;
					}
				}
				ground.LoadMap(tiles);
				RenderMinimap(ground.Sprites);
			} else throw new InvalidOperationException("Тайл должен иметь значение");
		}

		#endregion

		#region Minimap Methods

		private void RenderMinimap(Sprite[,] sprites) {
			Color[,] mask = new Color[sprites.GetLength(0), sprites.GetLength(1)];
			for (int i = 0; i < sprites.GetLength(0); i++) {
				for (int j = 0; j < sprites.GetLength(1); j++) {
					mask[i, j] = sprites[i, j].Color;
				}
			}
			minimap.RenderMinimap(ground.Sprites, mask, World.TileW);
		}

		private void minimap_Click(object sender, MinimapMouseEventArgs e) {
			if (e.MouseState.LeftButton == ButtonState.Pressed) {
				camera = minimap.SetCamera(e.Position);
				return;
			}
		}

		#endregion

		/// <summary>
		/// Позиция мыши относительно мира, а не левого верхнего угла окна
		/// </summary>
		/// <param name="ms">Текущее состояние мыши</param>
		private Point MousePosition(MouseState ms) {
			return new Point(ms.X - camera.Viewport.X + camera.Rect.X, ms.Y - camera.Viewport.Y + camera.Rect.Y);
		}

		private Point FromPosToIndex(Point position, int width) {
			int x = position.X;
			int y = position.Y;
			int row = y / width; //+ ((y % width == 0) ? 0 : 1) - ((y == 0) ? 0 : 1);
			int column = x / width; //+ ((x % width == 0) ? 0 : 1) - ((x == 0) ? 0 : 1);
			return new Point(column, row);
		}

		private void CheckTileW(ref int tileW) {
			if (tileW > 256)
				tileW = 256;
			if (tileW < 16)
				tileW = 16;
		}

		private void CheckField(ref Rectangle field) {
			if (field.Width < 1280)
				field.Width = 1280;
			if (field.Height < 1024)
				field.Height = 1024;
		}

		private void OnSpriteChanged(SpriteChangedEventArgs e) {
			EventHandler<SpriteChangedEventArgs> spriteChanged = SpriteChanged;
			if (spriteChanged != null)
				spriteChanged(this, e);
		}

		#endregion
	}
}
