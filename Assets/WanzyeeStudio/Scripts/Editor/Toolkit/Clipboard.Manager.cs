
/*WWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWWW*\     (   (     ) )
|/                                                      \|       )  )   _((_
||  (c) Wanzyee Studio  < wanzyeestudio.blogspot.com >  ||      ( (    |_ _ |=n
|\                                                      /|   _____))   | !  ] U
\.ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ All rights reserved./  (_(__(S)   |___*/

using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Object = UnityEngine.Object;

namespace WanzyeeStudio.Editrix.Toolkit{
	
	public partial class Clipboard{
		
		/// <summary>
		/// Manager to create and maintain all the copies.
		/// </summary>
		private static class Manager{

			#region Fields

			/// <summary>
			/// Occur to refresh when copied items changed.
			/// </summary>
			public static Action refresh = () => _groups = null;

			/// <summary>
			/// The stored groups for each <c>Clipboard</c> window to copy.
			/// Set <c>null</c> as the flag to update when the next loading.
			/// </summary>
			private static Group[] _groups;

			/// <summary>
			/// The scene root container to store copies, or as scene copy's name prefix.
			/// </summary>
			private const string _SCENE_ROOT = "WanzyeeStudio/Clipboard";

			/// <summary>
			/// The required <c>UnityEngine.Component</c> type pairs.
			/// </summary>
			/*
			 * For some reasons, Unity didn't declare RequireComponent attribute on types below, but they do need.
			 * NetworkTransformChild checks requirement in OnValidate(), and also check its hierarchy.
			 * Audio filters require AudioSource or AudioListener, but don't need both, so it can't declare.
			 */
			private static Dictionary<Type, Type> _requires = new Dictionary<Type, Type>(){
				{typeof(NetworkTransformChild), typeof(NetworkTransform)},
				{typeof(AudioChorusFilter), typeof(AudioSource)},
				{typeof(AudioDistortionFilter), typeof(AudioSource)},
				{typeof(AudioEchoFilter), typeof(AudioSource)},
				{typeof(AudioHighPassFilter), typeof(AudioSource)},
				{typeof(AudioLowPassFilter), typeof(AudioSource)},
				{typeof(AudioReverbFilter), typeof(AudioSource)}
			};

			#endregion

			
			#if !WZ_LITE

			/// <summary>
			/// The asset root directory reference for lazy initializing, use <c>_assetRoot</c> instead.
			/// </summary>
			private static string _assetRootRef;

			/// <summary>
			/// The asset root directory to store copies, find default if existing, otherwise use project root.
			/// </summary>
			/// <value>The path of the asset root directory.</value>
			private static string _assetRoot{
				
				get{

					if(!string.IsNullOrEmpty(_assetRootRef) && Directory.Exists(_assetRootRef)) return _assetRootRef;

					var _p = AssetDatabase.FindAssets("WanzyeeStudio").Select(_v => AssetDatabase.GUIDToAssetPath(_v));
					var _d = _p.FirstOrDefault(_v => Path.GetFileName(_v) == "WanzyeeStudio" && Directory.Exists(_v));

					_assetRootRef = (_d ?? "Assets") + "/Temp/Clipboard";
					return _assetRootRef;
					
				}
				
			}

			#endif


			#region Common Methods

			/// <summary>
			/// Check if the specified source object is able to copy.
			/// </summary>
			/// 
			/// <remarks>
			/// I.e., the type's supported and the source is from out of clipboard.
			/// Otherwise throw exception.
			/// </remarks>
			/// 
			/// <param name="obj">Source object.</param>
			/// 
			public static void CheckCreatable(Object obj){

				if(null == obj) throw new ArgumentNullException("obj");

				if(!(obj is Component) && !(obj is Material))
					throw new NotSupportedException("Clipboard only supports Component and Material.");

				var _p = GetObjectPath(obj);
				var _s = _p.StartsWith(_SCENE_ROOT);

				#if WZ_LITE
				Limiter.CheckOperable();
				#else
				if(!_s) _s = _p.StartsWith(_assetRoot);
				#endif

				if(_s) throw new InvalidOperationException("Clipboard won't copy upskirt self, please don't do it.");

			}

