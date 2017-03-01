
/*WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW*\     (   (     ) )
|/                                                      \|       )  )   _((_
||  (c) Wanzyee Studio  < wanzyeestudio.blogspot.com >  ||      ( (    |_ _ |=n
|\                                                      /|   _____))   | !  ] U
\.ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ All rights reserved./  (_(__(S)   |___*/

using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

namespace WanzyeeStudio.Editrix{

	/// <summary>
	/// Draw an auxiliary icon follows the cursor globally.
	/// </summary>
	/// 
	/// <remarks>
	/// Used to provide a cross windows hint for reasons below:
	/// 	1. <c>DragAndDrop.visualMode</c> only works when using <c>UnityEditor.DragAndDrop</c> system.
	/// 	2. <c>EditorGUIUtility.AddCursorRect()</c> only applies inside the current window.
	/// </remarks>
	/// 
	public class AuxCursor : EditorWindow{

		#region Properties

		/// <summary>
		/// The visual mode of cursor to show, to set this will override the <c>icon</c>.
		/// </summary>
		/// <value>The mode.</value>
		public static DragAndDropVisualMode mode{
			get{ return (null == _instance) ? DragAndDropVisualMode.None : _instance._mode; }
			set{ SetCursor(value, null); }
		}

		/// <summary>
		/// The texture of cursor to show, to set this will override the <c>mode</c>.
		/// </summary>
		/// <value>The icon.</value>
		public static Texture icon{
			get{ return (null == _instance) ? null : _instance._icon; }
			set{ SetCursor(DragAndDropVisualMode.None, value); }
		}

		/// <summary>
		/// The pixel offset between the auxiliary icon and the system cursor.
		/// </summary>
		/// <value>The offset.</value>
		public static Vector2 offset{
			get{ return (null == _instance) ? Vector2.zero : _instance._offset; }
			set{ if(null != _instance) _instance._offset = value; }
		}

		/// <summary>
		/// The pixel size of the auxiliary icon, set <c>Vector2.zero</c> to use the default texture size.
		/// </summary>
		/// <value>The size.</value>
		public static Vector2 size{
			get{ return (null == _instance) ? Vector2.zero : _instance._size; }
			set{ if(null != _instance) _instance._size = value; }
		}

		#endregion


		#region Static

		/// <summary>
		/// The singleton of <c>AuxCursor</c>.
		/// </summary>
		private static AuxCursor _instance;

		/// <summary>
		/// The visual modes paired with corresponding icon texture.
		/// </summary>
		private static readonly Dictionary<DragAndDropVisualMode, Texture> _cursors = (
			
			new Dictionary<DragAndDropVisualMode, Texture>(){
				
				{DragAndDropVisualMode.None, Texture2D.blackTexture},
				{DragAndDropVisualMode.Copy, EditrixStyle.copyCursor},

				{DragAndDropVisualMode.Link, EditrixStyle.linkCursor},
				{DragAndDropVisualMode.Move, EditrixStyle.moveCursor},

				{DragAndDropVisualMode.Generic, EditrixStyle.copyCursor},
				{DragAndDropVisualMode.Rejected, EditrixStyle.stopCursor}

			}

		);

		/// <summary>
		/// Set the cursor with specified visual mode and icon texture.
		/// Also create the singleton if need to show and not existing.
		/// </summary>
		/// <param name="mode">Mode.</param>
		/// <param name="icon">Icon.</param>
		private static void SetCursor(DragAndDropVisualMode mode, Texture icon){

			if(null == _instance){
				if(DragAndDropVisualMode.None == mode && null == icon) return;
				else CreateInstance<AuxCursor>().ShowPopup();
			}

			_instance._mode = _cursors.ContainsKey(mode) ? mode : DragAndDropVisualMode.None;
			_instance._icon = icon;

		}

		#endregion


		#region Fields

		/// <summary>
		/// The visual mode of cursor to show.
		/// </summary>
		/*
		 * Designed as serialized fields in case if scripts reloaded while displaying.
		 */
		[Obfuscation(Exclude = true)]
		[SerializeField]
		[Tooltip("Visual mode of cursor to show.")]
		private DragAndDropVisualMode _mode = DragAndDropVisualMode.None;

		/// <summary>
		/// The texture of cursor to show.
		/// </summary>
		[Obfuscation(Exclude = true)]
		[SerializeField]
		[Tooltip("Texture of cursor to show.")]
		private Texture _icon;

		/// <summary>
		/// The pixel offset from the system cursor.
		/// </summary>
		[Obfuscation(Exclude = true)]
		[SerializeField]
		[Tooltip("Pixel offset from the system cursor.")]
		private Vector2 _offset = new Vector2(2f, 16f);

		/// <summary>
		/// The pixel size to draw if set, otherwise use texture size.
		/// </summary>
		[Obfuscation(Exclude = true)]
		[SerializeField]
		[Tooltip("Pixel size to draw if set, otherwise use texture size.")]
		private Vector2 _size;

		#endregion


		#region Methods

		/// <summary>
		/// OnEnable, set the size limit and check the singleton.
		/// </summary>
		private void OnEnable(){

			minSize = new Vector2(0f, 0f);
			
			if(null == _instance) _instance = this;
			else if(this != _instance) Close();

		}

		/// <summary>
		/// Update, check if to close, otherwise constantly repaint.
		/// </summary>
		private void Update(){
			if(this != _instance || (DragAndDropVisualMode.None == _mode && null == _icon)) Close();
			else Repaint();
		}

		/// <summary>
		/// OnGUI, follow the cursor, and draw the <c>icon</c> is assigned, otherwise draw icon of the <c>mode</c>.
		/// </summary>
		private void OnGUI(){

			if(EventType.Repaint != Event.current.type) return;

			var _t = icon ?? _cursors[_mode];
			var _s = new Vector2((0f < _size.x) ? _size.x : _t.width, (0f < _size.y) ? _size.y : _t.height);

			position = new Rect(position.position + Event.current.mousePosition + _offset, _s);
			GUI.DrawTexture(new Rect(Vector2.zero, _s), _t);

		}

		#endregion

	}

}
