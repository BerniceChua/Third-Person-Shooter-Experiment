
/*WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW*\     (   (     ) )
|/                                                      \|       )  )   _((_
||  (c) Wanzyee Studio  < wanzyeestudio.blogspot.com >  ||      ( (    |_ _ |=n
|\                                                      /|   _____))   | !  ] U
\.ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ All rights reserved./  (_(__(S)   |___*/

using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Object = UnityEngine.Object;

namespace WanzyeeStudio.Editrix.Toolkit{

	public partial class Clipboard{

		/// <summary>
		/// Handle data and search operations for <c>Clipboard</c>.
		/// </summary>
		private static class Filer{

			#region Fields

			/// <summary>
			/// The temp directory to store scene copies' data, e.g., display name, note, source, etc.
			/// </summary>
			private const string _DATA_ROOT = "Temp/WanzyeeStudio/Clipboard/Data";

			#endregion


			#region Methods

			/// <summary>
			/// Get the hierarchy path in one line.
			/// </summary>
			/// <returns>The hierarchy path.</returns>
			/// <param name="transform">Transform.</param>
			public static string GetHierarchyPath(Transform transform){

				var _result = transform.name;

				for(var t = transform.parent; null != t; t = t.parent) _result = t.name + "/" + _result;

				return _result.Replace('\n', '\t');

			}

			/// <summary>
			/// Set the source info data of specified copy, include asset guid and hierarchy or name.
			/// </summary>
			/// <param name="copy">Copied object.</param>
			/// <param name="source">Source object.</param>
			public static void SetSource(Object copy, Object source){

				var _s = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetOrScenePath(source)) + ":";

				if(source is Component) _s += GetHierarchyPath(((Component)source).transform);

				else _s += (string.IsNullOrEmpty(source.name) ? source.GetType().Name : source.name);

				WriteData(copy, (_s + "\n" + GetNote(copy)).Trim());

			}

			/// <summary>
			/// Get the source of specified copy.
			/// Return <c>UnityEngine.GameObject</c> if the copy is an <c>UnityEngine.Component</c>.
			/// </summary>
			/// <returns>The source object.</returns>
			/// <param name="copy">Copied object.</param>
			public static Object GetSource(Object copy){

				var _s = new Regex(":").Split(ReadData(copy).Split('\n')[0], 2);
				if(2 > _s.Length) return null;

				var _p = AssetDatabase.GUIDToAssetPath(_s[0]);
				var _a = AssetDatabase.LoadAssetAtPath<Object>(_p);

				if((null == _a && !string.IsNullOrEmpty(_p)) || (!(copy is Component))) return _a;

				var _g = Resources.FindObjectsOfTypeAll<GameObject>().AsEnumerable();
				if(!string.IsNullOrEmpty(_p)) _g = _g.Where(_v => AssetDatabase.GetAssetOrScenePath(_v) == _p);

				_g = _g.Where(_v => _s[1] == GetHierarchyPath(_v.transform));
				return _g.OrderBy(_v => null == _v.GetComponent(copy.GetType())).FirstOrDefault() ?? _a;

			}

			/// <summary>
			/// Get the note of specified copy, optional to get source info as default if not existing.
			/// </summary>
			/// <returns>The note.</returns>
			/// <param name="copy">Copied object.</param>
			/// <param name="must">If set to <c>true</c> must.</param>
			public static string GetNote(Object copy, bool must = false){

				var _d = new Regex("\n").Split(ReadData(copy).Trim(), 2);

				#if !WZ_LITE
				if(1 < _d.Length) return _d[1].Trim();
				#endif

				if(!must) return "";

				var _s = new Regex(":").Split(_d[0], 2);
				var _n = (1 < _s.Length) ? _s[1] : copy.GetType().Name;

				return (_n + "\n" + AssetDatabase.GUIDToAssetPath(_s[0].Trim())).Trim();

			}

			/// <summary>
			/// Set the data of specified copy from another.
			/// </summary>
			/// <param name="copy">Copied object.</param>
			/// <param name="source">Source object.</param>
			public static void SetData(Object copy, Object source){
				WriteData(copy, ReadData(source));
			}

