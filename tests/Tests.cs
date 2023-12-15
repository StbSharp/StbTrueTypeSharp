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
	}
}