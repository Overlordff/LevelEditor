using System;
using System.Collections.Generic;
using System.Linq;
using Graph;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using LESL;

namespace LevelEditor.Core
{
	public class Ground
	{
		Rectangle field;
		int tileW;
		Sprite[,] tiles;
		Vector2 drawPosition;
		int countX, countY;

		public Sprite[,] Sprites { get { return tiles; } }

		public int CountX { get { return countX; } }
		public int CountY { get { return countY; } }

		public Ground(Rectangle field, int tileW) {
			this.field = field;
			this.tileW = tileW;

			countX = field.Width / tileW + 1;
			countY = field.Height / tileW + 1;
			tiles = new Sprite[countX, countY];
			for (int i = 0; i < countY; i++) {
				for (int j = 0; j < countX; j++) {
					Sprite sprite = new Sprite(World.Content);
					sprite.LoadTexture(Fnames.NONE, new Vector2(j * tileW, i * tileW), new Vector2(tileW, tileW));
					sprite.Visible = false;
					tiles[j, i] = sprite;
				}
			}
		}

		public void LoadMap(Tile?[,] tiles) {
			int countY = tiles.GetLength(0);
			int countX = tiles.GetLength(1);
			for (int i = 0; i < countY; i++) {
				for (int j = 0; j < countX; j++) {
					if (tiles[i,j].HasValue)
						CastSprite(tiles[i, j].Value, this.tiles[i, j]);
					else
						this.tiles[i, j].LoadTexture(Fnames.NONE, new Vector2(j * tileW, i * tileW), new Vector2(tileW, tileW));
				}
			}
		}

		public Sprite GetSprite(Point index) {
			int row = index.Y;
			int column = index.X;
			return tiles[column, row];
		}

		public Sprite ClearSprite(Point index) {
			int row = index.Y;
			int column = index.X;
			tiles[column, row].LoadTexture(Fnames.NONE, new Vector2(column * tileW, row * tileW), new Vector2(tileW, tileW));
			return tiles[column, row];
		}

		public void Draw(SpriteBatch spriteBatch, Camera camera) {
			for (int i = 0; i < countY; i++) {
				for (int j = 0; j < countX; j++) {
					if (camera.Contains(new Rectangle(j * tileW, i * tileW, tileW, tileW))) {
						tiles[j, i].Visible = true;
						InternalDraw(j, i, camera);
						tiles[j, i].Draw(spriteBatch);
					} else
						tiles[j, i].Visible = false;
				}
			}
		}

		private void InternalDraw(int x, int y, Camera camera) {
			drawPosition = new Vector2(x * tileW - camera.Rect.X, y * tileW - camera.Rect.Y);
			tiles[x, y].Position = drawPosition;
		}

		public static void CastSprite(Tile tile, Sprite sprite) {
			Vector2 pos = sprite.Position;
			Vector2 size = sprite.Size;
			string fname = "";
			switch (tile.TileType) {

				#region Choose Tiles

				case Tiles.Dust:
					fname = Fnames.DUST;
					break;
				case Tiles.Grass1:
					fname = Fnames.GRASS1;
					break;
				case Tiles.Grass2:
					fname = Fnames.GRASS2;
					break;
				case Tiles.Grass3:
					fname = Fnames.GRASS3;
					break;
				case Tiles.DarkGrass:
					fname = Fnames.DARK_GRASS;
					break;
				case Tiles.GrassPath:
					fname = Fnames.GRASS_PATH;
					break;
				case Tiles.Snow:
					fname = Fnames.SNOW;
					break;
				case Tiles.Sand1:
					fname = Fnames.SAND1;
					break;
				case Tiles.Sand2:
					fname = Fnames.SAND2;
					break;
				case Tiles.Sand3:
					fname = Fnames.SAND3;
					break;
				case Tiles.Sand4:
					fname = Fnames.SAND4;
					break;
				case Tiles.Water1a:
					fname = Fnames.WATER1_A;
					break;
				case Tiles.Water1b:
					fname = Fnames.WATER1_B;
					break;
				case Tiles.Water1c:
					fname = Fnames.WATER1_C;
					break;
				case Tiles.Water1d:
					fname = Fnames.WATER1_D;
					break;
				case Tiles.Water2:
					fname = Fnames.WATER2;
					break;
				case Tiles.Water3a:
					fname = Fnames.WATER3_A;
					break;
				case Tiles.Water3b:
					fname = Fnames.WATER3_B;
					break;
				case Tiles.Water3c:
					fname = Fnames.WATER3_C;
					break;
				case Tiles.Ford1:
					fname = Fnames.FORD1;
					break;
				case Tiles.Road1:
					fname = Fnames.ROAD1;
					break;
				case Tiles.Road2:
					fname = Fnames.ROAD2;
					break;
				default:
					throw new ArgumentException("Встречен неизвестный тайл", "tile.TileType");

				#endregion

			}
			sprite.LoadTexture(fname, pos, size);
			sprite.Color = tile.Color;
		}
	}
}
