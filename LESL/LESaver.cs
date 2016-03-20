using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Graph;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace LESL
{
	public static class LESaver
	{
		static BinaryFormatter bf = new BinaryFormatter();

		public static bool SaveMap(MapInfo map) {
			if (!Directory.Exists(Global.MAP_DIR))
				Directory.CreateDirectory(Global.MAP_DIR);
			string[] fnames = Directory.GetFiles(Global.MAP_DIR, "map*.lsmap", SearchOption.TopDirectoryOnly);
			string fname = null;
			if (fnames.Length > 0) {
				for (int j = 0; j < Global.MAX_MAPS; j++) {
					bool isFind = false;
					for (int i = 0; i < fnames.Length; i++) {
						if (fnames[i].Length != 18)
							continue;
						string temp = fnames[i];
						int num;
						if (!int.TryParse(temp.Substring(8, 4), out num))
							continue;
						if (num == j) {
							isFind = true;
							break;
						}
					}
					if (!isFind) {
						fname = "map" + j.ToString().PadLeft(4, '0') + ".lsmap";
						break;
					}
				}
				if (fname == null)
					return false;
			} else
				fname = "map0000.lsmap";
			using (FileStream fs = File.Create(Path.Combine(Global.MAP_DIR,fname))) {
				bf.Serialize(fs, map);
				return true;
			}
		}
	}
}
