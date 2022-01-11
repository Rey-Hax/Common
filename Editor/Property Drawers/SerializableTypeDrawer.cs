using PhantasmicGames.Common;
using UnityEditor;
using UnityEngine;

namespace PhantasmicGames.CommonEditor
{
	[CustomPropertyDrawer(typeof(SerializableType))]
	public class SerializableTypeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.LabelField(position, label, new GUIContent(property.FindPropertyRelative("m_Name").stringValue));
		}
	}
}