			/// <summary>
			/// Find or create the host <c>UnityEngine.GameObject</c> at specified hierarchy path.
			/// Like <c>GameObject.Find()</c>, but optional to create if none.
			/// Any created host is editable but hide in hierarchy and inactive.
			/// </summary>
			/// <returns>The scene host.</returns>
			/// <param name="path">Path.</param>
			/// <param name="create">If set to <c>true</c> create.</param>
			private static GameObject GetSceneHost(string path, bool create){

				var _r = Resources.FindObjectsOfTypeAll<Transform>(
					).Where(_v => (null == _v.parent) && !EditorUtility.IsPersistent(_v)
				);

				Transform _h = null;
				foreach(var _v in path.Split('/')){

					var _t = (null == _h) ? _r.FirstOrDefault(_o => _v == _o.name) : _h.Find(_v);
					if(null == _t && !create) return null;
					
					if(null == _t){
						_t = new GameObject(_v){ hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy }.transform;
						_t.gameObject.SetActive(false);
						_t.SetParent(_h);
					}

					_h = _t;

				}

				return _h.gameObject;

			}
			
			/// <summary>
			/// Get the object path, i.e., asset path of asset object, or hierarchy path of scene object.
			/// </summary>
			/// <returns>The object path.</returns>
			/// <param name="obj">Object.</param>
			private static string GetObjectPath(Object obj){
				
				if(AssetDatabase.Contains(obj)) return AssetDatabase.GetAssetPath(obj);
				
				if(obj is Component) return Filer.GetHierarchyPath(((Component)obj).transform);
				
				return obj.name;
				
			}

			/// <summary>
			/// Get the type name combined with assembly name for identifying.
			/// </summary>
			/// <returns>The type name.</returns>
			/// <param name="type">Type.</param>
			private static string GetTypeName(Type type){
				return type.FullName + "@" + type.Assembly.GetName().Name;
			}

			#endregion
			
			
			#region Create Methods
			
			/// <summary>
			/// Create a copy from specified source object.
			/// </summary>
			/// <param name="source">Source object.</param>
			public static void Create(Object source){
				
				CheckCreatable(source);
				Object _c = null;
				
				#if !WZ_LITE
				if(!CheckReferScene(source)){
					if(source is Material) _c = CreateAssetMaterial((Material)source);
					else _c = CreateAssetComponent((Component)source);
				}
				#endif

				if(null == _c){
					if(source is Material) _c = CreateSceneMaterial((Material)source);
					else _c = CreateSceneComponent((Component)source);
				}

				Filer.SetSource(_c, source);

			}

			/// <summary>
			/// Duplicate the specified copy.
			/// </summary>
			/// <param name="copy">Copied object.</param>
			public static void Duplicate(Object copy){

				var _p = GetObjectPath(copy);
				Object _c = null;

				#if !WZ_LITE
				if(_p.StartsWith(_assetRoot)){
					if(copy is Material) _c = CreateAssetMaterial((Material)copy);
					else _c = CreateAssetComponent((Component)copy);
				}
				#endif
				
				if(_p.StartsWith(_SCENE_ROOT)){
					if(copy is Material) _c = CreateSceneMaterial((Material)copy);
					else _c = CreateSceneComponent((Component)copy);
				}

				if(null == _c) throw new InvalidOperationException("Clipboard can only duplicate copied item.");
				Filer.SetData(_c, copy);

			}

			/// <summary>
			/// Create a copied <c>UnityEngine.Material</c> stored in scene.
			/// </summary>
			/// <returns>The scene material.</returns>
			/// <param name="source">Source material.</param>
			private static Material CreateSceneMaterial(Material source){
				
				var _result = Object.Instantiate(source);

				_result.hideFlags = HideFlags.DontSave;
				_result.name = _SCENE_ROOT + "/" + GetTypeName(source.GetType()) + "/" + _result.GetInstanceID();

				return _result;

			}

			/// <summary>
			/// Create a copied <c>UnityEngine.Component</c> stored in scene.
			/// Create the hierarchy host with unique path at the meantime.
			/// </summary>
			/// <returns>The scene component.</returns>
			/// <param name="source">Source component.</param>
			/*
			 * The redundancy instantiating is only used to make operations not undoable.
			 */
			private static Component CreateSceneComponent(Component source){

				var _p = GetSceneHost(_SCENE_ROOT + "/" + GetTypeName(source.GetType()), true).transform;
				var _n = GameObjectUtility.GetUniqueNameForSibling(_p, source.GetInstanceID().ToString());

				var _s = CreateComponent(source); //create temp
				var _result = Instantiate(_s);
				DestroyImmediate(_s.gameObject); //kill temp

				_result.transform.SetParent(_p);
				_result.gameObject.hideFlags = _p.gameObject.hideFlags;
				_result.gameObject.name = _n;

				return _result;

			}

