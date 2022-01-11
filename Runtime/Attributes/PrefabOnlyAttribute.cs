using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[AttributeUsage(AttributeTargets.Field)]
public class PrefabOnlyAttribute : PropertyAttribute
{
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(PrefabOnlyAttribute))]
public class PrefabOnlyDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		property.objectReferenceValue = EditorGUI.ObjectField(position, label, property.objectReferenceValue, fieldInfo.FieldType, false);
	}
}
#endif