			/// <summary>
			/// Write the data of specified copy, 1st line as the source info, and note from the 2nd line.
			/// Use <c>AssetImporter.userData</c> for an asset, or temp file for a scene object.
			/// </summary>
			/// <param name="copy">Copied object.</param>
			/// <param name="data">Data.</param>
			private static void WriteData(Object copy, string data){
				
				var _i = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(copy));

				if(null != _i){
					_i.userData = data;
					_i.SaveAndReimport();
				}else{
					Directory.CreateDirectory(_DATA_ROOT);
					File.WriteAllText(Path.Combine(_DATA_ROOT, copy.GetInstanceID().ToString()), data);
				}

				Manager.refresh.Invoke();

			}

			/// <summary>
			/// Read the data of specified copy.
			/// </summary>
			/// <returns>The data.</returns>
			/// <param name="copy">Copied object.</param>
			private static string ReadData(Object copy){

				var _i = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(copy));
				if(null != _i) return _i.userData;

				var _p = Path.Combine(_DATA_ROOT, copy.GetInstanceID().ToString());
				return File.Exists(_p) ? File.ReadAllText(_p) : "";

			}

			#endregion


			#if !WZ_LITE

			/// <summary>
			/// Set the note of specified copy.
			/// </summary>
			/// <param name="copy">Copied object.</param>
			/// <param name="note">Note.</param>
			public static void SetNote(Object copy, string note){
				WriteData(copy, ReadData(copy).Split('\n')[0] + "\n" + note.Trim());
			}

			/// <summary>
			/// Get the filter includes all types of selected objects and the original labels.
			/// </summary>
			/// <returns>The selection filter.</returns>
			/// <param name="filter">Filter.</param>
			public static string GetSelectionFilter(string filter){

				var _f = Regex.Replace(filter, @"(^|\s)t:\S+(?=(\s|$))", "").Trim();

				var _c = Selection.gameObjects.SelectMany(_v => _v.GetComponents<Component>()).Cast<Object>();

				var _t = Selection.objects.Union(_c).Where(IsCopyable).Select(_v => _v.GetType().FullName).Distinct();

				return (_f + "    " + string.Join(" ", _t.Select(_v => "t:" + _v).ToArray())).Trim();

			}

			/// <summary>
			/// Get the <c>groups</c> items filtered by specified pattern.
			/// </summary>
			/// <returns>The items.</returns>
			/// <param name="groups">Groups.</param>
			/// <param name="filter">Filter.</param>
			public static IEnumerable<KeyValuePair<Group, Item[]>> FilterItems(Group[] groups, string filter){

				var _t = CombineFilter(filter, @"(?<=(^|\s)t:)\S+(?=(\s|$))", false);
				var _l = CombineFilter(filter, @"(?<=(^|\s)l:)\S+(?=(\s|$))", true);
				var _n = CombineFilter(filter, @"(?<=(^|\s)(?!(t:|l:)))\S+(?=(\s|$))", true);

				Func<IEnumerable<Item>, Item[]> _f = (i) => {
					if(null != _l) i = i.Where(_v => _l.IsMatch(_v.label.tooltip.Substring(_v.label.text.Length)));
					return ((null != _n) ? i.Where(_v => _n.IsMatch(_v.label.text)) : i).ToArray();
				};

				var _g = (null != _t) ? groups.Where(_v => _t.IsMatch(_v.label.tooltip)) : groups;
				return _g.ToDictionary(_v => _v, _v => _f.Invoke(_v.items)).Where(_v => _v.Value.Any());

			}

			/// <summary>
			/// Show the menu to switch search filter by selecting type, name, or label, and send the new back.
			/// </summary>
			/// <param name="groups">Groups.</param>
			/// <param name="filter">Filter.</param>
			/// <param name="callback">Callback to receive new search filter.</param>
			/*
			 * Use callback to delay return new filter, since lambda doesn't allow "ref".
			 * And passing the Clipboard instance might be inappropriate.
			 */
			public static void ShowFilterMenu(Group[] groups, string filter, Action<string> callback){

				var _m = new GenericMenu();
				var _t = groups.Select(_v => new []{_v.label.text, "t:" + _v.label.tooltip});

				AddFilterMenu(_m, "Type/", @"(^|\s)t:\S+(?=(\s|$))", _t, filter, callback);

				var _i = FilterItems(groups, filter).SelectMany(_v => _v.Value).Select(_v => _v.label);
				var _n = SplitWords(_i.Select(_v => _v.text)).Select(_v => new []{_v, _v});

				AddFilterMenu(_m, "Name/", @"(^|\s)(?!(t:|l:))\S+(?=(\s|$))", _n, filter, callback);

				var _w = SplitWords(_i.Select(_v => _v.tooltip.Substring(_v.text.Length)));
				var _l = _w.Select(_v => new []{_v, "l:" + _v});

				AddFilterMenu(_m, "Label/", @"(^|\s)l:\S+(?=(\s|$))", _l, filter, callback);

				_m.ShowAsContext();

			}

			/// <summary>
			/// Add the menu items to switch specified <c>string</c> value in the <c>filter</c> and send the new back.
			/// </summary>
			/// <param name="menu">Menu.</param>
			/// <param name="root">Root name.</param>
			/// <param name="cleaner">Cleaner pattern.</param>
			/// <param name="items">Menu name and value pairs.</param>
			/// <param name="filter">Filter.</param>
			/// <param name="callback">Callback to receive new search filter.</param>
			private static void AddFilterMenu(
				GenericMenu menu,
				string root,
				string cleaner,
				IEnumerable<string[]> items,
				string filter,
				Action<string> callback
			){
				
				var _a = Regex.Replace(filter, cleaner, "").Trim();
				menu.AddItem(new GUIContent(root + "All"), false, () => callback.Invoke(_a));

				if(!items.Any()) return;
				menu.AddSeparator(root + "");

				foreach(var _v in items){

					var _r = new Regex(@"(^|\s)" + _v[1] + @"(?=(\s|$))", RegexOptions.IgnoreCase);
					var _m = _r.IsMatch(filter);

					var _f = (_m ? _r.Replace(filter, "") : (_v[1] + " " + filter)).Trim();
					menu.AddItem(new GUIContent(root + _v[0]), _m, () => callback.Invoke(_f));

				}

			}

			/// <summary>
			/// Get a combo filter to match each word found in the input <c>string</c>.
			/// Optional to return a filter for matching all the words, otherwise for matching any of them.
			/// </summary>
			/// <returns>The combo filter.</returns>
			/// <param name="input">Input string.</param>
			/// <param name="pattern">Pattern to match words.</param>
			/// <param name="all">If set to <c>true</c> match all.</param>
			private static Regex CombineFilter(string input, string pattern, bool all){

				var _o = RegexOptions.IgnoreCase;

				var _m = Regex.Matches(input, pattern, _o);

				var _w = _m.Cast<Match>().Select(_v => Regex.Escape(_v.Value));

				if(!_w.Any()) return null;

				else if(!all) return new Regex("(" + string.Join("|", _w.ToArray()) + ")", _o);

				else return new Regex(string.Join("", _w.Select(_v => "(?=(.|\n)*" + _v + ")").ToArray()), _o);

			}

			/// <summary>
			/// Split all words from the given texts.
			/// </summary>
			/// <returns>The words.</returns>
			/// <param name="texts">Texts.</param>
			private static IEnumerable<string> SplitWords(IEnumerable<string> texts){

				var _i = CultureInfo.InvariantCulture.TextInfo;

				var _w = texts.SelectMany(_v => Regex.Split(_v, @"\W+")).Where(_v => !string.IsNullOrEmpty(_v));

				return _w.Select(_v => _i.ToTitleCase(_v.ToLower())).Distinct().OrderBy(_v => _v);

			}

			#endif

		}

	}

}
