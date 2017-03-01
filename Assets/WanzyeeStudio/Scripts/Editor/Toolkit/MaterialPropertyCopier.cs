
/*WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW*\     (   (     ) )
|/                                                      \|       )  )   _((_
||  (c) Wanzyee Studio  < wanzyeestudio.blogspot.com >  ||      ( (    |_ _ |=n
|\                                                      /|   _____))   | !  ] U
\.ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ All rights reserved./  (_(__(S)   |___*/

using UnityEditor;
using UnityEngine;
using System;
using System.Linq;

using Object = UnityEngine.Object;

namespace WanzyeeStudio.Editrix.Toolkit{

	/// <summary>
	/// Copy or paste properties from one <c>UnityEngine.Material</c> to another.
	/// </summary>
	/// 
	/// <remarks>
	/// Operate by <c>UnityEngine.Material</c> context menu "Copy Properties" and "Paste Properties".
	/// This works like the similar menu of <c>UnityEngine.Component</c>, and will change the shader.
	/// To keep valid after script reloaded by a temporary material as medium storage.
	/// Copy properties by built-in <c>Material.CopyPropertiesFromMaterial()</c> method.
	/// </remarks>
	/// 
	public static class MaterialPropertyCopier{

		#region Menu

		/// <summary>
		/// Copy the material properties to created new temp.
		/// </summary>
		/// <param name="command">Command.</param>
		[MenuItem("CONTEXT/Material/Copy Properties", false, 710)]
		private static void CopyProperties(MenuCommand command){
			Copy((Material)command.context);
		}

		/// <summary>
		/// Paste the material properties to current context from temp.
		/// </summary>
		/// <param name="command">Command.</param>
		[MenuItem("CONTEXT/Material/Paste Properties", false, 710)]
		private static void PasteProperties(MenuCommand command){
			Paste((Material)command.context);
		}

		/// <summary>
		/// Check if <c>PasteProperties()</c> valid, temporary material exists.
		/// </summary>
		/// <returns><c>true</c>, if valid.</returns>
		/// <param name="command">Command.</param>
		[MenuItem("CONTEXT/Material/Paste Properties", true)]
		private static bool PastePropertiesValid(MenuCommand command){
			return Resources.FindObjectsOfTypeAll<Material>().Any(_v => _name == _v.name);
		}

		#endregion


		#region Static

		/// <summary>
		/// The name of temporary material.
		/// </summary>
		private static readonly string _name = typeof(MaterialPropertyCopier).FullName;

		/// <summary>
		/// Copy properties of specified material as source to paste later.
		/// </summary>
		/// <param name="source">Source.</param>
		public static void Copy(Material source){

			if(null == source) throw new ArgumentNullException("source");

			var _d = Resources.FindObjectsOfTypeAll<Material>().Where(_v => _name == _v.name);

			foreach(var _v in _d.ToArray()) Object.DestroyImmediate(_v);

			Object.Instantiate(source).name = _name;

		}

		/// <summary>
		/// Paste properties of the source copied before to the specified target.
		/// </summary>
		/// <returns><c>false</c>, if the source doesn't exist yet, otherwise <c>true</c>.</returns>
		/// <param name="target">Target.</param>
		public static bool Paste(Material target){

			if(null == target) throw new ArgumentNullException("target");

			var _m = Resources.FindObjectsOfTypeAll<Material>().FirstOrDefault(_v => _name == _v.name);
			if(null == _m) return false;

			Undo.RecordObject(target, "Paste Material");

			target.shader = _m.shader;
			target.CopyPropertiesFromMaterial(_m);

			return true;

		}

		#endregion

	}

}
