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
	static class World
	{
		public static ContentManager Content;
		public static int TileW = 64;
		public static Rectangle Field = new Rectangle(0, 0, 5000, 4000);
		public static int CameraSpeed = 20;
	}
}
