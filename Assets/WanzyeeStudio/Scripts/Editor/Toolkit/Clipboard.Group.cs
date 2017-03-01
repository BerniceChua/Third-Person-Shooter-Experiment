
/*WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW*\     (   (     ) )
|/                                                      \|       )  )   _((_
||  (c) Wanzyee Studio  < wanzyeestudio.blogspot.com >  ||      ( (    |_ _ |=n
|\                                                      /|   _____))   | !  ] U
\.ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ All rights reserved./  (_(__(S)   |___*/

using UnityEditor;
using UnityEngine;
using System;
using System.Linq;

namespace WanzyeeStudio.Editrix.Toolkit{
	
	public partial class Clipboard{
		
		/// <summary>
		/// Group includes copied items of specified type.
		/// To draw GUI with expandable label and all items within.
		/// </summary>
		[Serializable]
		private class Group{

			#region Fields
			
			/// <summary>
			/// The toolbar button to clear whole group.
			/// </summary>
			private static readonly GUIContent _editBtn = new GUIContent("X", "Clear group");

			/// <summary>
			/// The expand state.
			/// </summary>
			/*
			 * Public as Serializable for Clipboard restoring expand state, also as items and label.
			 */
			public bool expand = true;

			/// <summary>
			/// The copied items to draw.
			/// </summary>
			public Item[] items;

			/// <summary>
			/// The label to show on expandable toolbar.
			/// </summary>
			public readonly GUIContent label;

			/// <summary>
			/// The type of copied items.
			/// </summary>
			public readonly Type type;

			#endregion


			#region Methods

			/// <summary>
			/// Initialize each value specified.
			/// </summary>
			/// <param name="type">Type.</param>
			/// <param name="items">Items.</param>
			public Group(Type type, Item[] items){
				
				this.type = type;
				this.items = items;

				label = new GUIContent(type.Name, type.FullName);
				if(typeof(Component) == type || typeof(Behaviour) == type){
					
					var _t = items.Select(_v => ObjectNames.GetInspectorTitle(_v.copy).Replace(" ", ""));
					label.text = label.tooltip = string.Join("/", _t.Distinct().ToArray());

				}

			}

			/// <summary>
			/// Draw GUI includes label bar with buttons, and items if expand.
			/// </summary>
			public void Draw(){

				EditorGUILayout.Space();

				var _c = GUI.backgroundColor;
				var _a = _c * new Color(1f, 1f, 1f, 0.7f);
				
				GUI.backgroundColor = _a;
				GUILayout.BeginHorizontal(EditorStyles.toolbar);

				GUI.backgroundColor = _c;
				DrawLabel();

				GUI.backgroundColor = _a;
				DrawButtons();
				
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUI.backgroundColor = _c;

			}

			/// <summary>
			/// Draw the label, clicked to switch expand.
			/// </summary>
			private void DrawLabel(){
				
				var _s = new GUIStyle(EditorStyles.foldout){ clipping = TextClipping.Clip, fontStyle = FontStyle.Bold };
				
				var _w = GUILayout.Width(EditorGUIUtility.currentViewWidth - 86f);

				expand = GUILayout.Toggle(expand, label, _s, _w);

			}

			/// <summary>
			/// Draws the expand items and remove group buttons.
			/// </summary>
			private void DrawButtons(){

				var _s = EditorStyles.toolbarButton;
				var _w = GUILayout.Width(22f);

				GUILayout.Space(8f);

				if(GUILayout.Button(_expandBtn, _s, _w)){
					if(expand){
						var _e = items.Any(_v => 0 != _v.expand) ? 0 : 3;
						foreach(var _v in items) _v.expand = _e;
					}else{
						expand = true;
					}
				}

				if(GUILayout.Button(_editBtn, _s, _w)) Manager.RemoveAll(type, true);
				
			}

			#endregion

		}

	}

}
