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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace LESL
{
	/// <summary>
	/// Класс загрузчик
	/// </summary>
	public class LELoader
	{
		static BinaryFormatter bf = new BinaryFormatter();
		static Random random = new Random(DateTime.Now.Millisecond);

		/// <summary>
		/// Загружает карту из директории MAP_DIR, используя указанный мод загрузки.
		/// Возвращает код завершения:
		/// 0 - успех.
		/// 1 - Директория не найдена.
		/// 2 - Операция не может быть совершена, т.к. в директории нет файлов карты нужного формата.
		/// 3 - Файл карты не найден.
		/// 4 - Встречен неопределенный мод загрузки.
		/// </summary>
		/// <param name="mode">Мод загрузки</param>
		/// <param name="result">Записывает карту в указанную переменную</param>
		public static int Load(LoadModes mode, out MapInfo result) {
			if (!Directory.Exists(Global.MAP_DIR)) {
				//throw new DirectoryNotFoundException("Директория " + Global.MAP_DIR + " не найдена.");
				result = new MapInfo();
				return 1;
			}
			string[] fnames = Directory.GetFiles(Global.MAP_DIR, "map*.lsmap", SearchOption.TopDirectoryOnly);
			var fnamesValide = fnames.Where(IsValide);
			List<string> list = new List<string>(fnamesValide);
			if (list.Count == 0) {
				//throw new InvalidOperationException("Операция не может быть совершена, т.к. в директории " + Global.MAP_DIR +
				//                                    " нет файлов карты нужного формата.");
				result = new MapInfo();
				return 2;
			}
			switch (mode) {
				case LoadModes.Top:
					return Load(list[list.Count - 1], out result);
				case LoadModes.Random:
					return Load(list[random.Next(0, list.Count - 1)], out result);
				default:
					//throw new ArgumentException("Встречен неопределенный мод загрузки.", "mode");
					result = new MapInfo();
					return 4;
			}
		}
		/// <summary>
		/// Загружает карту, расположенную по указанному пути
		/// Возвращает код завершения:
		/// 0 - успех.
		/// 3 - Файл карты не найден.
		/// </summary>
		/// <param name="fname">Путь к карте</param>
		/// <param name="result">Записывает карту в указанную переменную</param>
		public static int Load(string fname, out MapInfo result) {
			if (!File.Exists(fname)) {
				//throw new FileNotFoundException("Файл карты не найден.", fname);
				result = new MapInfo();
				return 3;
			}
			using (FileStream fs = File.OpenRead(fname)) {
				result = (MapInfo)bf.Deserialize(fs);
				return 0;
			}
		}

		private static bool IsValide(string fname) {
			int num;
			if (fname.Length == 18 && int.TryParse(fname.Substring(8, 4), out num))
				return true;
			return false;
		}
	}
}
