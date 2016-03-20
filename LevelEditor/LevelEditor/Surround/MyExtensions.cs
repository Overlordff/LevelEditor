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

namespace LevelEditor
{
	public static class MyExtensions
	{
		/// <summary>
		/// Возвращает новый вектор с округленными компонентами
		/// </summary>
		public static Vector2 Round(this Vector2 v) {
			return new Vector2((float)Math.Round(v.X), (float)Math.Round(v.Y));
		}
		/// <summary>
		/// Форматирует вектор в соответствии с окном текущей игры
		/// </summary>
		public static Vector2 Format(this Vector2 v, Game game) {
			return new Vector2(v.X * (float)game.Window.ClientBounds.Width / 1280f, v.Y * (float)game.Window.ClientBounds.Height / 1024f);
		}

		public static float FormatW(this float width, Game game) {
			return width * (float)game.Window.ClientBounds.Width / 1280f;
		}

		public static float FormatH(this float height, Game game) {
			return height * (float)game.Window.ClientBounds.Height / 1024f;
		}

		public static Tile ToTile(this TexturePrev prev) {
			Tile result;
			result.TileType = prev.TileType;
			result.Color = prev.Image.Color;
			return result;
		}

		public static Stack<T> ToStack<T>(this IEnumerable<T> source) {
			Stack<T> result = new Stack<T>();
			foreach (var item in source) {
				result.Push(item);
			}
			return result;
		}
	}
}
