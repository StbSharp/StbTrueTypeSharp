using StbImageWriteSharp;
using System;
using System.IO;
using static StbTrueTypeSharp.StbTrueType;

namespace OutputString
{
	unsafe class Program
	{
		static void Main(string[] args)
		{
			if (args.Length < 1 || string.IsNullOrEmpty(args[0].Trim()))
			{
				Console.WriteLine("Usage: OutputString.exe <string>");
				return;
			}

			var s = args[0];
			var fontSize = 32.0f;
			var bufferSize = 256;

			var bytes = File.ReadAllBytes("Fonts/DroidSans.ttf");
			var buffer = new byte[bufferSize * bufferSize];

			var info = new stbtt_fontinfo();

			fixed (byte* ptr = bytes)
			{
				var res = stbtt_InitFont(info, ptr, 0);
			}

			int ascent, descent, lineGap;
			stbtt_GetFontVMetrics(info, &ascent, &descent, &lineGap);
			var lineHeight = ascent - descent + lineGap;

			var scale = stbtt_ScaleForPixelHeight(info, fontSize);

			ascent = (int)(ascent * scale + 0.5f);
			descent = (int)(descent * scale - 0.5f);
			lineHeight = (int)(lineHeight * scale + 0.5f);

			int posX = 0, posY = 0;

			for (var i = 0; i < s.Length; ++i)
			{
				var c = s[i];
				var glyphId = stbtt_FindGlyphIndex(info, c);
				if (glyphId == 0)
				{
					continue;
				}

				int advanceTemp = 0, lsbTemp = 0;
				stbtt_GetGlyphHMetrics(info, glyphId, &advanceTemp, &lsbTemp);
				var advance = (int)(advanceTemp * scale + 0.5f);

				int x0, y0, x1, y1;
				stbtt_GetGlyphBitmapBox(
					info, glyphId, scale, scale, &x0, &y0, &x1, &y1
				);

				posX += x0;
				posY = ascent + y0;

				if (posY < 0)
				{
					posY = 0;
				}

				fixed (byte* ptr = buffer)
				{
					var ptr2 = ptr + (posX + posY * bufferSize);

					stbtt_MakeGlyphBitmap(
						info,
						ptr2,
						x1 - x0,
						y1 - y0,
						bufferSize,
						scale,
						scale,
						glyphId
					);
				}

				posX += advance;
			}

			var imageWriter = new ImageWriter();
			using (var stream = File.OpenWrite("output.png"))
			{
				imageWriter.WritePng(buffer, bufferSize, bufferSize, ColorComponents.Grey, stream);
			}
		}
	}
}