			/// <summary>
			/// Create a component copied from the source, and attached on a hidden empty <c>UnityEngine.GameObject</c>.
			/// </summary>
			/// <returns>The component.</returns>
			/// <param name="source">Source component.</param>
			/*
			 * http://answers.unity3d.com/questions/530178/
			 * Not to instantiate directly to avoid potential Awake() and redundancy components.
			 * Check type for some components are not specific type, e.g., Halo.
			 */
			private static Component CreateComponent(Component source){

				var _g = new GameObject(){ hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy };
				_g.SetActive(false);

				var _t = source.GetType();
				if(_requires.ContainsKey(_t)) _g.AddComponent(_requires[_t]);

				ComponentUtility.CopyComponent(source);
				if(typeof(Component) == _t || typeof(Behaviour) == _t) ComponentUtility.PasteComponentAsNew(_g);
				else if(typeof(Transform) == _t) ComponentUtility.PasteComponentValues(_g.transform);
				else ComponentUtility.PasteComponentValues(_g.AddComponent(_t));

				var _result = _g.GetComponents<Component>().Last(_v => _v.GetType() == _t);
				RedirectReferences(_result, source);
				return _result;

			}

			/// <summary>
			/// Redirect the references to specified object instead of itself, a copied instance.
			/// </summary>
			/// <param name="obj">Object.</param>
			/// <param name="replace">Replace.</param>
			private static void RedirectReferences(Object obj, Object replace){
				
				var _p = new SerializedObject(obj).GetIterator();
				while(_p.NextVisible(true)){

					if(SerializedPropertyType.ObjectReference != _p.propertyType) continue;
					if(obj != _p.objectReferenceValue) continue;

					_p.objectReferenceValue = replace;
					_p.serializedObject.ApplyModifiedProperties();

				}

			}

			#endregion


			#region Load Methods

			/// <summary>
			/// Get a deep copy from the stored groups contains all loaded copied items sorted by labels.
			/// </summary>
			/// 
			/// <remarks>
			/// This invokes the <c>refresh</c> event if the stored groups are updated or force reloaded.
			/// </remarks>
			/// 
			/// <returns>The groups.</returns>
			/// <param name="reload">If set to <c>true</c> reload.</param>
			/// 
			public static Group[] LoadGroups(bool reload = false){

				if(null == _groups || reload || _groups.SelectMany(_v => _v.items).Any(_v => null == _v.copy)){

					#if WZ_LITE
					var _c = Limiter.PrepareCopies(LoadSceneCopies());
					#else
					var _c = LoadAssetCopies().Union(LoadSceneCopies());
					#endif

					var _i = _c.Select(_v => new Item(_v, Filer.GetNote(_v, true))).OrderBy(_v => _v.label.text);
					var _g = _i.GroupBy(_v => _v.copy.GetType()).OrderBy(_v => GetTypeOrder(_v.Key));

					refresh.Invoke();
					_groups = _g.Select(_v => new Group(_v.Key, _v.ToArray())).ToArray();

				}

				return _groups.Select(
					_v => new Group(_v.type, _v.items.Select(_o => new Item(_o.copy, _o.note)).ToArray())
				).ToArray();

			}
			
			/// <summary>
			/// Load all the copies in scene root container.
			/// </summary>
			/// <returns>The scene copies.</returns>
			private static Object[] LoadSceneCopies(){

				var _m = Resources.FindObjectsOfTypeAll(typeof(Material)).Where(_v => _v.name.StartsWith(_SCENE_ROOT));

				var _h = GetSceneHost(_SCENE_ROOT, false);
				if(null == _h) return _m.ToArray();

				var _t = _h.transform.Cast<Transform>().SelectMany(_v => _v.Cast<Transform>());
				var _a = _t.Select(_v => _v.GetComponents<Component>());

				var _c = _a.Select(_v => _v.LastOrDefault(_o => GetTypeName(_o.GetType()) == _o.transform.parent.name));
				return _c.Where(_v => null != _v).Cast<Object>().Union(_m).ToArray();

			}

