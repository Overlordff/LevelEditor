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
using LESL;

namespace LevelEditor
{
	class UIPanel : Subpanel
	{
		#region Variables

		Rectangle mapRect;
		Rectangle bottomPanelRect;
		Rectangle rightPanelRect;

		Rectangle prevViewRect;
		TexturePrev prevView;

		List<Button> staticButtons = new List<Button>();
		List<Button> notStaticButtons = new List<Button>();

		List<Button> buttons = new List<Button>();
		List<Slider> sliders = new List<Slider>();
		List<TexturePrev> prevsList = new List<TexturePrev>();
		PrevList Navigator;

		TexturePrev currentPrev;
		Slider r_Slider, g_Slider, b_Slider;

		#endregion

		public Rectangle MapRect { get { return mapRect; } }

		private TexturePrev CurrentPrev {
			get { return currentPrev; }
			set {
				TexturePrev prev = currentPrev;
				currentPrev = value;
				if (prev != value)
					OnChangeCurrentPrev(new ChangePrevEventArgs(value));
			}
		}

		private TexturePrev PrevView {
			get { return prevView; }
			set {
				prevView = value;
				if (prevView != null) {
					prevView.Position = new Vector2(prevViewRect.X, prevViewRect.Y);
					prevView.Size = new Vector2(prevViewRect.Width, prevViewRect.Height);
					prevView.IsTextVisible = false;
					OnChoseTile(new TileEventArgs(prevView.ToTile()));
				}
				else
					OnChoseTile(new TileEventArgs(null));
			}
		}

		private event EventHandler<ChangePrevEventArgs> ChangeCurrentPrev;
		public event EventHandler<TileEventArgs> ChoseTile;
		public event EventHandler<EventArgs> SaveCalled;
		public event EventHandler<EventArgs> LoadCalled;
		public event EventHandler<EventArgs> ClearCalled;
		public event EventHandler<TileEventArgs> FillCalled;
		public event EventHandler<EventArgs> ExitCalled;

