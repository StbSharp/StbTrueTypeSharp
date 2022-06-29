using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using StbTrueTypeSharp.Tests.Utility;
using static StbTrueTypeSharp.StbTrueType;

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
	}
}