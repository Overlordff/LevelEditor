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
	public interface IComponent
	{
		void Update(GameTime gameTime);
	}

	public interface IDrawableComponent : IComponent
	{
		bool Visible { get; set; }
		void Draw(SpriteBatch spriteBatch);
	}
}
