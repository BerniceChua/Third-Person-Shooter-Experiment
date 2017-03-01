
/*WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW*\     (   (     ) )
|/                                                      \|       )  )   _((_
||  (c) Wanzyee Studio  < wanzyeestudio.blogspot.com >  ||      ( (    |_ _ |=n
|\                                                      /|   _____))   | !  ] U
\.ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ All rights reserved./  (_(__(S)   |___*/

#if !WZ_LITE

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
		/// Handle all Drag'n'Drop operations for <c>Clipboard</c>.
		/// </summary>
		private static class Dragger{

			#region Fields

			/// <summary>
			/// Type of Inspector window.
			/// </summary>
			private static readonly Type _inspectorType = Type.GetType("UnityEditor.InspectorWindow, UnityEditor");

			/// <summary>
			/// Method info to get the <c>ActiveEditorTracker</c> of an Inspector window.
			/// Used to find the inspected targets to paste.
			/// Reflect to <c>UnityEditor.InspectorWindow.GetTracker()</c>.
			/// It's a public instance method, without params, return <c>UnityEditor.ActiveEditorTracker</c>.
			/// </summary>
			private static readonly MethodInfo _trackerMethod = new Func<MethodInfo>(() => {

				if(null == _inspectorType) return null;

				var _b = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
				var _t = typeof(ActiveEditorTracker);

				var _m = _inspectorType.GetMethod("GetTracker", _b, null, new Type[0], null);
				if(null != _m && _t == _m.ReturnType) return _m;

				_m = _inspectorType.GetProperty("tracker", _b).GetGetMethod(true);
				return (null != _m && _t == _m.ReturnType) ? _m : null;

			})();

			/// <summary>
			/// The targets of existing Inspector windows to paste.
			/// </summary>
			private static Dictionary<Object, IEnumerable<Object>> _pasteTargets;

			/// <summary>
			/// The paste source from a copied item.
			/// Also as the flag to check if drop to paste.
			/// </summary>
			private static Object _pasteSource;

			/// <summary>
			/// The position where the mouse pressed, used to ensure to start drag from the same area.
			/// </summary>
			private static Vector2 _dragPos;

			#endregion


			#region Copy Methods

			/// <summary>
			/// Check if Drag'n'Drop performed in a <c>Clipboard</c> window to copy dragged objects.
			/// </summary>
			/// 
			/// <remarks>
			/// Copy all attached <c>UnityEngine.Component</c> if drop any <c>UnityEngine.GameObject</c>.
			/// Show the type menu to filter if "Ctrl" key pressed when dropping.
			/// </remarks>
			/// 
			public static void CheckDragCopy(){

				var _t = Event.current.type;
				if(EventType.DragUpdated != _t && EventType.DragPerform != _t) return;

				var _d = DragAndDrop.objectReferences;
				var _c = _d.OfType<GameObject>().SelectMany(_v => _v.GetComponents<Component>()).Cast<Object>();
				var _o = _d.Union(_c).Where(IsCopyable);

				DragAndDrop.visualMode = _o.Any() ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.Rejected;
				if(!_o.Any() || EventType.DragPerform != _t) return;

				DragAndDrop.AcceptDrag();
				Event.current.Use();

				if(Event.current.control) ShowTypeCopyMenu(_o);
				else foreach(var _v in _o) Copy(_v);

			}

			/// <summary>
			/// Show the types menu to copy the filtered sources.
			/// </summary>
			/// <param name="sources">Sources.</param>
			private static void ShowTypeCopyMenu(IEnumerable<Object> sources){
				
				GenericMenu.MenuFunction2 _f = (o) => { foreach(var _v in (IEnumerable<Object>)o) Copy(_v); };

				var _g = sources.GroupBy(_v => _v.GetType()).OrderBy(_v => _v.Key.FullName);
				var _m = new GenericMenu();

				_m.AddItem(new GUIContent("Copy All"), false, _f, sources);

				if(1 < _g.Count() && _g.Any(_v => typeof(Transform).IsAssignableFrom(_v.Key))){
					_m.AddItem(new GUIContent("Exclude Transform"), false, _f, sources.Where(_v => !(_v is Transform)));
				}

				_m.AddSeparator("");
				foreach(var _v in _g) _m.AddItem(new GUIContent(_v.Key.Name), false, _f, _v);

				_m.ShowAsContext();

			}

			#endregion


			#region Drag Methods

			/// <summary>
			/// Check if to start drag the paste source from specified area.
			/// </summary>
			/// 
			/// <remarks>
			/// Only starts if all the situations below are valid:
			/// 	1. The positions to press and drag are both in the area when dragging.
			/// 	2. The reflection works and valid targets found.
			/// 	3. It's not dragging currently.
			/// </remarks>
			/// 
			/// <param name="area">Area.</param>
			/// <param name="source">Source.</param>
			/// 
			/*
			 * Check clickCount to ensure receiving MouseUp when CheckDropPaste() to close the AuxCursor.
			 */
			public static void CheckDragPaste(Rect area, Object source){

				var _e = Event.current;

				if(EventType.MouseDown == _e.type) _dragPos = (2 > _e.clickCount) ? _e.mousePosition : Vector2.zero;
				if(EventType.MouseDrag != _e.type) return;

				if(!area.Contains(_e.mousePosition) || !area.Contains(_dragPos)) return;
				if(null == _trackerMethod || null != _pasteSource) return;

				_pasteTargets = GetPasteTargets(source);
				if(!_pasteTargets.Any()) return;

				_pasteSource = source;
				_e.Use();

			}

			/// <summary>
			/// Get the paste targets from the existing Inspector windows by the specified source type.
			/// </summary>
			/// <returns>The paste targets.</returns>
			/// <param name="source">Source.</param>
			private static Dictionary<Object, IEnumerable<Object>> GetPasteTargets(Object source){
				
				var _result = new Dictionary<Object, IEnumerable<Object>>();

				foreach(var _o in Resources.FindObjectsOfTypeAll(_inspectorType)){

					var _e = ((ActiveEditorTracker)_trackerMethod.Invoke(_o, null)).activeEditors;
					var _t = _e.SelectMany(_v => _v.targets);

					if(source is Component) _t = _t.OfType<GameObject>().Cast<Object>();
					else _t = _t.Where(_v => source.GetType().IsInstanceOfType(_v));

					if(_t.Any()) _result.Add(_o, _t);

				}

				return _result;

			}

			#endregion


			#region Paste Methods

			/// <summary>
			/// Check if dropped in an Inspector window with valid paste targets while dragging.
			/// </summary>
			/// 
			/// <remarks>
			/// Change the cursor as a hint while the mouse over the window.
			/// Paste the source to the targets when dropped in the window.
			/// </remarks>
			/// 
			/*
			 * Check the Event.current.rawType to detect dropping anywhere.
			 * Store the mouseOverWindow to repaint, when the mouse may over nothing when click a generic menu.
			 */
			public static void CheckDropPaste(){
				
				if(null == _pasteSource) return;

				var _w = mouseOverWindow;
				var _d = (EventType.MouseUp == Event.current.rawType);

				var _i = (null != _w && _pasteTargets.ContainsKey(_w));
				AuxCursor.mode = _i ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.Rejected;

				if(_d && _i){
					PasteTargets(_pasteTargets[_w]);
					_w.Repaint();
				}

				if(_d || KeyCode.Escape == Event.current.keyCode){
					_pasteSource = null;
					AuxCursor.mode = DragAndDropVisualMode.None;
				}

			}

			/// <summary>
			/// Determine the paste source type to paste it to the specified targets.
			/// Show a sub menu to operate if "Ctrl" key pressed when dropping.
			/// </summary>
			/// <param name="targets">Targets.</param>
			private static void PasteTargets(IEnumerable<Object> targets){

				if(_pasteSource is Component){

					var _g = targets.Cast<GameObject>();
					var _t = _pasteSource.GetType();

					Func<Component, bool> _p = (c) => {
						if(c.GetType() != _t) return false;
						else if(typeof(Component) != _t && typeof(Behaviour) != _t) return true;
						return ObjectNames.GetInspectorTitle(c) == ObjectNames.GetInspectorTitle(_pasteSource);
					};

					var _d = _g.ToDictionary(_v => _v, _v => _v.GetComponents<Component>().Where(_p));
					if(!Event.current.control) PasteComponents(0, _d);
					else ShowComponentPasteMenu(_d);

				}else{

					MaterialPropertyCopier.Copy((Material)_pasteSource);
					foreach(var _v in targets) MaterialPropertyCopier.Paste((Material)_v);

				}

			}

			/// <summary>
			/// Paste the source <c>UnityEngine.Component</c> to specified <c>UnityEngine.GameObject</c> targets.
			/// Not allow to paste <c>UnityEngine.Transform</c> if the types are different.
			/// The default paste operations below:
			/// 	1. Paste as new if there's no component of the same type.
			/// 	2. Paste values to the indexed existing component if the index valid.
			/// 	3. Paste values to all the existing components if the index negative.
			/// 	4. Paste as new if the index is out of range.
			/// </summary>
			/// <param name="index">Index.</param>
			/// <param name="targets">Targets.</param>
			/*
			 * It crashes when pasting a RectTransform to a Transform sometimes.
			 * Simply disallow it to avoid, since it's an irregular operation originally.
			 * Unfortunately, I haven't reproduced the crash in a new empty project.
			 */
			private static void PasteComponents(int index, Dictionary<GameObject, IEnumerable<Component>> targets){

				ComponentUtility.CopyComponent((Component)_pasteSource);

				foreach(var _v in targets){

					if(_pasteSource is Transform && _pasteSource.GetType() != _v.Key.transform.GetType()){
						var _f = "It's not allowed to paste a {0} to {1}.";
						Debug.LogWarningFormat(_v.Key, _f, _pasteSource.GetType(), _v.Key.transform);
						continue;
					}

					if(!_v.Value.Any() || _v.Value.Count() <= index) ComponentUtility.PasteComponentAsNew(_v.Key);

					else if(0 <= index) ComponentUtility.PasteComponentValues(_v.Value.ElementAt(index));

					else foreach(var _o in _v.Value) ComponentUtility.PasteComponentValues(_o);

				}

			}

			/// <summary>
			/// Show the menu to paste a <c>UnityEngine.Component</c> as new or paste the values to a specified one.
			/// </summary>
			/// <param name="targets">Targets.</param>
			/*
			 * End with sending an event to trigger the menu shows up right away.
			 */
			private static void ShowComponentPasteMenu(Dictionary<GameObject, IEnumerable<Component>> targets){
				
				GenericMenu.MenuFunction2 _f = (o) => PasteComponents((int)o, targets);

				var _l = targets.Max(_v => _v.Value.Count());
				var _m = new GenericMenu();

				_m.AddItem(new GUIContent("Paste As New"), false, _f, int.MaxValue);
				if(1 < _l) _m.AddItem(new GUIContent("Paste Values to All"), false, _f, -1);
				if(0 < _l) _m.AddSeparator("");

				for(int i = 0; i < _l; i++){
					var _n = (3 > i) ? new []{"1st", "2nd", "3rd"}[i] : ((i + 1) + "th");
					_m.AddItem(new GUIContent("Paste Values to " + _n), false, _f, i);
				}

				_m.ShowAsContext();
				mouseOverWindow.SendEvent(Event.KeyboardEvent("~"));

			}

			#endregion

		}

	}

}

#endif