		public UIPanel(Screen owner, Sprite sprite)
			: base(owner, sprite) {
			mapRect = new Rectangle(1085, 808, 180, 200);
			bottomPanelRect = new Rectangle(18, 808, 1065, 200);
			rightPanelRect = new Rectangle(1085, 18, 178, 800);
			prevViewRect = new Rectangle(bottomPanelRect.X + 650, bottomPanelRect.Y + 15, 125, 125);
			this.ChangeCurrentPrev += new EventHandler<ChangePrevEventArgs>(UIPanel_ChangeCurrentPrev);

			#region Texture Prevs

			TexturePrev SnowPrev = new TexturePrev(MultiSprite.CreateSprite(content, Fnames.SNOW, new Vector2(rightPanelRect.X, rightPanelRect.Y + 50), new Vector2(80, 80), Vector2.One),
												   Tiles.Snow,
												   content);
			TexturePrev DustPrev = new TexturePrev(MultiSprite.CreateSprite(content, Fnames.DUST, new Vector2(rightPanelRect.X, SnowPrev.Position.Y + SnowPrev.Size.Y + 50), new Vector2(80, 80), Vector2.One),
												   Tiles.Dust,
												   content);
			TexturePrev Grass1 = new TexturePrev(MultiSprite.CreateSprite(content, Fnames.GRASS1, new Vector2(rightPanelRect.X, DustPrev.Position.Y + DustPrev.Size.Y + 50), new Vector2(80, 80), Vector2.One),
												   Tiles.Grass1,
												   content);
			TexturePrev Grass2 = new TexturePrev(MultiSprite.CreateSprite(content, Fnames.GRASS2, new Vector2(rightPanelRect.X, Grass1.Position.Y + DustPrev.Size.Y + 50), new Vector2(80, 80), Vector2.One),
												   Tiles.Grass2,
												   content);
			TexturePrev Grass3 = new TexturePrev(MultiSprite.CreateSprite(content, Fnames.GRASS3, new Vector2(rightPanelRect.X, Grass2.Position.Y + DustPrev.Size.Y + 50), new Vector2(80, 80), Vector2.One),
												   Tiles.Grass3,
												   content);
			TexturePrev DarkGrassPrev = new TexturePrev(MultiSprite.CreateSprite(content, Fnames.DARK_GRASS, new Vector2(rightPanelRect.X, Grass3.Position.Y + Grass1.Size.Y + 50), new Vector2(80, 80), Vector2.One),
												   Tiles.DarkGrass,
												   content);
			TexturePrev GrassPath = new TexturePrev(MultiSprite.CreateSprite(content, Fnames.GRASS_PATH, new Vector2(rightPanelRect.X, DarkGrassPrev.Position.Y + Grass1.Size.Y + 50), new Vector2(80, 80), Vector2.One),
												   Tiles.GrassPath,
												   content);
			TexturePrev Sand1 = new TexturePrev(MultiSprite.CreateSprite(content, Fnames.SAND1, new Vector2(rightPanelRect.X, GrassPath.Position.Y + Grass1.Size.Y + 50), new Vector2(80, 80), Vector2.One),
												   Tiles.Sand1,
												   content);
			TexturePrev Sand2 = new TexturePrev(MultiSprite.CreateSprite(content, Fnames.SAND2, new Vector2(rightPanelRect.X, Sand1.Position.Y + Grass1.Size.Y + 50), new Vector2(80, 80), Vector2.One),
												   Tiles.Sand2,
												   content);
			TexturePrev Sand3 = new TexturePrev(MultiSprite.CreateSprite(content, Fnames.SAND3, new Vector2(rightPanelRect.X, Sand2.Position.Y + Grass1.Size.Y + 50), new Vector2(80, 80), Vector2.One),
												   Tiles.Sand3,
												   content);
			TexturePrev Sand4 = new TexturePrev(MultiSprite.CreateSprite(content, Fnames.SAND4, new Vector2(rightPanelRect.X, Sand3.Position.Y + Grass1.Size.Y + 50), new Vector2(80, 80), Vector2.One),
												   Tiles.Sand4,
												   content);
			TexturePrev Water1a = new TexturePrev(MultiSprite.CreateSprite(content, Fnames.WATER1_A, new Vector2(rightPanelRect.X, Sand4.Position.Y + DarkGrassPrev.Size.Y + 50), new Vector2(80, 80), Vector2.One),
												   Tiles.Water1a,
												   content);
			TexturePrev Water1b = new TexturePrev(MultiSprite.CreateSprite(content, Fnames.WATER1_B, new Vector2(rightPanelRect.X, Water1a.Position.Y + DarkGrassPrev.Size.Y + 50), new Vector2(80, 80), Vector2.One),
												   Tiles.Water1b,
												   content);
			TexturePrev Water1c = new TexturePrev(MultiSprite.CreateSprite(content, Fnames.WATER1_C, new Vector2(rightPanelRect.X, Water1b.Position.Y + DarkGrassPrev.Size.Y + 50), new Vector2(80, 80), Vector2.One),
												   Tiles.Water1c,
												   content);
			TexturePrev Water1d = new TexturePrev(MultiSprite.CreateSprite(content, Fnames.WATER1_D, new Vector2(rightPanelRect.X, Water1c.Position.Y + DarkGrassPrev.Size.Y + 50), new Vector2(80, 80), Vector2.One),
												   Tiles.Water1d,
												   content);
			TexturePrev Water2 = new TexturePrev(MultiSprite.CreateSprite(content, Fnames.WATER2, new Vector2(rightPanelRect.X, Water1d.Position.Y + Water1a.Size.Y + 50), new Vector2(80, 80), Vector2.One),
												   Tiles.Water2,
												   content);
			TexturePrev Water3a = new TexturePrev(MultiSprite.CreateSprite(content, Fnames.WATER3_A, new Vector2(rightPanelRect.X, Water2.Position.Y + Water2.Size.Y + 50), new Vector2(80, 80), Vector2.One),
												   Tiles.Water3a,
												   content);
			TexturePrev Water3b = new TexturePrev(MultiSprite.CreateSprite(content, Fnames.WATER3_B, new Vector2(rightPanelRect.X, Water3a.Position.Y + Water2.Size.Y + 50), new Vector2(80, 80), Vector2.One),
												   Tiles.Water3b,
												   content);
			TexturePrev Water3c = new TexturePrev(MultiSprite.CreateSprite(content, Fnames.WATER3_C, new Vector2(rightPanelRect.X, Water3b.Position.Y + Water2.Size.Y + 50), new Vector2(80, 80), Vector2.One),
												   Tiles.Water3c,
												   content);
			TexturePrev Ford1 = new TexturePrev(MultiSprite.CreateSprite(content, Fnames.FORD1, new Vector2(rightPanelRect.X, Water3c.Position.Y + Water2.Size.Y + 50), new Vector2(80, 80), Vector2.One),
												   Tiles.Ford1,
												   content);
			TexturePrev Road1 = new TexturePrev(MultiSprite.CreateSprite(content, Fnames.ROAD1, new Vector2(rightPanelRect.X, Ford1.Position.Y + Water2.Size.Y + 50), new Vector2(80, 80), Vector2.One),
												   Tiles.Road1,
												   content);
			TexturePrev Road2 = new TexturePrev(MultiSprite.CreateSprite(content, Fnames.ROAD2, new Vector2(rightPanelRect.X, Road1.Position.Y + Water2.Size.Y + 50), new Vector2(80, 80), Vector2.One),
												   Tiles.Road2,
												   content);

			prevsList.Add(SnowPrev);
			prevsList.Add(DustPrev);
			prevsList.Add(Grass1);
			prevsList.Add(Grass2);
			prevsList.Add(Grass3);
			prevsList.Add(DarkGrassPrev);
			prevsList.Add(GrassPath);
			prevsList.Add(Sand1);
			prevsList.Add(Sand2);
			prevsList.Add(Sand3);
			prevsList.Add(Sand4);
			prevsList.Add(Water1a);
			prevsList.Add(Water1b);
			prevsList.Add(Water1c);
			prevsList.Add(Water1d);
			prevsList.Add(Water2);
			prevsList.Add(Water3a);
			prevsList.Add(Water3b);
			prevsList.Add(Water3c);
			prevsList.Add(Ford1);
			prevsList.Add(Road1);
			prevsList.Add(Road2);
			prevsList.ForEach((x) => x.Click += new EventHandler<MouseElementEventArgs>(texturePrev_Click));

			#endregion

			#region Navigator

			Navigator = new PrevList(MultiSprite.CreateSprite(content, Fnames.ARROW, new Vector2(rightPanelRect.X + 50, rightPanelRect.Y + 15), new Vector2(32, 24), Vector2.One),
									 content,
									 5,
									 (x) => x.Position = new Vector2(rightPanelRect.X + rightPanelRect.Width / 2 - x.Size.X / 2, x.Position.Y));
			foreach (var prev in prevsList) {
				Navigator.AddPrev(prev);
			}

			#endregion

			#region Sliders

			r_Slider = new Slider(MultiSprite.CreateSprite(content, Fnames.SLIDER, new Vector2(bottomPanelRect.X + 50, bottomPanelRect.Y + 50), new Vector2(150, 23), Vector2.One),
										 Sprite.CreateSprite(content, Fnames.SLIDER_, new Vector2(bottomPanelRect.X + 50, bottomPanelRect.Y + 50 + 18), new Vector2(12, 12)),
										 Color.Red,
										 content,
										 "Red component");
			g_Slider = new Slider(MultiSprite.CreateSprite(content, Fnames.SLIDER, new Vector2(bottomPanelRect.X + 50 + 200, bottomPanelRect.Y + 50), new Vector2(150, 23), Vector2.One),
										 Sprite.CreateSprite(content, Fnames.SLIDER_, new Vector2(bottomPanelRect.X + 50 + 200, bottomPanelRect.Y + 50 + 18), new Vector2(12, 12)),
										 Color.Green,
										 content,
										 "Green component");
			b_Slider = new Slider(MultiSprite.CreateSprite(content, Fnames.SLIDER, new Vector2(bottomPanelRect.X + 50 + 400, bottomPanelRect.Y + 50), new Vector2(150, 23), Vector2.One),
										 Sprite.CreateSprite(content, Fnames.SLIDER_, new Vector2(bottomPanelRect.X + 50 + 400, bottomPanelRect.Y + 50 + 18), new Vector2(12, 12)),
										 Color.Blue,
										 content,
										 "Blue component");

			sliders.Add(r_Slider);
			sliders.Add(g_Slider);
			sliders.Add(b_Slider);
			foreach (var slider in sliders) {
				slider.Visible = false;
				slider.ValueChanged += new EventHandler<ValueChangedEventArgs<float>>(slider_ValueChanged);
			}

			#endregion

			#region Buttons

			Button Fill = new Button(MultiSprite.CreateSprite(content, Fnames.FILL_B, new Vector2(bottomPanelRect.X + 50, bottomPanelRect.Y + 150), new Vector2(126, 36), Vector2.One));
			Fill.Click += new EventHandler<MouseElementEventArgs>(Fill_Click);
			Fill.Visible = false;

			Button Save = new Button(MultiSprite.CreateSprite(content, Fnames.SAVE_B, new Vector2(bottomPanelRect.Right - 160, bottomPanelRect.Y + 15), new Vector2(126, 36), Vector2.One));
			Save.Click += new EventHandler<MouseElementEventArgs>(Save_Click);

			Button Load = new Button(MultiSprite.CreateSprite(content, Fnames.LOAD_B, new Vector2(bottomPanelRect.Right - 160, Save.Position.Y + Save.Size.Y + 10), new Vector2(126, 36), Vector2.One));
			Load.Click += new EventHandler<MouseElementEventArgs>(Load_Click);

			Button Clear = new Button(MultiSprite.CreateSprite(content, Fnames.CLEAR_B, new Vector2(bottomPanelRect.Right - 160, Load.Position.Y + Save.Size.Y + 10), new Vector2(126, 36), Vector2.One));
			Clear.Click += new EventHandler<MouseElementEventArgs>(Clear_Click);

			Button Exit = new Button(MultiSprite.CreateSprite(content, Fnames.EXIT_B, new Vector2(bottomPanelRect.Right - 160, Clear.Position.Y + Save.Size.Y + 10), new Vector2(126, 36), Vector2.One));
			Exit.Click += new EventHandler<MouseElementEventArgs>(Exit_Click);

			notStaticButtons.Add(Fill);

			staticButtons.Add(Save);
			staticButtons.Add(Load);
			staticButtons.Add(Clear);
			staticButtons.Add(Exit);

			buttons.AddRange(staticButtons);
			buttons.AddRange(notStaticButtons);
			foreach (var button in buttons) {
				button.AnimationMode = AnimationStates.Small;
				button.MouseMoveIn += new EventHandler<MouseElementEventArgs>(Button_MouseMoveIn);
				button.MouseMoveOut += new EventHandler<MouseElementEventArgs>(Button_MouseMoveOut);
			}

			#endregion

			Components.Add(Navigator);
			Components.AddRange(sliders);
			Components.AddRange(buttons);
		}

