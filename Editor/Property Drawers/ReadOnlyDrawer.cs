using UnityEditor;
using UnityEngine;
using PhantasmicGames.Common;

namespace PhantasmicGames.CommonEditor
{
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
}