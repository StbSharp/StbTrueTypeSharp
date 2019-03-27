using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StbTrueTypeSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace StbSharp.MonoGame.Test
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		private const int FontBitmapWidth = 8192;
		private const int FontBitmapHeight = 8192;

		GraphicsDeviceManager _graphics;
		SpriteBatch _spriteBatch;

		private SpriteFont _font;

		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this)
			{
				PreferredBackBufferWidth = 1920,
				PreferredBackBufferHeight = 1080,
				SynchronizeWithVerticalRetrace = false
			};
			IsFixedTimeStep = false;

			Content.RootDirectory = "Content";
			IsMouseVisible = true;
			Window.AllowUserResizing = true;
		}

		private void LoadFont()
		{
			var fontBaker = new FontBaker();

			fontBaker.Begin(FontBitmapWidth, FontBitmapHeight);
			fontBaker.Add(File.ReadAllBytes("Fonts/DroidSans.ttf"), 32, new[]
			{
				CharacterRange.BasicLatin,
				CharacterRange.Latin1Supplement,
				CharacterRange.LatinExtendedA,
				CharacterRange.Cyrillic,
				CharacterRange.Greek,
			});

			fontBaker.Add(File.ReadAllBytes("Fonts/DroidSansJapanese.ttf"), 32, new[]
			{
				CharacterRange.Hiragana,
				CharacterRange.Katakana,
			});

			fontBaker.Add(File.ReadAllBytes("Fonts/ZCOOLXiaoWei-Regular.ttf"), 32, new[]
			{
				CharacterRange.CjkSymbolsAndPunctuation,
				CharacterRange.CjkUnifiedIdeographs
			});

			fontBaker.Add(File.ReadAllBytes("Fonts/KoPubBatang-Regular.ttf"), 32, new[]
			{
				CharacterRange.HangulCompatibilityJamo,
				CharacterRange.HangulSyllables
			});

			var _charData = fontBaker.End();

			// Offset by minimal offset
			int minimumOffsetY = 10000;
			foreach (var pair in _charData.Glyphs)
			{
				if (pair.Value.YOffset < minimumOffsetY)
				{
					minimumOffsetY = pair.Value.YOffset;
				}
			}

			var keys = _charData.Glyphs.Keys.ToArray();
			foreach (var key in keys)
			{
				var pc = _charData.Glyphs[key];
				pc.YOffset -= minimumOffsetY;
				_charData.Glyphs[key] = pc;
			}

			var rgb = new Color[FontBitmapWidth * FontBitmapHeight];
			for (var i = 0; i < _charData.Bitmap.Length; ++i)
			{
				var b = _charData.Bitmap[i];
				rgb[i].R = b;
				rgb[i].G = b;
				rgb[i].B = b;

				rgb[i].A = b;
			}

			var fontTexture = new Texture2D(GraphicsDevice, FontBitmapWidth, FontBitmapHeight);
			fontTexture.SetData(rgb);

			var glyphBounds = new List<Rectangle>();
			var cropping = new List<Rectangle>();
			var chars = new List<char>();
			var kerning = new List<Vector3>();

			var orderedKeys = _charData.Glyphs.Keys.OrderBy(a => a);
			foreach (var key in orderedKeys)
			{
				var character = _charData.Glyphs[key];

				var bounds = new Rectangle(character.X, character.Y,
										character.Width,
										character.Height);

				glyphBounds.Add(bounds);
				cropping.Add(new Rectangle(character.XOffset, character.YOffset, bounds.Width, bounds.Height));

				chars.Add((char)key);

				kerning.Add(new Vector3(0, bounds.Width, character.XAdvance - bounds.Width));
			}

			var constructorInfo = typeof(SpriteFont).GetTypeInfo().DeclaredConstructors.First();
			_font = (SpriteFont)constructorInfo.Invoke(new object[]
			{
				fontTexture, glyphBounds, cropping,
				chars, 20, 0, kerning, ' '
			});
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			_spriteBatch = new SpriteBatch(GraphicsDevice);

			// TODO: use this.Content to load your game content here
			// Create white texture

			// Load ttf
			LoadFont();

			GC.Collect();
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// game-specific content.
		/// </summary>
		protected override void UnloadContent()
		{
			// TODO: Unload any non ContentManager content here
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
				Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			_spriteBatch.Begin();

			// Draw alphabet for all common languages.
			_spriteBatch.DrawString(_font, "Eng: A a B b C c D d E e F f G g H h I i J j K k L l M m N n O o P p Q q R r S s T t U u V v W w X x Y y Z z",
				new Vector2(0, 0), Color.White);
			_spriteBatch.DrawString(_font, "Rus: А а, Б б, В в, Г г, Д д, Е е, Ё ё, Ж ж, З з, И и, Й й, К к, Л л, М м, Н н, О о, П п, Р р, С с, Т т, У у, Ф ф, Х х, Ц ц, Ч ч, Ш ш, Щ щ, Ъ ъ, Ы ы, Ь ь, Э э, Ю ю, Я я, І і, Ѳ ѳ, Ѣ ѣ, Ѵ ѵ",
				new Vector2(0, 30), Color.Maroon);
			_spriteBatch.DrawString(_font, "Scandinavian: Å å, Ø ø, Æ æ, œ, þ Fra: â ç è é ê î ô û ë ï ù á í ì ó ò ú, Romana: ă â î ș ț",
				new Vector2(0, 60), Color.Green);
			_spriteBatch.DrawString(_font, "Fra: â ç è é ê î ô û ë ï ù á í ì ó ò ú // Romana: ă â î ș ț",
				new Vector2(0, 90), Color.Navy);
			_spriteBatch.DrawString(_font, "Pol: Ą Ć Ę Ł Ń Ó Ś Ź Ż ą ć ę ł ń ó ś ź ż Zażółć gęślą jaźń, Prtgs: ã, õ, â, ê, ô, á, é, í, ó, ú, à, ç",
				new Vector2(0, 120), Color.Yellow);
			_spriteBatch.DrawString(_font, "Cze: ž š ů ě ř Příliš žluťoučký kůň úpěl ďábelské kódy, Lat/Lit: ā, č, ē, ģ, ī, ķ, ļ, ņ, ō, ŗ, š, ū, ž, ą, č, ę, ė, į, š, ų, ū, ž",
				new Vector2(0, 150), Color.Black);
			_spriteBatch.DrawString(_font, "Greek: Α α, Β β, Γ γ, Δ δ, Ε ε, Ζ ζ, Η η, Θ θ, Ι ι, Κ κ, Λ λ, Μ μ, Ν ν, Ξ ξ, Ο ο, Π π, Ρ ρ, Σ σ/ς, Τ τ, Υ υ, Φ φ, Χ χ, Ψ ψ, Ω ω ά έ ή ί ό ύ ώ",
				new Vector2(0, 180), Color.Aqua);
			_spriteBatch.DrawString(_font, "Jap: いろはにほ 。へどひらがなカタカナ, Kor: 한국어조선말, Cn: 国会这来对开关门时个书长万边东车爱儿。吾艾、贼德艾尺",
				new Vector2(0, 210), Color.Cyan);
			_spriteBatch.DrawString(_font, "Other symbols: Ñ ñ ¿ ¡ Ç ç á ê Ä ä à â Ö ö ô Ü ü ë ß ẞ Ÿ ÿ Œ Æ æ ï Ğ ğ Ş ş Ő ő Ű ű ù",
				new Vector2(0, 240), Color.Moccasin);

			// _spriteBatch.Draw(_font.Texture, new Vector2(0, 300));

			_spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}