		public override void Update(GameTime gameTime) {
			base.Update(gameTime);
		}

		public override void Draw(SpriteBatch spriteBatch) {
			base.Draw(spriteBatch);
			if (prevView != null)
				prevView.Draw(spriteBatch);
		}

		public bool SetCurrentPrev(Tile? tile) {
			if (tile.HasValue) {
				foreach (var prev in prevsList) {
					if (tile.Value.TileType == prev.TileType) {
						if (CurrentPrev != null)
							CurrentPrev.IsFocused = false;
						CurrentPrev = prev;
						if (CurrentPrev != null)
							CurrentPrev.IsFocused = true;
						SetColorSliders(tile.Value.Color);
						return true;
					}
				}
				return false;
			} else {
				if (CurrentPrev != null)
					CurrentPrev.IsFocused = false;
				CurrentPrev = null;
				return true;
			}
		}

		protected override void MouseHandler(MouseState ms, MouseState oms) {
			base.MouseHandler(ms, oms);
		}

		#region Private Methods

		private void FormatPanel(List<TexturePrev> prevsList = null) {
			if (prevsList != null) {

			}
		}

		private void SetColorSliders(Color color) {
			r_Slider.Visible = true;
			r_Slider.Value = (decimal)color.R / 255 * 100;
			g_Slider.Visible = true;
			g_Slider.Value = (decimal)color.G / 255 * 100;
			b_Slider.Visible = true;
			b_Slider.Value = (decimal)color.B / 255 * 100;
		}

