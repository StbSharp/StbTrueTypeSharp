using System;
using System.Collections.Generic;

namespace StbTrueTypeSharp
{
	public class TtfFontBakerResult
	{
		public TtfFontBakerResult(Dictionary<int, GlyphInfo> glyphs,
			float fontPixelHeight,
			byte[] pixels,
			int width,
			int height)
		{
			if (glyphs == null)
				throw new ArgumentNullException(nameof(glyphs));

			if (fontPixelHeight <= 0)
				throw new ArgumentOutOfRangeException(nameof(fontPixelHeight));

			if (pixels == null)
				throw new ArgumentNullException(nameof(pixels));

			if (width <= 0)
				throw new ArgumentOutOfRangeException(nameof(width));

			if (height <= 0)
				throw new ArgumentOutOfRangeException(nameof(height));

			if (pixels.Length < width * height)
				throw new ArgumentException("pixels.Length should be higher than width * height");

			Glyphs = glyphs;
			FontFontPixelHeight = fontPixelHeight;
			Pixels = pixels;
			Width = width;
			Height = height;
		}

		public Dictionary<int, GlyphInfo> Glyphs
		{
			get;
		}

		public float FontFontPixelHeight
		{
			get;
		}

		public byte[] Pixels
		{
			get;
		}

		public int Width
		{
			get;
		}

		public int Height
		{
			get;
		}
	}
}