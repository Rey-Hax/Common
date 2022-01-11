using PhantasmicGames.Common;
using UnityEditor;
using UnityEngine;

namespace PhantasmicGames.CommonEditor
{
    [CustomPropertyDrawer(typeof(SerializableDictionary<,>))]
    public class SerializableDictionaryPropertyDrawer : PropertyDrawer
    {
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var kvpProp = property.FindPropertyRelative("m_KeyValuePairs");

			EditorGUI.PropertyField(position, kvpProp, label, true);

			var keyCollision = property.FindPropertyRelative("m_KeyCollision").boolValue;
			if (keyCollision)
			{
				var warning = new GUIContent("Contains duplicate keys.");
				var warningRect = new Rect(position.x + EditorGUIUtility.labelWidth, position.y, position.width - EditorGUIUtility.labelWidth - 50f, EditorGUIUtility.singleLineHeight);

				var warningSize = GUI.skin.label.CalcSize(warning);
				if (warningRect.width < warningSize.x)
					warning.text = string.Empty;

				EditorGUI.HelpBox(warningRect, warning.text, MessageType.Warning);
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var kvpProp = property.FindPropertyRelative("m_KeyValuePairs");
			return EditorGUI.GetPropertyHeight(kvpProp);
		}
	}
}