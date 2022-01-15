using System;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ReadOnlyCollectionAttribute))]
public class ReadOnlyCollectionDrawer : DecoratorDrawer
{
	public override void OnGUI(Rect position)
	{
		var enabled = (attribute as ReadOnlyAttribute).playModeOnly && !Application.isPlaying;
		if (!enabled)
			EditorGUI.BeginDisabledGroup(true);
	}

	public override float GetHeight()
	{
		return 0f;
	}
}
#endif

/// <summary>
/// Use for fields like Arrays or Lists to remove the ability to edit the collection in the editor.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class ReadOnlyCollectionAttribute : ReadOnlyAttribute
{
	/// <param name="playModeOnly">Set true to disallow editing field while editor is in playmode.</param>
	public ReadOnlyCollectionAttribute(bool playModeOnly = false) : base(playModeOnly)
	{
	}
}