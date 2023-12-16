using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using StbTrueTypeSharp.Tests.Utility;
using static StbTrueTypeSharp.StbTrueType;
using FontsList = System.Collections.Generic.List<StbTrueTypeSharp.StbTrueType.stbtt_fontinfo>;

namespace StbTrueTypeSharp.Tests
{
	[TestFixture]
	public unsafe class Tests
	{
		private static readonly Assembly _assembly = typeof(Tests).Assembly;

		/// <summary>
		///     Makes sure Exception is thrown if font file lacks index map
		/// </summary>
		[Test]
		public void TestNoIndexMap()
		{
			var ttfData = File.ReadAllBytes(@"C:\Windows\Fonts\webdings.ttf");
			Assert.Throws<Exception>(() =>
			{
				var fontInfo = CreateFont(ttfData, 0);
			});
		}

		[Test]
		public void TestCreationAndDispose()
		{
			var ttfData = _assembly.ReadResourceAsBytes("DroidSans.ttf");
			var fontInfo = CreateFont(ttfData, 0);
			Assert.NotNull(fontInfo);
			Assert.IsTrue(fontInfo.isDataCopy);
			fontInfo.Dispose();
			Assert.IsTrue(fontInfo.data == null);
		}

		[Test]
		public unsafe void TestLoadFontCollection()
		{
			string fontsPath = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
			string someTtcPath = Directory.EnumerateFiles(fontsPath, "*.ttc", new EnumerationOptions()
			{
				AttributesToSkip = FileAttributes.Directory,
				MatchCasing = MatchCasing.CaseInsensitive,
				MatchType = MatchType.Simple,
				RecurseSubdirectories = false,
				IgnoreInaccessible = true,
				ReturnSpecialDirectories = false
			}).FirstOrDefault();

			if (someTtcPath == null)
				Assert.Inconclusive("You don't have a ttc font installed on your computer, but this test requires it.");

			byte[] ttcContent = File.ReadAllBytes(someTtcPath);
			// Assert.NotNull(someTtc);
			FontsList fonts;
			int numberOfFonts;

			fixed (byte* ttcPtr = ttcContent)
			{
				numberOfFonts = stbtt_GetNumberOfFonts(ttcPtr);
				fonts = new FontsList(numberOfFonts);
				for (int i = 0; i < numberOfFonts; i++)
				{
					int offset = stbtt_GetFontOffsetForIndex(ttcPtr, i);
					fonts.Add(CreateFont(ttcContent, offset));
				}
			}

			Assert.AreEqual(numberOfFonts, fonts.Count);
		}

		private void TestRasterize(stbtt_fontinfo fontInfo, string text, float size)
		{
			int iascent, idescent, ilineGap;
			stbtt_GetFontVMetrics(fontInfo, &iascent, &idescent, &ilineGap);

			var scale = stbtt_ScaleForPixelHeight(fontInfo, 32.0f);
			var ascent = iascent * scale;
			var descent = idescent * scale;
			var lineGap = ilineGap * scale;

			var lineHeight = ascent - descent + lineGap;

			Assert.IsTrue(lineHeight.EpsilonEquals(32.0f));

			for (var i = 0; i < text.Length; ++i)
			{
				var c = text[i];

				if (char.IsWhiteSpace(c))
				{
					continue;
				}

				var glyphId = stbtt_FindGlyphIndex(fontInfo, c);
				Assert.NotZero(glyphId);

				int advanceWidth, leftSideBearing;
				stbtt_GetGlyphHMetrics(fontInfo, glyphId, &advanceWidth, &leftSideBearing);

				int x0, y0, x1, y1;
				stbtt_GetGlyphBitmapBox(fontInfo, glyphId, scale, scale, &x0, &y0, &x1, &y1);

				var width = x1 - x0;
				var height = y1 - y0;

				Assert.NotZero(width);
				Assert.NotZero(height);
				var data = new byte[width * height];

				fixed (byte* ptr = data)
				{
					stbtt_MakeGlyphBitmap(fontInfo, ptr, width, height, width, scale, scale, glyphId);
				}
			}
		}

		[Test]
		public void TestNewRasterizer()
		{
			var ttfData = _assembly.ReadResourceAsBytes("DroidSans.ttf");
			var fontInfo = CreateFont(ttfData, 0);
			Assert.NotNull(fontInfo);
			Assert.IsTrue(fontInfo.isDataCopy);

			TestRasterize(fontInfo, "Hello, World!", 32.0f);

			Assert.IsFalse(usedOldRasterizer);

			fontInfo.Dispose();
			Assert.IsTrue(fontInfo.data == null);
		}

		[Test]
		public void TestOldRasterizer()
		{
			var ttfData = _assembly.ReadResourceAsBytes("DroidSans.ttf");
			var fontInfo = CreateFont(ttfData, 0);
			Assert.NotNull(fontInfo);
			Assert.IsTrue(fontInfo.isDataCopy);

			fontInfo.useOldRasterizer = true;

			TestRasterize(fontInfo, "Hello, World!", 32.0f);

			Assert.IsTrue(usedOldRasterizer);

			fontInfo.Dispose();
			Assert.IsTrue(fontInfo.data == null);
		}
	}
}