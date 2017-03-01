
/*WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW*\     (   (     ) )
|/                                                      \|       )  )   _((_
||  (c) Wanzyee Studio  < wanzyeestudio.blogspot.com >  ||      ( (    |_ _ |=n
|\                                                      /|   _____))   | !  ] U
\.ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ All rights reserved./  (_(__(S)   |___*/

using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Object = UnityEngine.Object;

namespace WanzyeeStudio.Editrix.Toolkit{
	
	/// <summary>
	/// Utility to copy <c>UnityEngine.Component</c> or <c>UnityEngine.Material</c> and paste it back.
	/// </summary>
	/// 
	/// <remarks>
	/// Useful for tweaking lots of objects in the editor, even in play mode.
	/// Click the menu "Window/Clipboard" to open the window.
	/// Just play and tweak, drag and drop, copy and paste whenever.
	/// It acts as sort of preset system, edit lots of copies as presets, and paste to apply values quickly.
	/// Easy to find and manage copies with regex search filter, custom item label, and foldable inspector.
	/// </remarks>
	/// 
	/// <remarks>
	/// To copy:
	/// 	1. Drag'n'Drop from "Inspector" to copy the inspected sources.
	/// 	2. Drag'n'Drop a <c>UnityEngine.GameObject</c> to copy the components on it.
	/// 	3. Show menu to specify the component type if "Ctrl" pressed when dropping gameObjects.
	/// 	4. Or click the context menu "Copy to Clipboard".
	/// </remarks>
	/// 
	/// <remarks>
	/// To paste:
	/// 	1. Drag'n'Drop to "Inspector" to paste back to the inspected targets.
	/// 	2. When dragging a component, it pastes values to the first one on the gameObject, or pastes as new if none.
	/// 	3. Show menu to specify a target of multiple components if "Ctrl" pressed when dropping.
	/// 	4. Or click the "Copy" button above any item, and paste by the target's context menu.
	/// </remarks>
	/// 
	/// <remarks>
	/// To filter items with the search bar:
	/// 	1. Click the "Magnifier" icon to show a context menu made from the current items to select filters easily.
	/// 	2. Or input any text to filter the item's name, just like the Project window's search bar.
	/// 	3. Prefix "t:" filters by the type, the search will include all specified types.
	/// 	4. Prefix "l:" filters by the tooltip as labels, an item has to match all specified labels.
	/// 	5. Toggle the "Link" icon at the top-right of window to filter automatically by tracking selection.
	/// </remarks>
	/// 
	/// <remarks>
	/// To edit the item label:
	/// 	1. Click the "Pen" button beside the label to show or hide the edit field.
	/// 	2. To save the change, just press "Ctrl-Enter" keys or unfocus the field after editing.
	/// 	3. The first line shows as the label title, and the full text is the tooltip.
	/// 	4. Leave the field empty to show the default text, i.e., the copy source path.
	/// </remarks>
	/// 
	/// <remarks>
	/// To find the copy source object:
	/// 	1. Click the "Aim" button to ping it or double-click to select it.
	/// 	2. The saved trace path is also shown as the default label tooltip.
	/// 	3. It'll beep if not found, e.g., the trace may be lost if the source is moved or renamed.
	/// </remarks>
	/// 
	/// <remarks>
	/// The reasons not to save the trace by references below:
	/// 	1. A scene object reference will change when load a scene.
	/// 	2. We can't save the scene reference in the project assets.
	/// 	3. We shouldn't save the edit data in user's game scene.
	/// </remarks>
	/// 
	/// <remarks>
	/// For component references to scene object.
	/// This creates copies and store in editor scene temporarily to ensure content correct.
	/// It means the copy will be destroyed when quitting the editor.
	/// And also, the scene references will become missing when opening another scene.
	/// The situation above is applicable to a material with scene texture, too.
	/// </remarks>
	/// 
	/// <remarks>
	/// For material or component without reference to any scene object.
	/// The copies will be saved in an asset folder to make them still until "Clear" triggered manually.
	/// This tracks all copies by specific name or folder to ensure valid after script reloaded.
	/// </remarks>
	/// 
	/// <remarks>
	/// SVN users may clear all manually to avoid committing, or ignore the storing folder below:
	/// 	1. The default is "Temp/Clipboard" under the root folder "Assets/WanzyeeStudio".
	/// 	2. If the root is moved, it becomes "Temp/Clipboard" under the first found "WanzyeeStudio".
	/// 	3. If there's no "WanzyeeStudio" folder, it'll be "Assets/Temp/Clipboard".
	/// </remarks>
	/// 
	/// <remarks>
	/// Instructions, to copy and paste generic component is dangerous, even if reflect all the fields.
	/// Since we'll never know what the developer do when the component awake.
	/// As the <a href="http://answers.unity3d.com/answers/1013926/view.html" target="_blank">thread</a>
	/// I commented, we'd be very careful with which aren't made by ourselves.
	/// Finally, this was created, works in the editor with Unity built-in classes and API.
	/// </remarks>
	/// 
	public partial class Clipboard : EditorWindow, IHasCustomMenu{
		
