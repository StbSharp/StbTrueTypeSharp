using NUnit.Framework;
using System;
using System.IO;
using System.Reflection;
using static StbTrueTypeSharp.StbTrueType;

namespace StbTrueTypeSharp.Tests
{
	[TestFixture]
	public unsafe class Tests
	{
		private static readonly Assembly _assembly = typeof(Tests).Assembly;

		/// <summary>
		/// Makes sure Exception is thrown if font file lacks index map
		/// </summary>
		[Test]
		public void TestNoIndexMap()
		{
			var ttfData = File.ReadAllBytes(@"C:\Windows\Fonts\webdings.ttf");
			var fontInfo = new stbtt_fontinfo();
			Assert.Throws<Exception>(() =>
			{
				fixed (byte* ttfPtr = ttfData)
				{
					stbtt_InitFont(fontInfo, ttfPtr, 0);
				}
			});
		}
	}
}
