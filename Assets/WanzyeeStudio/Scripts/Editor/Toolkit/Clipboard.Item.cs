
/*WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW*\     (   (     ) )
|/                                                      \|       )  )   _((_
||  (c) Wanzyee Studio  < wanzyeestudio.blogspot.com >  ||      ( (    |_ _ |=n
|\                                                      /|   _____))   | !  ] U
\.ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ All rights reserved./  (_(__(S)   |___*/

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Object = UnityEngine.Object;

namespace WanzyeeStudio.Editrix.Toolkit{
	
	public partial class Clipboard{
		
		/// <summary>
		/// Item of editor targets to specified copied asset.
		/// To draw GUI with expandable label and inspector view.
		/// </summary>
		/*
		 * To draw GUI with built-in Editor, lots reference below.
		 * http://git.io/v8SZm
		 * 
		 * Especially these two class.
		 * http://git.io/v8SGS
		 * http://git.io/v8SGF
		 * 
		 * Maintain editors by creating for Clipboard only, and find to reuse.
		 * And register callbacks to confirm editors invalid to destroy.
		 * 
		 * The reasons of maintain solution below:
		 * 		1. We can't prevent copies from user deleting manually.
		 * 		2. Editors which lost target will cause error after script compiled.
		 * 		3. Can't reuse which created by Inspector, unexpected file operation might cause crash.
		 * 		4. Can't destroy after using, it might depend on reusing, e.g., with UnityEventDrawer.
		 */
		[InitializeOnLoad]
		[Serializable]
		private class Item{
			
			#region Static Fields

			/// <summary>
			/// The <c>forceVisible</c> property of <c>UnityEditor.MaterialEditor</c>.
			/// Set <c>true</c> to enable to draw inspector.
			/// </summary>
			private static PropertyInfo _matVisibleProp = typeof(MaterialEditor).GetProperty(
				"forceVisible", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
			);

			/// <summary>
			/// The <c>ShaderPopup()</c> method of <c>UnityEditor.MaterialEditor</c>.
			/// Invoke to draw shader popup, since its header is useless.
			/// </summary>
			private static MethodInfo _matShaderMethod = typeof(MaterialEditor).GetMethod(
				"ShaderPopup", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
			);

			/// <summary>
			/// The dictionary of stored copies paired with the <c>UnityEditor.Editor</c> to reuse.
			/// </summary>
			private static Dictionary<Object, Editor> _editors;

			/// <summary>
			/// The name of <c>UnityEditor.Editor</c> created by this.
			/// </summary>
			private const string _EDITOR_NAME = "WanzyeeStudio_Clipboard_Item_Editor";

			#endregion


			#region Static Methods

			/// <summary>
			/// Static ctor to register callbacks to check editors valid.
			/// </summary>
			static Item(){

				EditorApplication.projectWindowChanged += CheckEditors;
				EditorApplication.hierarchyWindowChanged += CheckEditors;

				EditorApplication.delayCall += CheckEditors;
				
			}
			
			/// <summary>
			/// Check if all created editors valid, otherwise clean up and rebuild.
			/// </summary>
			private static void CheckEditors(){

				if(null != _editors && _editors.All(_v => null != _v.Key)) return;

				var _e = Resources.FindObjectsOfTypeAll<Editor>().Where(_v => _EDITOR_NAME == _v.name);

				_editors = _e.Where(_v => null != _v.target).ToDictionary(_v => _v.target, _v => _v);

				foreach(var _v in _e.Except(_editors.Values).ToArray()) DestroyImmediate(_v);

			}

			/// <summary>
			/// Get the editor for specified target, create new for reusing if not existed.
			/// </summary>
			/// <returns>The editor.</returns>
			/// <param name="target">Target.</param>
			private static Editor GetEditor(Object target){

				CheckEditors();

				if(_editors.ContainsKey(target)) return _editors[target];

				var _result = _editors[target] = Editor.CreateEditor(target);

				_result.name = _EDITOR_NAME;

				if(_result is MaterialEditor && null != _matVisibleProp) _matVisibleProp.SetValue(_result, true, null);

				return _result;

			}