		#region Menu
		
		/// <summary>
		/// Copy the <c>UnityEngine.Component</c> into clipboard.
		/// </summary>
		/// <param name="command">Command.</param>
		[MenuItem("CONTEXT/Component/Copy to Clipboard", false, 700)]
		private static void ComponentCopyToClipboard(MenuCommand command){
			Copy(command.context, true);
		}
		
		/// <summary>
		/// Copy the <c>UnityEngine.Material</c> into clipboard.
		/// </summary>
		/// <param name="command">Command.</param>
		[MenuItem("CONTEXT/Material/Copy to Clipboard", false, 700)]
		private static void MaterialCopyToClipboard(MenuCommand command){
			Copy(command.context, true);
		}
		
		/// <summary>
		/// Check if <c>ComponentCopyToClipboard()</c> or <c>MaterialCopyToClipboard()</c> valid.
		/// The context is copyable.
		/// </summary>
		/// <returns><c>true</c>, if valid.</returns>
		/// <param name="command">Command.</param>
		[MenuItem("CONTEXT/Component/Copy to Clipboard", true)]
		[MenuItem("CONTEXT/Material/Copy to Clipboard", true)]
		private static bool CopyToClipboardValid(MenuCommand command){
			return IsCopyable(command.context);
		}
		
		/// <summary>
		/// Show the clipboard window.
		/// </summary>
		[MenuItem("Window/Clipboard", false, 2100)]
		public static void OpenWindow(){
			GetWindow<Clipboard>();
		}
		
		#endregion

		
		#region Static Methods
		
		/// <summary>
		/// Determine if the specified source is able to copy to clipboard.
		/// </summary>
		/// <returns><c>true</c> if is copyable; otherwise, <c>false</c>.</returns>
		/// <param name="source">Source object.</param>
		public static bool IsCopyable(Object source){

			try{ Manager.CheckCreatable(source); }
			catch{ return false; }
			return true;

		}
		
		/// <summary>
		/// Copy the specified source to clipboard.
		/// </summary>
		/// 
		/// <remarks>
		/// Optional to open window after copying.
		/// </remarks>
		/// 
		/// <param name="source">Source object.</param>
		/// <param name="open">If set to <c>true</c> open.</param>
		/// 
		public static void Copy(Object source, bool open = false){
			Manager.Create(source);
			if(open) OpenWindow();
		}
		
		/// <summary>
		/// Clear clipboard by specified type, or pass <c>null</c> to clear all.
		/// </summary>
		public static void Clear(Type type = null){
			Manager.RemoveAll(type, false);
		}

