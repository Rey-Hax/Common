using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;

[CustomPropertyDrawer(typeof(OnChangedAttribute))]
public class OnChangedAttributeDrawer : PropertyDrawer
{
	private const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginChangeCheck();
		EditorGUI.PropertyField(position, property, label, true);
		if (EditorGUI.EndChangeCheck())
		{
			var index = property.propertyPath.LastIndexOf('.');

			object obj;
			if (index == -1)
				obj = property.serializedObject.targetObject;
			else
			{
				var targetPropPath = property.propertyPath.Remove(index);
				var prop = property.serializedObject.FindProperty(targetPropPath);

				//FIX ME
				//obj = prop.managedReferenceValue;
				obj = null;
			}

			var method = fieldInfo.DeclaringType.GetMethod((attribute as OnChangedAttribute).methodName, bindingFlags);
			if (method != null && method.GetParameters().Length == 0)
				method.Invoke(obj, null);
		}
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return EditorGUI.GetPropertyHeight(property, label, true);
	}
}
#endif

public class OnChangedAttribute : PropertyAttribute
{
	public readonly string methodName;

	public OnChangedAttribute(string methodName)
	{
		this.methodName = methodName;
	}
}