			#endregion


			#region Public Fields

			/// <summary>
			/// The expand mask flag, 1 for inspector, 2 for preview, 4 for note, 0 to hide and -1 to show all.
			/// </summary>
			/*
			 * Public as Serializable for Clipboard restoring expand state, also as target.
			 */
			public int expand = 3;

			/// <summary>
			/// The note text to edit.
			/// </summary>
			public string note;

			/// <summary>
			/// The copied item to show and edit.
			/// </summary>
			public Object copy;

			/// <summary>
			/// The label to show on the toolbar.
			/// </summary>
			public readonly GUIContent label;

			#endregion


			#region Private Fields

			/// <summary>
			/// The editor to draw GUI of specified copied asset.
			/// </summary>
			private readonly Editor _editor;

			/// <summary>
			/// The label style, bold font for scene object, otherwise normal.
			/// </summary>
			private readonly GUIStyle _labelStyle = new GUIStyle(EditorStyles.label);

			/// <summary>
			/// The toolbar button to remove this, or deposit if target references scene object.
			/// </summary>
			private readonly GUIContent _editBtn = new GUIContent("X", "Remove");

			/// <summary>
			/// The toolbar button to copy the item to buffer.
			/// </summary>
			private readonly GUIContent _copyBtn = new GUIContent(EditrixStyle.copyIcon, "Copy");
			
			/// <summary>
			/// The toolbar button to find the copy source.
			/// </summary>
			private readonly GUIContent _findBtn = new GUIContent(EditrixStyle.aimIcon, "Find source");

			/// <summary>
			/// The toolbar button to edit the note, i.e., the label tooltip.
			/// </summary>
			private readonly GUIContent _noteBtn = new GUIContent(EditrixStyle.editIcon, "Edit label");

			/// <summary>
			/// Determine if the mouse position is inside <c>_area</c>.
			/// </summary>
			private bool _hover;

			#endregion


			#region Methods

			/// <summary>
			/// Initialize with specified copy target, source, and note as label.
			/// </summary>
			/// <param name="copy">Target copied object.</param>
			/// <param name="note">Note.</param>
			public Item(Object copy, string note){
				
				this.copy = copy;
				_editor = GetEditor(copy);

				this.note = note;
				label = new GUIContent(note.Split('\n')[0], note);

				#if WZ_LITE

				_noteBtn.tooltip += " [Full only]";

				#else

				if(!AssetDatabase.Contains(copy)){
					_editBtn = new GUIContent("%", "Break with scene");
					_labelStyle.fontStyle = FontStyle.Bold;
				}

				#endif

			}

			/// <summary>
			/// Draw GUI includes toolbar and content, also check Drag'n'Drop to paste and hover state.
			/// </summary>
			public void Draw(){

				var _r = new Rect(GUILayoutUtility.GetRect(1f, 0f)){ height = EditorGUIUtility.isProSkin ? 1f : -1f };
				var _c = GUI.color;

				GUI.color = _c * new Color(1f, 1f, 1f, 0.5f);
				GUI.DrawTexture(_r, EditrixStyle.splitterPixel);
				GUI.color = _c;
				
				#if !WZ_LITE
				Dragger.CheckDragPaste(new Rect(_r){ height = 18f }, copy);
				#endif

				DrawToolbar();
				DrawContent();

				if(EventType.Repaint == Event.current.type){
					_r.yMax = GUILayoutUtility.GetLastRect().yMax;
					_hover = _r.Contains(Event.current.mousePosition);
				}
				
			}

			/// <summary>
			/// Draw the content includes inspector, preview, and note field if expand.
			/// </summary>
			private void DrawContent(){

				#if !WZ_LITE
				if(4 == (expand & 4)) DrawNote();
				#endif

				if(1 == (expand & 1)) DrawInspector();

				if(2 == (expand & 2)) DrawPreview();

				if(0 != expand) EditorGUILayout.Space();

			}
			