		/// <summary>
		/// Show a dialog with tooltip and the button to open online manual.
		/// </summary>
		private static void OpenAbout(){

			var _m = (

				"Clipboard for Component and Material." +
				#if WZ_LITE
				"\nSome features are only available in Full version." +
				#endif

				"\n\nCopy:" +
				"\n- Click their context menu 'Copy to Clipboard'." +
				#if !WZ_LITE
				"\n- Or Drag'n'Drop into Clipboard to grab a snapshot." +
				#endif
				"\n- Note, a scene or runtime reference may be lost when opening a scene or when stopping." +

				"\n\nPaste:" +
				"\n- Click an item 'Copy' button, then context menu 'Paste Component Values' or 'Paste Properties'." +
				#if !WZ_LITE
				"\n- Or Drag'n'Drop into Inspector to paste back." +
				#endif

				"\n\nPlease read the manual for detail."

			);

			var _o = EditorUtility.DisplayDialog("About Clipboard", _m, "Online Manual", "Close");
			if(_o) Help.BrowseURL("https://git.io/viqBd");

		}

		#endregion


		#region Static Fields
		
		/// <summary>
		/// The expand button on toolbars.
		/// </summary>
		private static GUIContent _expandBtn;
		
		/// <summary>
		/// The clear button on main toolbar.
		/// </summary>
		private static GUIContent _clearBtn;

		#endregion


		#region Fields

		/// <summary>
		/// Flag to track selection to change search filter automatically.
		/// </summary>
		[Tooltip("If to track selection to change search filter automatically.")]
		public bool track;

		/// <summary>
		/// The search filter pattern in the search bar.
		/// </summary>
		[Tooltip("Search filter pattern.")]
		public string search = "";

		/// <summary>
		/// The stored search pattern to check if to update <c>_items</c>.
		/// </summary>
		private string _filter = "";

		/// <summary>
		/// The items filtered from <c>_group</c> to draw GUI.
		/// </summary>
		private IEnumerable<KeyValuePair<Group, Item[]>> _items;
		
		/// <summary>
		/// The groups of all loaded copy items.
		/// </summary>
		/*
		 * Serialize to keep expand states after exiting editor.
		 */
		[Obfuscation(Exclude = true)]
		[SerializeField]
		[Tooltip("Groups of all loaded copy items.")]
		private Group[] _groups = new Group[0];

		/// <summary>
		/// Flag to refresh, set when initialize or copies possible changed.
		/// Checked to trigger refresh when <c>OnGUI()</c>.
		/// </summary>
		private bool _refresh;

		/// <summary>
		/// The repaint time, used to reduce invoking by <c>Update</c>.
		/// </summary>
		private double _repaint;
		
		/// <summary>
		/// The GUI scroll position.
		/// </summary>
		private Vector2 _scroll;

		#endregion


		#region Message Methods
		
		/// <summary>
		/// OnEnable, initialize styles, set window title and the min size, then register callbacks.
		/// </summary>
		private void OnEnable(){

			if(null == _expandBtn) _expandBtn = new GUIContent(EditrixStyle.hierarchyIcon, "Expand / Fold all");
			if(null == _clearBtn) _clearBtn = new GUIContent(EditrixStyle.deleteIcon, "Clear all");

			titleContent = new GUIContent(" Clipboard", EditrixStyle.clipboardIcon);
			minSize = new Vector2(275f, 150f);

			wantsMouseMove = true;
			_refresh = true;

			Manager.refresh += MarkRefresh;
			Selection.selectionChanged += SearchSelection;
			
			#if WZ_LITE
			Limiter.CheckInitialize(this);
			#endif

		}

		/// <summary>
		/// OnDisable, deregister all callbacks.
		/// </summary>
		private void OnDisable(){
			Manager.refresh -= MarkRefresh;
			Selection.selectionChanged -= SearchSelection;
		}

		/// <summary>
		/// OnLostFocus, trigger all items to check save note if changed.
		/// </summary>
		private void OnLostFocus(){
			#if !WZ_LITE
			foreach(var _v in _groups.SelectMany(_o => _o.items)) _v.SaveNote(true);
			#endif
		}

		/// <summary>
		/// Update, repaint displayed values and check if all the copies are still in a lower frequency.
		/// And trigger to reload ASAP when the refresh event occurs.
		/// </summary>
		private void Update(){
			if(_refresh || 1f < EditorApplication.timeSinceStartup - _repaint) Repaint();
		}

