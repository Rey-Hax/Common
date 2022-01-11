using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomPropertyDrawer(typeof(TagAttribute))]
public class TagDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		if (property.propertyType == SerializedPropertyType.String)
			property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
		else
			EditorGUI.PropertyField(position, property, true);
	}
}
#endif

public class TagAttribute : PropertyAttribute
{
}
