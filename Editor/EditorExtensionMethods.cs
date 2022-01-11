using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PhantasmicGames.CommonEditor
{
	public static class EditorExtensionMethods
	{
		public static FieldInfo GetFieldInfo(this SerializedProperty prop)
		{
			var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
			var pathLevels = prop.propertyPath.Split('.');

			var t = prop.serializedObject.targetObject.GetType();
			FieldInfo fieldInfo = null;
			for (int i = 0; i < pathLevels.Length; i++)
			{
				if (pathLevels[i] == "Array")
				{
					i += 2;
					continue;
				}

				fieldInfo = t.GetField(pathLevels[i], bindingFlags);

				t = fieldInfo.FieldType;
			}
			return fieldInfo;
		}

		public static SerializedProperty GetRootProperty(this SerializedProperty prop)
		{
			return prop.serializedObject.FindProperty(prop.propertyPath.Split('.')[0]);
		}

		public static SerializedProperty GetParentProperty(this SerializedProperty prop)
		{
			var removeIndex = prop.propertyPath.LastIndexOf('.');
			var parentPath = prop.propertyPath.Remove(removeIndex);
			return prop.serializedObject.FindProperty(parentPath);
		}

		public static SerializedProperty[] GetAncestorProperties(this SerializedProperty property, bool includeSelf = true)
		{
			var iterator = property.serializedObject.GetIterator();
			iterator.Next(true);

			var properties = new List<SerializedProperty>();
			while (iterator.Next(property.propertyPath.Contains(iterator.propertyPath)))
			{
				if (property.propertyPath.Contains(iterator.propertyPath))
				{
					if (property == iterator && includeSelf == false)
						continue;
					properties.Add(iterator.Copy());
				}
			}
			return properties.ToArray();
		}

		public static Attribute GetAttribute(this SerializedProperty property, Type type)
		{
			return (Attribute)property.GetFieldInfo().GetCustomAttributes(type, true).FirstOrDefault();
		}

		public static T GetAttribute<T>(this SerializedProperty property) where T : Attribute
		{
			return (T)property.GetAttribute(typeof(T));
		}

		public static Attribute GetAttributeInAncestors(this SerializedProperty property, Type type)
		{
			Attribute attribute = null;
			foreach (var prop in property.GetAncestorProperties())
			{
				attribute = prop.GetAttribute<Attribute>();
				if (attribute != null)
					break;
			}
			return attribute;
		}

		public static T GetAttributeInAncestors<T>(this SerializedProperty property) where T : Attribute
		{
			return property.GetAttributeInAncestors(typeof(T)) as T;
		}

		/// <summary>
		/// Removes <paramref name="rects"/>' heights from <paramref name="rect"/> with a EditorGUIUtility.standardVerticalSpacing added in between <paramref name="rects"/>.
		/// Used getting rects for EditorGUI methods.
		/// </summary>
		/// <param name="rect"></param>
		/// <param name="rects"></param>
		public static Rect RemoveUsedRects(this Rect rect, params Rect[] rects)
		{
			for (int i = 0; i < rects.Length; i++)
			{
				float heightToRemove = rects[i].height + EditorGUIUtility.standardVerticalSpacing;
				rect.y += heightToRemove;
				rect.height -= heightToRemove;
			}

			return rect;
		}
	}
}