			/// <summary>
			/// Get an order <c>string</c> of given type for sorting.
			/// </summary>
			/// 
			/// <remarks>
			/// The types is order by categories below, then order by name:
			/// 	1. Transform, then sub classes of Transform.
			/// 	2. UnityEngine types.
			/// 	3. UnityEditor types.
			/// 	4. Other types.
			/// 	5. Material, then sub classes of Material.
			/// </remarks>
			/// 
			/// <returns>The order.</returns>
			/// <param name="type">Type.</param>
			/// 
			private static string GetTypeOrder(Type type){

				if(typeof(Transform) == type) return "1";
				if(typeof(Transform).IsAssignableFrom(type)) return "1." + type.Name;

				if(typeof(Material) == type) return "5";
				if(typeof(Material).IsAssignableFrom(type)) return "5." + type.Name;

				if(type.FullName.StartsWith("UnityEngine.")) return "2." + type.Name;
				if(type.FullName.StartsWith("UnityEditor.")) return "3." + type.Name;
				
				return "4." + type.Name;
				
			}

			#endregion


			#region Remove Methods

			/// <summary>
			/// Remove all copies of specified type, or all types if assign <c>null</c>.
			/// </summary>
			/// 
			/// <remarks>
			/// Optional to delay invoke, e.g., while using the copies to draw GUI.
			/// </remarks>
			/// 
			/// <param name="type">Copy type.</param>
			/// <param name="delay">If set to <c>true</c> delay.</param>
			/// 
			public static void RemoveAll(Type type, bool delay){
				
				Delete((null == type) ? _SCENE_ROOT : (_SCENE_ROOT + "/" + GetTypeName(type)), delay);

				#if !WZ_LITE
				Delete((null == type) ? _assetRoot : (_assetRoot + "/" + GetTypeName(type)), delay);
				#endif

			}

			/// <summary>
			/// Remove the specified copied object.
			/// </summary>
			/// 
			/// <remarks>
			/// Optional to delay invoke, e.g., while using the copy to draw GUI.
			/// </remarks>
			/// 
			/// <param name="copy">Copied object.</param>
			/// <param name="delay">If set to <c>true</c> delay.</param>
			/// 
			public static void Remove(Object copy, bool delay){
				Delete(GetObjectPath(copy), delay);
			}

			/// <summary>
			/// Delete the asset, directory, or scene host at specified path.
			/// Optional to delay invoke, e.g., while using the object in caller method.
			/// </summary>
			/// <param name="path">Asset or hierarchy path.</param>
			/// <param name="delay">If set to <c>true</c> delay.</param>
			private static void Delete(string path, bool delay){

				EditorApplication.CallbackFunction _f = () => {

					if(path.StartsWith(_SCENE_ROOT)) DeleteSceneCopies(path);
					#if !WZ_LITE
					else if(path.StartsWith(_assetRoot)) AssetDatabase.DeleteAsset(path);
					#endif
					else throw new InvalidOperationException("Clipboard can't remove external.");

					refresh.Invoke();

				};
				
				if(!delay) _f.Invoke();
				else EditorApplication.delayCall += _f;
				
			}

			/// <summary>
			/// Delete the scene copies with the specified path or name.
			/// </summary>
			/// <param name="path">Path.</param>
			private static void DeleteSceneCopies(string path){

				var _m = Resources.FindObjectsOfTypeAll<Material>().Where(_v => _v.name.StartsWith(path)).ToArray();

				foreach(var _v in _m) DestroyImmediate(_v);

				DestroyImmediate(GetSceneHost(path, false));

			}

			#endregion
			
			
			#if !WZ_LITE

			/// <summary>
			/// Deposit a scene copy to asset for next edit time, but any scene reference will be lost.
			/// </summary>
			/// 
			/// <remarks>
			/// Optional to delay remove origin copy while still using.
			/// </remarks>
			/// 
			/// <param name="copy">Copied object in scene.</param>
			/// <param name="delay">If set to <c>true</c> delay.</param>
			/// 
			public static void Deposit(Object copy, bool delay){
				
				if(!GetObjectPath(copy).StartsWith(_SCENE_ROOT))
					throw new InvalidOperationException("Clipboard can only move scene copy.");
				
				Object _c;
				if(copy is Material) _c = CreateAssetMaterial((Material)copy);
				else _c = CreateAssetComponent((Component)copy);

				Filer.SetData(_c, copy);
				Remove(copy, delay);
				
			}
			