		private void texturePrev_Click(object sender, MouseElementEventArgs e) {
			if (sender is TexturePrev) {
				TexturePrev temp = (TexturePrev)sender;
				if (CurrentPrev != null)
					CurrentPrev.IsFocused = false;
				if (CurrentPrev != temp)
					temp.IsFocused = !temp.IsFocused;
				if (temp.IsFocused) {
					CurrentPrev = temp;
				} else {
					CurrentPrev = null;
				}
			}
		}

		private void UIPanel_ChangeCurrentPrev(object sender, ChangePrevEventArgs e) {
			if (e.Prev == null) {
				foreach (var slider in sliders) {
					slider.Visible = false;
				}
				foreach (var button in notStaticButtons) {
					button.Visible = false;
				}
				PrevView = null;
			} else {
				SetColorSliders(e.Prev.Image.Color);
				PrevView = (TexturePrev)CurrentPrev.Clone();
				if (PrevView != null)
					PrevView.Visible = true;
				foreach (var button in notStaticButtons) {
					button.Visible = true;
				}
			}
		}

		private void slider_ValueChanged(object sender, ValueChangedEventArgs<float> e) {
			if (sender == r_Slider) {
				if (prevView != null) {
					Color temp = prevView.Image.Color;
					temp.R = (byte)(e.Value / 100 * 255);
					prevView.Image.Color = temp;
					OnChoseTile(new TileEventArgs(prevView.ToTile()));
				}
			}
			if (sender == g_Slider) {
				if (prevView != null) {
					Color temp = prevView.Image.Color;
					temp.G = (byte)(e.Value / 100 * 255);
					prevView.Image.Color = temp;
					OnChoseTile(new TileEventArgs(prevView.ToTile()));
				}
			}
			if (sender == b_Slider) {
				if (prevView != null) {
					Color temp = prevView.Image.Color;
					temp.B = (byte)(e.Value / 100 * 255);
					prevView.Image.Color = temp;
					OnChoseTile(new TileEventArgs(prevView.ToTile()));
				}
			}
		}

