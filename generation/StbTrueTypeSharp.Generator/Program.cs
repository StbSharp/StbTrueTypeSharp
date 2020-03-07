using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sichem;

namespace StbSharp.StbTrueType.Generator
{
	static class Program
	{
		private static readonly Dictionary<string, string[]> _outputs = new Dictionary<string, string[]>
		{
			["Bitmap"] = new string[]
			{
				"stbtt__handle_clipped_edge",
				"stbtt__rasterize_sorted_edges",
				"stbtt__fill_active_edges_new",
				"stbtt__sort_edges_ins_sort",
				"stbtt__sort_edges_quicksort",
				"stbtt__sort_edges",
				"stbtt__bitmap",
				"stbtt__edge",
				"stbtt__active_edge",
			},
			["Buf"] = new string[]
			{
				"stbtt__buf",
				"stbtt__new_buf",

			},
			["CharString"] = new string[]
			{
				"stbtt__csctx"
			},
			["FontInfo"] = new string[]
			{
				"stbtt_fontinfo",
				"stbtt__close_shape",
				"stbtt__GetCoverageIndex",
				"stbtt__GetGlyphClass",
				"stbtt_kerningentry",
			},
			["RectPack"] = new string[]
			{
				"stbrp_context",
				"stbtt_pack_context",
				"stbtt_pack_range",
				"stbrp_node",
				"stbrp_rect",
			},
			["Heap"] = new string[]
			{
				"stbtt__hheap",
				"stbtt__hheap_chunk"
			}
		};

		private static bool OwnedByClass(this string value, string cls)
		{
			return value.Contains("(" + cls + " ") || value.Contains("(" + cls + "*");
		}

		private static void Write(Dictionary<string, string> input, Dictionary<string, string> output)
		{
			foreach (var pair in input)
			{
				string outputKey = null;
				foreach (var pair2 in _outputs)
				{
					foreach (var prefix in pair2.Value)
					{
						if (pair.Key.StartsWith(prefix))
						{
							outputKey = pair2.Key;
							goto found;
						}
					}
				}
			found:
				;

				if (outputKey == null)
				{
					if (pair.Value.OwnedByClass("stbtt__bitmap"))
					{
						outputKey = "Bitmap";
					}
					else if (pair.Value.OwnedByClass("stbtt__buf"))
					{
						outputKey = "Buf";
					}
					else if (pair.Value.OwnedByClass("stbtt__csctx"))
					{
						outputKey = "CharString";
					}
					else if (pair.Value.OwnedByClass("stbtt_fontinfo"))
					{
						outputKey = "FontInfo";
					}
					else if (pair.Value.OwnedByClass("stbrp_context") ||
						pair.Value.OwnedByClass("stbtt_pack_context"))
					{
						outputKey = "RectPack";
					}
					else if (pair.Value.OwnedByClass("stbtt__hheap"))
					{
						outputKey = "Heap";
					}
				}

				if (outputKey == null)
				{
					outputKey = "Common";
				}

				if (!output.ContainsKey(outputKey))
				{
					output[outputKey] = string.Empty;
				}

				output[outputKey] += pair.Value;
			}
		}

