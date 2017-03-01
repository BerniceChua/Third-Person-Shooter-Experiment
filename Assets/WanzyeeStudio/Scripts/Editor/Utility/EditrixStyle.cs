
/*WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW*\     (   (     ) )
|/                                                      \|       )  )   _((_
||  (c) Wanzyee Studio  < wanzyeestudio.blogspot.com >  ||      ( (    |_ _ |=n
|\                                                      /|   _____))   | !  ] U
\.ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ./  (_(__(S)   |___*/

using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace WanzyeeStudio.Editrix{

	/// <summary>
	/// Extend editor GUI style, and include some premade icons or styles.
	/// </summary>
	public static class EditrixStyle{

		#region Styles

		/// <summary>
		/// The splitter pixel texture with the same color as on the Inspector, 1 x 1.
		/// </summary>
		public static readonly Texture2D splitterPixel = LoadTexture(
			1,
			new []{EditorGUIUtility.isProSkin ? new Color32(89, 89, 89, 255) : new Color32(116, 116, 116, 255)}
		);

		/// <summary>
		/// The status bar style.
		/// </summary>
		/// 
		/// <remarks>
		/// Text colored light if pro skin, otherwise dark, with darken background anyway.
		/// </remarks>
		/// 
		public static readonly GUIStyle statusBar = new Func<GUIStyle>(() => {
			
			var _c = EditorGUIUtility.isProSkin ? new Color(0.7f, 0.7f, 0.7f) : new Color(0.3f, 0.3f, 0.3f);
			var _p = new []{new Color32(0, 0, 0, 13), new Color32(0, 0, 0, 13), new Color32(0, 0, 0, 140)};

			var _n = new GUIStyleState(){ background = LoadTexture(1, _p), textColor = _c };
			return new GUIStyle(){ border = new RectOffset(0, 0, 2, 1), fixedHeight = 18, normal = _n };
			
		})();

		#endregion


		#region Icons

		/// <summary>
		/// The icon represents to edit, a pencil, 12 x 12.
		/// </summary>
		public static readonly Texture2D editIcon = LoadIcon(new []{
			"        00  ",
			"       0 00 ",
			"      000 0 ",
			"     0 000  ",
			"    0   0   ",
			"   0   0    ",
			"  0   0     ",
			" 0   0      ",
			" 00 0       ",
			" 000        ",
			"            ",
			" 00 00 00 0 "
		});

		/// <summary>
		/// The icon represents to copy, overlapping notes, 12 x 12.
		/// </summary>
		public static readonly Texture2D copyIcon = LoadIcon(new []{
			"    00000   ",
			"    0   00  ",
			"        000 ",
			" 00000    0 ",
			" 0   00   0 ",
			" 0   000  0 ",
			" 0     0  0 ",
			" 0 000 0  0 ",
			" 0     0 00 ",
			" 0 000 0    ",
			" 0     0    ",
			" 0000000    "
		});

		/// <summary>
		/// The icon represents to delete, a trash can, 12 x 12.
		/// </summary>
		public static readonly Texture2D deleteIcon = LoadIcon(new []{
			"   00       ",
			"   0 0 00   ",
			"    0 00 0  ",
			"     0 000  ",
			" 000000 0   ",
			" 00 0 00 0  ",
			"  0 0 0 0 0 ",
			"  0 0 0 000 ",
			"  0 0 0 0   ",
			"  0 0 0 0   ",
			"  0 0 0 0   ",
			"  0000000   "
		});
		
		/// <summary>
		/// The icon represents to aim, a front sight, 12 x 12.
		/// </summary>
		public static readonly Texture2D aimIcon = LoadIcon(new []{
			"     00     ",
			"   000000   ",
			"  0      0  ",
			" 0        0 ",
			" 0   00   0 ",
			"00  0  0  00",
			"00  0  0  00",
			" 0   00   0 ",
			" 0        0 ",
			"  0      0  ",
			"   000000   ",
			"     00     "
		});

		/// <summary>
		/// The icon represents a hierarchy, indent level lines, 12 x 12.
		/// </summary>
		public static readonly Texture2D hierarchyIcon = LoadIcon(new []{
			"            ",
			"            ",
			"00          ",
			"00  00000000",
			" 0          ",
			" 0          ",
			" 00  0000000",
			" 0          ",
			" 0          ",
			" 00  0000000",
			"            ",
			"            "
		});

		/// <summary>
		/// The icon represents a clipboard, 12 x 12.
		/// </summary>
		public static readonly Texture2D clipboardIcon = LoadIcon(new []{
			"     00     ",
			"    0  0    ",
			" 00 0  0 00 ",
			" 0   00   0 ",
			" 0 000000 0 ",
			" 0        0 ",
			" 0        0 ",
			" 0 000    0 ",
			" 0        0 ",
			" 0 00000  0 ",
			" 0        0 ",
			" 0000000000 "
		});

		/// <summary>
		/// The icon represents a question mark, 12 x 12.
		/// </summary>
		public static readonly Texture2D questionIcon = LoadIcon(new []{
			"0          0",
			"   00000    ",
			"  00   00   ",
			"  00   00   ",
			"  00   00   ",
			"      000   ",
			"     00     ",
			"     00     ",
			"            ",
			"     00     ",
			"     00     ",
			"0          0"
		}); //Super Mario !?

		/// <summary>
		/// The icon represents to link, a connected chain, 9 x 9.
		/// </summary>
		public static readonly Texture2D linkIcon = LoadIcon(new []{
			"     000 ",
			"    00 00",
			"        0",
			"  0000 00",
			" 00   00 ",
			"00 0000  ",
			"0        ",
			"00 00    ",
			" 000     "
		}, 116, 75);

		/// <summary>
		/// The icon represents to unlink, a broken chain, 9 x 9.
		/// </summary>
		public static readonly Texture2D unlinkIcon = LoadIcon(new []{
			"0    000 ",
			" 0  00 00",
			"  0 0   0",
			"       00",
			" 00   00 ",
			"00       ",
			"0   0 0  ",
			"00 00  0 ",
			" 000    0"
		}, 116, 75);

		#endregion


		#region Cursors

		/// <summary>
		/// The aux cursor represents a stop mark, 20 x 20.
		/// </summary>
		public static readonly Texture2D stopCursor = LoadCursor(new []{
			"       111111       ",
			"     1100000011     ",
			"    100000000001    ",
			"   10000111100001   ",
			"  100011    110001  ",
			" 1000001      10001 ",
			" 10010001      1001 ",
			"1000110001     10001",
			"1001  10001     1001",
			"1001   10001    1001",
			"1001    10001   1001",
			"1001     10001  1001",
			"10001     1000110001",
			" 1001      10001001 ",
			" 10001      1000001 ",
			"  100011     10001  ",
			"   10000111110001   ",
			"    100000000001    ",
			"     1100000011     ",
			"       111111       "
		});

		/// <summary>
		/// The aux cursor with a plus symbol, 19 x 15.
		/// </summary>
		public static readonly Texture2D copyCursor = LoadCursor(new []{
			"0101010101010      ",
			"1010101010101      ",
			"01         10      ",
			"10         01      ",
			"01      00000000000",
			"10      01111111110",
			"01      01111111110",
			"1010101001111011110",
			"0101010101111011110",
			"        01100000110",
			"        01111011110",
			"        01111011110",
			"        01111111110",
			"        01111111110",
			"        00000000000"
		});

		/// <summary>
		/// The aux cursor with an arrow, 19 x 15.
		/// </summary>
		public static readonly Texture2D linkCursor = LoadCursor(new []{
			"0101010101010      ",
			"1010101010101      ",
			"01         10      ",
			"10         01      ",
			"01      00000000000",
			"10      01111111110",
			"01      01110000110",
			"1010101001111000110",
			"0101010101110000110",
			"        01100010110",
			"        01100111110",
			"        01101111110",
			"        01110111110",
			"        01111111110",
			"        00000000000"
		});

		/// <summary>
		/// The aux cursor represents a dotted frame, 13 x 9.
		/// </summary>
		public static readonly Texture2D moveCursor = LoadCursor(new []{
			"0101010101010",
			"1010101010101",
			"01         10",
			"10         01",
			"01         10",
			"10         01",
			"01         10",
			"1010101010101",
			"0101010101010"
		});

		#endregion


		#region Methods

		/// <summary>
		/// Load a <c>UnityEngine.Texture2D</c> with color <c>pixels</c> for editor usage.
		/// </summary>
		/// 
		/// <remarks>
		/// Return the texture with the same pixels created by this if exists, otherwise create new one.
		/// </remarks>
		/// 
		/// <returns>The texture.</returns>
		/// <param name="width">Width.</param>
		/// <param name="pixels">Pixels.</param>
		/// 
		/*
		 * Set as HideFlags.HideAndDontSave to avoid being destroyed by Unity after reload.
		 * Name the created texture by pixels hash for reusing.
		 */
		public static Texture2D LoadTexture(int width, Color32[] pixels){
			
			if(null == pixels || 0 == pixels.Length) throw new ArgumentException("The pixels can't be null or none.");
			if(0 == width || 0 != pixels.Length % width) throw new ArgumentException("The width doesn't match pixels.");

			var _b = BitConverter.GetBytes(width).Concat(pixels.SelectMany(_v => new []{_v.r, _v.g, _v.b, _v.a}));
			var _n = typeof(EditrixStyle).FullName + Convert.ToBase64String(MD5.Create().ComputeHash(_b.ToArray()));

			var _result = Resources.FindObjectsOfTypeAll<Texture2D>().FirstOrDefault(_v => _n == _v.name);
			if(null != _result) return _result;

			_result = new Texture2D(width, pixels.Length / width, TextureFormat.ARGB32, false){ name = _n };
			_result.hideFlags = HideFlags.HideAndDontSave;

			_result.SetPixels32(pixels);
			_result.Apply();

			return _result;

		}

		/// <summary>
		/// Trick to load a <c>UnityEngine.Texture2D</c> by parsing pixel <c>string</c> array.
		/// </summary>
		/// 
		/// <remarks>
		/// Array length as texture height, element <c>string</c> length as width.
		/// Set each <c>char</c> pixel by <c>colors</c> map if existing, otherwise the <c>other</c> color.
		/// Return the texture with the same pixels created by this if exists, otherwise create new one.
		/// </remarks>
		/// 
		/// <returns>The texture.</returns>
		/// <param name="pixels">Pixel bits.</param>
		/// <param name="colors">Colors map.</param>
		/// <param name="other">Other.</param>
		/// 
		public static Texture2D LoadTexture(string[] pixels, Dictionary<char, Color32> colors, Color32 other){

			if(null == pixels || 0 == pixels.Length) throw new ArgumentException("The pixels can't be null or none.");

			if(pixels.Any(_v => null == _v)) throw new ArgumentException("Each line of pixels must be set.");

			if(1 < pixels.GroupBy(_v => _v.Length).Count()) throw new ArgumentException("Line lengths must be same.");

			var _p = pixels.Reverse().SelectMany(_v => _v).Select(_v => colors.ContainsKey(_v) ? colors[_v] : other);

			return LoadTexture(pixels[0].Length, _p.ToArray());

		}

		/// <summary>
		/// Trick to load icon <c>UnityEngine.Texture2D</c> by parsing pixel <c>string</c> array.
		/// </summary>
		/// 
		/// <remarks>
		/// Array length as icon height, element <c>string</c> length as width.
		/// Any space char as transparent, others color light if pro skin, otherwise dark.
		/// Return the texture with the same pixels created by this if exists, otherwise create new one.
		/// </remarks>
		/// 
		/// <returns>The icon <c>UnityEngine.Texture2D</c>.</returns>
		/// <param name="pixels">Pixel bits.</param>
		/// <param name="pro">Grayscale for pro skin.</param>
		/// <param name="free">Grayscale for free skin.</param>
		/// 
		public static Texture2D LoadIcon(string[] pixels, byte pro = 196, byte free = 60){

			var _c = EditorGUIUtility.isProSkin ? new Color32(pro, pro, pro, 255) : new Color32(free, free, free, 255);

			return LoadTexture(pixels, new Dictionary<char, Color32>(){{' ', new Color32()}}, _c);
			
		}

		/// <summary>
		/// Trick to load cursor <c>UnityEngine.Texture2D</c> by parsing pixel <c>string</c> array.
		/// </summary>
		/// 
		/// <remarks>
		/// Array length as cursor height, element <c>string</c> length as width.
		/// Any '1' char as white, '0' as black, otherwise transparent.
		/// Return the texture with the same pixels created by this if exists, otherwise create new one.
		/// </remarks>
		/// 
		/// <returns>The cursor <c>UnityEngine.Texture2D</c>.</returns>
		/// <param name="pixels">Pixel bits.</param>
		/// 
		public static Texture2D LoadCursor(string[] pixels){

			var _c = new Dictionary<char, Color32>(){{'1', (Color32)Color.white}, {'0', (Color32)Color.black}};

			return LoadTexture(pixels, _c, new Color32());

		}

		#endregion

	}

}
