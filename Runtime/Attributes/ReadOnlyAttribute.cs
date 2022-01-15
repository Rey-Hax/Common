using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		var enabled = (attribute as ReadOnlyAttribute).playModeOnly && !Application.isPlaying;
		using (var scope = new EditorGUI.DisabledGroupScope(!enabled))
		{
			EditorGUI.PropertyField(position, property, label, true);
		}
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return EditorGUI.GetPropertyHeight(property, label, true);
	}
}
#endif

/// <summary>
/// Make the field unable to be changed from the editor.
/// Does not work as expected with variable types that have a custom PropertyDrawer.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class ReadOnlyAttribute : PropertyAttribute
{
	public bool playModeOnly;

	/// <param name="playModeOnly">Set true to disallow editing field while editor is in playmode.</param>
	public ReadOnlyAttribute(bool playModeOnly = false)
	{
		this.playModeOnly = playModeOnly;
	}
}
