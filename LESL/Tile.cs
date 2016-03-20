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
	public struct Tile : IEquatable<Tile>
	{
		public Tiles TileType;
		public Color Color;

		public bool Equals(Tile other) {
			return TileType == other.TileType && Color == other.Color;
		}
	}
}
