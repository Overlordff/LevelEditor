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

namespace LESL
{
	[Serializable]
	public struct MapInfo
	{
		public readonly Rectangle Field;
		public readonly Tile?[,] Tiles;
		public readonly int TileW;

		public MapInfo(Rectangle field, Tile?[,] tiles, int tileW) {
			this.Field = field;
			this.Tiles = tiles;
			this.TileW = tileW;
		}
	}
}