		#region Button Handlers

		private void Exit_Click(object sender, MouseElementEventArgs e) {
			OnExitCalled(new EventArgs());
		}

		private void Fill_Click(object sender, MouseElementEventArgs e) {
			OnFillCalled(new TileEventArgs(prevView.ToTile()));
		}

		private void Save_Click(object sender, MouseElementEventArgs e) {
			OnSaveCalled(new EventArgs());
		}

		private void Load_Click(object sender, MouseElementEventArgs e) {
			OnLoadCalled(new EventArgs());
		}

		private void Clear_Click(object sender, MouseElementEventArgs e) {
			OnClearCalled(new EventArgs());
		}

		private void Button_MouseMoveIn(object sender, MouseElementEventArgs e) {
			(sender as Button).Animation = true;
		}

		private void Button_MouseMoveOut(object sender, MouseElementEventArgs e) {
			(sender as Button).Animation = false;
		}

		#endregion

		#region Events Callers

		private void OnChoseTile(TileEventArgs e) {
			EventHandler<TileEventArgs> choseTile = ChoseTile;
			if (choseTile != null)
				choseTile(this, e);
		}

		private void OnChangeCurrentPrev(ChangePrevEventArgs e) {
			EventHandler<ChangePrevEventArgs> changeCurrentPrev = ChangeCurrentPrev;
			if (changeCurrentPrev != null)
				changeCurrentPrev(this, e);
		}

		private void OnExitCalled(EventArgs e) {
			EventHandler<EventArgs> exitCalled = ExitCalled;
			if (exitCalled != null)
				exitCalled(this, e);
		}

		private void OnSaveCalled(EventArgs e) {
			EventHandler<EventArgs> saveCalled = SaveCalled;
			if (saveCalled != null)
				saveCalled(this, e);
		}

		private void OnLoadCalled(EventArgs e) {
			EventHandler<EventArgs> loadCalled = LoadCalled;
			if (loadCalled != null)
				loadCalled(this, e);
		}

		private void OnFillCalled(TileEventArgs e) {
			EventHandler<TileEventArgs> fillCalled = FillCalled;
			if (fillCalled != null)
				fillCalled(this, e);
		}

		private void OnClearCalled(EventArgs e) {
			EventHandler<EventArgs> clearCalled = ClearCalled;
			if (clearCalled != null)
				clearCalled(this, e);
		}

		#endregion

		#endregion
	}
}