		/// <summary>
		/// ShowButton, show a button at the top-right corner to switch track state.
		/// </summary>
		/// <param name="r">Rect.</param>
		/*
		 * https://leahayes.wordpress.com/2013/04/30/adding-the-little-padlock-button-to-your-editorwindow/
		 */
		private void ShowButton(Rect r){

			var _e = GUI.enabled;

			#if WZ_LITE
			GUI.enabled = false;
			var _c = new GUIContent(EditrixStyle.linkIcon, "Track Selection [Full Only]");
			#else
			var _c = new GUIContent(track ? EditrixStyle.linkIcon : EditrixStyle.unlinkIcon, "Track Selection");
			#endif

			EditorGUI.BeginChangeCheck();
			track = GUI.Toggle(new Rect(r.x, r.y + 2f, r.width, r.height - 5f), track, _c, GUIStyle.none);

			if(EditorGUI.EndChangeCheck() && track) SearchSelection();
			GUI.enabled = _e;

		}

		/// <summary>
		/// OnGUI, mainly control process like update.
		/// Check refresh, draw contents, check Drag'n'Drop operations in ordered.
		/// </summary>
		private void OnGUI(){

			_repaint = EditorApplication.timeSinceStartup;
			if(Event.current.type == EventType.MouseMove) Repaint();

			#if !WZ_LITE
			Dragger.CheckDropPaste();
			#endif

			CheckRefresh();
			DrawToolbar();
			DrawContent();
			
			#if !WZ_LITE
			Dragger.CheckDragCopy();
			#endif

		}

		#endregion


		#region Methods

		/// <summary>
		/// Add the window context menu items.
		/// </summary>
		/// 
		/// <remarks>
		/// Menu "Help" to open the online manual, and "New window" to open another <c>Clipboard</c> window.
		/// </remarks>
		/// 
		/// <param name="menu">Menu.</param>
		/// 
		public void AddItemsToMenu(GenericMenu menu){
			
			menu.AddItem(new GUIContent("About"), false, OpenAbout);

			#if WZ_LITE
			menu.AddDisabledItem(new GUIContent("New window [Full only]"));
			#else
			menu.AddItem(new GUIContent("New window"), false, () => CreateInstance<Clipboard>().Show());
			#endif

		}

		/// <summary>
		/// Update search filter by selection if enabled.
		/// </summary>
		private void SearchSelection(){
			#if !WZ_LITE
			if(track){
				search = Filer.GetSelectionFilter(search);
				Repaint();
			}
			#endif
		}

		/// <summary>
		/// Callback method for <c>Manager.refresh</c> to mark the <c>_refresh</c> flag to reload later.
		/// </summary>
		/*
		 * Repaint twice, Update() and delayCall, for Item to update and show buttons.
		 */
		private void MarkRefresh(){
			_refresh = true;
			EditorApplication.delayCall += Repaint;
		}

		/// <summary>
		/// Check if refresh the groups and keep the expand state.
		/// </summary>
		private void CheckRefresh(){

			if(EventType.Layout != Event.current.type) return;

			var _i = _groups.SelectMany(_v => _v.items);
			var _r = !_refresh && _i.Any(_v => null == _v.copy);
			if(!_r && !_refresh) return;

			var _g = _groups.Select(_v => _v.expand ? null : _v.items.FirstOrDefault(_o => null != _o.copy));
			var _t = _g.Where(_v => null != _v).Select(_v => _v.copy.GetType()).ToList();
			var _c = _i.Where(_v => null != _v.copy).ToDictionary(_v => _v.copy, _v => _v.expand);

			_groups = Manager.LoadGroups(_r);
			_items = null;
			_refresh = false;

			foreach(var _v in _groups){
				if(_t.Contains(_v.type)) _v.expand = false;
				foreach(var _o in _v.items) _o.expand = _c.ContainsKey(_o.copy) ? _c[_o.copy] : 3;
			}

		}

		#endregion


		#region Draw Methods

		/// <summary>
		/// Draw the main toolbar, include the search field and all functional buttons.
		/// </summary>
		private void DrawToolbar(){

			GUILayout.BeginHorizontal(EditorStyles.toolbar);
			DrawSearch();

			GUILayout.FlexibleSpace();
			EditorGUILayout.Space();
			DrawButtons();

			GUILayout.Space(19f - EditorGUIUtility.currentViewWidth + position.width); //align items
			GUILayout.EndHorizontal();

		}