			/// <summary>
			/// Draw default inspector, additional shader menu for <c>UnityEngine.Material</c>.
			/// </summary>
			/*
			 * Use horizontal layout to do indentLevel, 'coz it doesn't work to set here.
			 * It might be for Inspector, not Editor.
			 */
			private void DrawInspector(){
				
				var _m = EditorGUIUtility.wideMode;
				var _w = EditorGUIUtility.labelWidth;
				
				EditorGUIUtility.wideMode = (330f < EditorGUIUtility.currentViewWidth);
				EditorGUIUtility.labelWidth = Mathf.Max(EditorGUIUtility.currentViewWidth * 0.45f - 40f, 120f);
				EditorGUIUtility.hierarchyMode = true;

				EditorGUILayout.Space();
				GUILayout.BeginHorizontal();
				GUILayout.Space(15f);
				GUILayout.BeginVertical();
				
				var _s = _editor is MaterialEditor && null != _matShaderMethod;
				if(_s) _matShaderMethod.Invoke(_editor, new object[]{(GUIStyle)"MiniPulldown"});
				_editor.OnInspectorGUI();
				
				GUILayout.EndVertical();
				GUILayout.EndHorizontal();
				EditorGUIUtility.labelWidth = _w;
				EditorGUIUtility.wideMode = _m;
				
			}

			/// <summary>
			/// Draw the preview of editor with settings.
			/// </summary>
			private void DrawPreview(){

				GUILayout.BeginHorizontal("preToolbar");
				GUILayout.FlexibleSpace();

				_editor.OnPreviewSettings();
				GUILayout.EndHorizontal();

				var _s = (GUIStyle)"preBackground";
				var _r = GUILayoutUtility.GetRect(10f, 120f);

				GUI.DrawTexture(_r, _s.normal.background, ScaleMode.StretchToFill);
				_editor.OnPreviewGUI(_r, _s);

			}

			#endregion


			#region Toolbar Methods

