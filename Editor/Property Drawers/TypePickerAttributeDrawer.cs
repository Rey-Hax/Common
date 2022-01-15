using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PhantasmicGames.CommonEditor
{
	[CustomPropertyDrawer(typeof(TypePickerAttribute))]
	public class TypePickerAttributeDrawer : SerializableTypeDrawer
	{
		private List<Type> m_Types;
		private GUIContent[] m_Options;
		private int m_Selection;

		private bool initialized;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (!initialized)
				Initialize(property);

			EditorGUI.BeginChangeCheck();
			m_Selection = EditorGUI.Popup(position, label, m_Selection, m_Options);
			if (EditorGUI.EndChangeCheck())
			{
				var newType = m_Types[m_Selection];
				property.FindPropertyRelative("m_AssemblyQualifiedName").stringValue = newType?.AssemblyQualifiedName;
				property.FindPropertyRelative("m_Name").stringValue = newType == null ? "Null" : newType.Name;
			}
		}

		private void Initialize(SerializedProperty property)
		{
			var attrib = property.GetAttributeInAncestors<TypePickerAttribute>();

			m_Types = TypeCache.GetTypesDerivedFrom(attrib.baseType).ToList();
			if (attrib.includeProvidedType)
				m_Types.Insert(0, attrib.baseType);
			m_Types.Insert(0, null);            //For selecting no type

			m_Options = new GUIContent[m_Types.Count];
			m_Options[0] = new GUIContent("Null");

			for (int i = 1; i < m_Types.Count; i++)
				m_Options[i] = new GUIContent(m_Types[i].Name);

			var currentType = GetCurrentType(property);
			m_Selection = m_Types.IndexOf(currentType);

			initialized = true;
		}

		private Type GetCurrentType(SerializedProperty property)
		{
			var assemblyQualifiedName = property.FindPropertyRelative("m_AssemblyQualifiedName").stringValue;
			return string.IsNullOrEmpty(assemblyQualifiedName) ? null : Type.GetType(assemblyQualifiedName);
		}
	}
}