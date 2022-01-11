using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

using UnityObject = UnityEngine.Object;

namespace PhantasmicGames.CommonEditor
{
	public class EditorUtils
	{
		public static T[] GetAssetsOfType<T>() where T : UnityObject
		{
			return InternalGetAssetsOfType<T>();
		}

		public static UnityObject[] GetAssetsOfType(Type type)
		{
			return InternalGetAssetsOfType<UnityObject>(type);
		}

		private static T[] InternalGetAssetsOfType<T>(Type type = null) where T : UnityObject
		{
			if (type == null)
				type = typeof(T);

			var isComponent = typeof(Component).IsAssignableFrom(type);
			var filter = isComponent ? "Prefab" : type.Name;
			var guids = AssetDatabase.FindAssets("t:" + filter);

			var result = new List<T>(guids.Length);
			for (int i = 0; i < guids.Length; i++)
			{
				var path = AssetDatabase.GUIDToAssetPath(guids[i]);
				if (isComponent)
				{
					var prefabGameObject = AssetDatabase.LoadAssetAtPath<GameObject>(path);
					var components = prefabGameObject.GetComponentsInChildren(type);
					result.AddRange(Array.ConvertAll(components, item => item as T));
				}
				else
				{
					var obj = AssetDatabase.LoadAssetAtPath<T>(path);
					if (obj)			//For the off chance that there are more than one type with the same Type.name (in different namespaces).
						result.Add(obj);
				}
			}
			return result.ToArray();
		}
	}
}