			/// <summary>
			/// Create a copied <c>UnityEngine.Material</c> stored in asset.
			/// </summary>
			/// <returns>The asset material.</returns>
			/// <param name="source">Source material.</param>
			private static Material CreateAssetMaterial(Material source){
				
				var _result = Object.Instantiate(source);
				
				AssetDatabase.CreateAsset(_result, GenerateAssetPath(source, "mat"));
				
				return _result;
				
			}
			
			/// <summary>
			/// Create a copied <c>UnityEngine.Component</c> prefab stored in asset.
			/// </summary>
			/// <returns>The asset component.</returns>
			/// <param name="source">Source component.</param>
			/*
			 * Disable the VerifySavingAssets flag while creating to avoid the Save Assets dialog.
			 */
			private static Component CreateAssetComponent(Component source){
				
				var _s = EditorPrefs.GetBool("VerifySavingAssets", false);
				EditorPrefs.SetBool("VerifySavingAssets", false);

				var _g = CreateComponent(source).gameObject;
				_g.hideFlags &= ~HideFlags.DontSaveInEditor;

				var _p = PrefabUtility.CreatePrefab(GenerateAssetPath(source, "prefab"), _g);
				DestroyImmediate(_g);

				EditorPrefs.SetBool("VerifySavingAssets", _s);
				return _p.GetComponent(source.GetType());
				
			}
			
			/// <summary>
			/// Generate unique asset path in directory of source type with extension.
			/// Create the directory at the meantime.
			/// </summary>
			/// <returns>The path to save new asset copy from source.</returns>
			/// <param name="source">Source object.</param>
			/// <param name="extension">File extension.</param>
			/*
			 * It's must to create folder before using AssetDatabase.GenerateUniqueAssetPath().
			 */
			private static string GenerateAssetPath(Object source, string extension){
				
				var _d = _assetRoot + "/" + GetTypeName(source.GetType());
				Directory.CreateDirectory(_d);
				
				var _p = string.Format("{0}/{1}.{2}", _d, source.GetInstanceID(), extension);
				return AssetDatabase.GenerateUniqueAssetPath(_p);
				
			}
			
			/// <summary>
			/// Load all the copies in asset root directory.
			/// </summary>
			/// <returns>The asset copies.</returns>
			/*
			 * Directory.GetDirectories and Directory.GetFiles return path relative to original.
			 * But it's not if from DirectoryInfo or FileInfo.
			 * Used to make sure to load asset with path relative to project directory.
			 */
			private static Object[] LoadAssetCopies(){

				if(!Directory.Exists(_assetRoot)) return new Object[0];

				Func<string, string, Object> _l = (p, t) => {

					var _a = AssetDatabase.LoadAssetAtPath<Object>(p);

					if(_a is Material) return (GetTypeName(_a.GetType()) == t) ? _a : null;
					if(!(_a is GameObject)) return null;

					var _c = ((GameObject)_a).GetComponents<Component>();
					return _c.LastOrDefault(_v => GetTypeName(_v.GetType()) == t);

				};

				return Directory.GetDirectories(_assetRoot
					
					).ToDictionary(_v => Path.GetFileName(_v), _v => Directory.GetFiles(_v)
					).SelectMany(_v => _v.Value.Select(_o => _l.Invoke(_o, _v.Key))
					
				).Where(_v => null != _v).ToArray();

			}

			/// <summary>
			/// Check if specified object has any reference to scene object.
			/// </summary>
			/// <returns><c>true</c>, if refer scene, <c>false</c> otherwise.</returns>
			/// <param name="obj">Object.</param>
			private static bool CheckReferScene(Object obj){

				var _p = new SerializedObject(obj).GetIterator();
				while(_p.NextVisible(true)){

					if(SerializedPropertyType.ObjectReference != _p.propertyType) continue;

					var _o = _p.objectReferenceValue;
					if(null != _o && !EditorUtility.IsPersistent(_o)) return true;

				}

				return false;

			}

			#endif

		}
		
	}
	
}