			/// <summary>
			/// Draw the toolbar with label and buttons.
			/// </summary>
			private void DrawToolbar(){

				var _c = GUI.backgroundColor;
				var _a = _c * new Color(1f, 1f, 1f, 0.3f);

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
			/// Draw the label with icon, and component enabled toggle if valid.
			/// Click name to switch expand.
			/// </summary>
			private void DrawLabel(){
				
				GUILayout.Label(AssetPreview.GetMiniThumbnail(copy), GUIStyle.none, GUILayout.Width(16f));
				DrawEnableToggle();

				var _w = GUILayout.Width(EditorGUIUtility.currentViewWidth - 158f);
				if(GUILayout.Button(label, _labelStyle, _w)){
					
					if(0 == (expand & ~4)) expand |= 3;
					else expand &= (3 == (expand & 3)) ? ~1 : ~3;

				}

				if(!_editor.HasPreviewGUI()) expand &= ~2;

			}

			/// <summary>
			/// Draw the enable toggle if valid.
			/// </summary>
			private void DrawEnableToggle(){
				
				var _r = GUILayoutUtility.GetRect(12f, 16f, EditorStyles.toggle); //occupy space

				var _e = EditorUtility.GetObjectEnabled(copy);
				if(-1 == _e) return;
				
				EditorGUI.BeginChangeCheck();
				var _t = EditorGUI.Toggle(_r, 1 == _e);
				if(!EditorGUI.EndChangeCheck()) return;

				Undo.RecordObject(copy, "Set Enabled");
				EditorUtility.SetObjectEnabled(copy, _t);

			}

			/// <summary>
			/// Draw the toolbar buttons, visible only while the mouse hovering.
			/// </summary>
			private void DrawButtons(){

				var _e = GUI.enabled;
				var _c = GUI.color;

				GUI.enabled = _hover;
				if(!_hover) GUI.color = Color.clear;

				DrawNoteButton();
				DrawCopyButton();

				DrawFindButton();
				DrawEditButton();

				GUI.enabled = _e;
				GUI.color = _c;

			}

			#endregion


			#region Button Methods

			/// <summary>
			/// Draw the note button.
			/// </summary>
			private void DrawNoteButton(){

				var _e = GUI.enabled;

				#if WZ_LITE
				GUI.enabled = false;
				#endif

				var _b = DrawButton(_noteBtn);
				GUI.enabled = _e;
				if(1 != _b) return;

				#if !WZ_LITE
				expand = (4 == (expand & 4)) ? (expand & ~4) : (expand | 4);
				GUI.FocusControl(null);
				#endif

			}

			/// <summary>
			/// Draw the copy button.
			/// Check if click to copy to buffer, and double click to duplicate a copy.
			/// </summary>
			private void DrawCopyButton(){

				var _b = DrawButton(_copyBtn);

				if(0 == _b) return;
				if(1 < _b) Manager.Duplicate(copy);

				if(copy is Material) MaterialPropertyCopier.Copy((Material)copy);
				else if(copy is Component) ComponentUtility.CopyComponent((Component)copy);

			}
			
			/// <summary>
			/// Draw the find source button.
			/// Check if click to find the source and ping it if found, and double click to select.
			/// </summary>
			private void DrawFindButton(){

				var _b = DrawButton(_findBtn);
				if(0 == _b) return;

				var _s = Filer.GetSource(copy);

				if(null == _s) EditorApplication.Beep();
				else if(1 < _b) Selection.activeObject = _s;
				else EditorGUIUtility.PingObject(_s);

			}
			
			/// <summary>
			/// Draw the remove or deposit button.
			/// </summary>
			private void DrawEditButton(){

				if(0 == DrawButton(_editBtn)) return;

				if("X" == _editBtn.text) Manager.Remove(copy, true);
				
				#if !WZ_LITE
				else Manager.Deposit(copy, true);
				#endif

			}

			/// <summary>
			/// Draw a button and return the click count.
			/// </summary>
			/// <returns>The button.</returns>
			/// <param name="content">Content.</param>
			private int DrawButton(GUIContent content){

				var _result = 0;

				var _r = GUILayoutUtility.GetRect(content, EditorStyles.toolbarButton, GUILayout.Width(22f));
				var _e = Event.current;

				if(_r.Contains(_e.mousePosition) && EventType.MouseDown == _e.type && 1 < _e.clickCount) _result = 2;
				if(0 < _result) _e.Use();

				if(GUI.Button(_r, content, EditorStyles.toolbarButton)) _result = 1;
				return _result;

			}
			
			#endregion


			#if !WZ_LITE

			/// <summary>
			/// The note field control ID, also as the flag if note changed.
			/// </summary>
			private int _control;

			/// <summary>
			/// Draw the text field to edit the note.
			/// </summary>
			private void DrawNote(){

				var _s = new GUIStyle(EditorStyles.textArea){ wordWrap = true };

				EditorGUI.BeginChangeCheck();

				note = EditorGUILayout.TextArea(note, _s);

				if(EditorGUI.EndChangeCheck()) _control = GUIUtility.keyboardControl;

				if(0 == _control || _control == GUIUtility.hotControl) return;
				
				else if(_control != GUIUtility.keyboardControl || !EditorGUIUtility.editingTextField) SaveNote(false);

				else if(0 != GUIUtility.hotControl || EventType.MouseDown == Event.current.type) SaveNote(true);

			}

			/// <summary>
			/// Save the note if changed, optional to unfocus.
			/// </summary>
			/// <param name="unfocus">If set to <c>true</c> unfocus.</param>
			public void SaveNote(bool unfocus){
				
				if(0 == _control || note == label.tooltip || null == copy) return;
				if(unfocus) GUI.FocusControl(null);

				_control = 0;
				Filer.SetNote(copy, note);

			}

			#endif

		}
		
	}
	
}