		/// <summary>
		/// Draw the search bar to edit or select the search pattern.
		/// </summary>
		private void DrawSearch(){

			#if WZ_LITE

			var _l = new GUIContent(_filter, "Search [Full only]");
			var _e = GUI.enabled;
			GUI.enabled = false;

			GUILayout.Label(_l, "ToolbarSeachTextFieldPopup", GUILayout.MaxWidth(285));
			GUILayout.Label(_l, "ToolbarSeachCancelButton");
			GUI.enabled = _e;

			#else

			var _r = new Rect(0f, 0f, 25f, EditorGUIUtility.singleLineHeight);
			var _m = GUI.Button(_r, new GUIContent("", "Search by..."), GUIStyle.none);
			EditorGUIUtility.AddCursorRect(_r, MouseCursor.Arrow);

			search = EditorGUILayout.TextField(search, (GUIStyle)"ToolbarSeachTextFieldPopup", GUILayout.MaxWidth(285));
			var _c = GUILayout.Button("", "ToolbarSeachCancelButton");

			if(_m || _c) GUI.FocusControl(null);
			if(_m) Filer.ShowFilterMenu(_groups, search, (s) => search = s);
			if(_c) search = "";

			#endif

		}

		/// <summary>
		/// Draw the fold and clear buttons on the toolbar.
		/// </summary>
		private void DrawButtons(){

			var _s = EditorStyles.toolbarButton;
			var _w = GUILayout.Width(22f);
			
			if(GUILayout.Button(_expandBtn, _s, _w)){
				
				if(!_groups.Any(_v => _v.expand)){
					foreach(var _v in _groups) _v.expand = true;
					foreach(var _v in _groups.SelectMany(_o => _o.items)) _v.expand = 3;
				}else{
					var _i = _groups.Where(_v => _v.expand).SelectMany(_v => _v.items);
					if(_i.Any(_v => 0 != _v.expand)) foreach(var _v in _i) _v.expand = 0;
					else foreach(var _v in _groups) _v.expand = false;
				}

			}

			if(GUILayout.Button(_clearBtn, _s, _w) && _groups.Any()){
				EditorApplication.Beep();
				var _m = "\nClearing is irreversible!\n\nAre you sure to delete all copies?";
				if(EditorUtility.DisplayDialog("Clear Clipboard", _m, "OK")) Manager.RemoveAll(null, true);
			}

		}

		/// <summary>
		/// Draw the content of copies inside window with scrolling if existing, otherwise show a help box.
		/// </summary>
		private void DrawContent(){

			#if WZ_LITE
			if(null == _items && _groups.Any()) _items = _groups.ToDictionary(_v => _v, _v => _v.items);
			#else
			if((_filter != search || null == _items) && _groups.Any()) _items = Filer.FilterItems(_groups, search);
			_filter = search;
			#endif

			_scroll = GUILayout.BeginScrollView(_scroll);

			if(null == _items) EditorGUILayout.HelpBox("Clipboard for Component and Material.", MessageType.Info);
			else if(!_items.Any()) EditorGUILayout.HelpBox("No item matches the search pattern.", MessageType.Warning);
			else DrawItems();

			GUILayout.Space(20f);
			GUILayout.EndScrollView();

		}

		/// <summary>
		/// Draw the items belong paired group, and an end line if need.
		/// </summary>
		private void DrawItems(){

			foreach(var _v in _items){
				_v.Key.Draw();
				if(_v.Key.expand) foreach(var _o in _v.Value) _o.Draw();
			}

			var _l = _items.Last().Key.expand ? _items.Last().Value.LastOrDefault() : null;
			if(null == _l || 0 == _l.expand || 2 == (_l.expand & 2)) return;

			GUI.DrawTexture(GUILayoutUtility.GetRect(1f, 1f), EditrixStyle.splitterPixel);

		}

		#endregion

	}

}