		private static string PostProcess(string data)
		{
			data = Utility.ReplaceNativeCalls(data);

			data = data.Replace("(void *)(0)", "null");
			data = data.Replace("stbtt_vertex* vertices = 0;", "stbtt_vertex* vertices = null;");
			data = data.Replace("(flags & 16)?dx:-dx", "(flags & 16) != 0?dx:-dx");
			data = data.Replace("(flags & 32)?dy:-dy", "(flags & 32) != 0?dy:-dy");
			data = data.Replace("(vertices) == (0)", "vertices == null");
			data = data.Replace("sizeof((vertices[0]))", "sizeof(stbtt_vertex)");
			data = data.Replace("(int)(!(flags & 1))", "((flags & 1) != 0?0:1)");
			data = data.Replace("vertices = 0;", "vertices = null;");
			data = data.Replace("stbtt_vertex* comp_verts = 0;", "stbtt_vertex* comp_verts = null;");
			data = data.Replace("stbtt_vertex* tmp = 0;", "stbtt_vertex* tmp = null;");
			data = data.Replace(",)", ")");
			data = data.Replace("+ +", "+");
			data = data.Replace("(sizeof(stbtt__hheap_chunk) + size * count)",
				"((ulong)sizeof(stbtt__hheap_chunk)+ size * (ulong)(count))");
			data = data.Replace("size * hh->num_remaining_in_head_chunk",
				"size * (ulong)hh->num_remaining_in_head_chunk");
			data = data.Replace("sizeof((*z))", "sizeof(stbtt__active_edge)");
			data = data.Replace("_next_ = 0;", "_next_ = null;");
			data = data.Replace("sizeof((scanline[0]))", "sizeof(float)");
			data = data.Replace("int c = (int)(((a)->y0) < ((b)->y0));", "int c = (int)(a->y0 < b->y0?1:0);");
			data = data.Replace("sizeof((*e))", "sizeof(stbtt__edge)");
			data = data.Replace("sizeof((**contour_lengths))", "sizeof(int)");
			data = data.Replace("sizeof((points[0]))", "sizeof(stbtt__point)");
			data = data.Replace("sizeof((*context))", "sizeof(stbrp_context)");
			data = data.Replace("sizeof((*nodes))", "sizeof(stbrp_node)");
			data = data.Replace("sizeof((*rects))", "sizeof(stbrp_rect)");
			data = data.Replace("(int)(((a[0]) == (b[0])) && ((a[1]) == (b[1])));",
				"(int)(((a[0] == b[0]) && (a[1] == b[1]))?1:0);");
			data = data.Replace("stbtt__hheap hh = (stbtt__hheap)({ null, null, 0 })",
				"stbtt__hheap hh = new stbtt__hheap()");

			return data;
		}

		static void Process()
		{
			var parameters = new ConversionParameters
			{
				InputPath = @"stb_truetype.h",
				Defines = new[]
				{
						"STB_TRUETYPE_IMPLEMENTATION",
				},
				SkipStructs = new string[]
				{
				},
				SkipGlobalVariables = new string[]
				{
				},
				SkipFunctions = new string[]
				{
						"stbtt__find_table",
				},
				Classes = new string[]
				{
						"stbtt_pack_context",
						"stbtt_fontinfo",
				},
				GlobalArrays = new string[]
				{
				},
				GenerateSafeCode = false,
			};

			var cp = new ClangParser();

			var result = cp.Process(parameters);

			// Post processing
			Logger.Info("Post processing...");

			var outputFiles = new Dictionary<string, string>();
			Write(result.Constants, outputFiles);
			Write(result.GlobalVariables, outputFiles);
			Write(result.Enums, outputFiles);
			Write(result.Structs, outputFiles);
			Write(result.Methods, outputFiles);

			foreach (var pair in outputFiles)
			{
				var data = PostProcess(pair.Value);

				var sb = new StringBuilder();
				sb.AppendLine(string.Format("// Generated by Sichem at {0}", DateTime.Now));
				sb.AppendLine();

				sb.AppendLine("using System;");
				sb.AppendLine("using System.Runtime.InteropServices;");

				sb.AppendLine();

				sb.Append("namespace StbTrueTypeSharp\n{\n\t");
				sb.AppendLine("unsafe partial class StbTrueType\n\t{");

				data = sb.ToString() + data;
				data += "}\n}";

				var fileName = @"..\..\..\..\..\src\StbTrueType.Generated." + pair.Key + ".cs";
				Logger.Info("Writing {0}", fileName);
				File.WriteAllText(fileName, data);
			}
		}

		static void Main(string[] args)
		{
			try
			{
				Process();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.StackTrace);
			}

			Console.WriteLine("Finished. Press any key to quit.");
			Console.ReadKey();
		}
	}
}