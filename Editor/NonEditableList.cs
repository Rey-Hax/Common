using UnityEditor;
using UnityEngine;

namespace PhantasmicGames.CommonEditor
{
	public class NonEditableList
	{
		public delegate float ElementHeightCallbackDelegate(int index);
		public delegate void DrawElementCallbackDelegate(Rect rect, int index);

		public SerializedObject serializedObject;
		public SerializedProperty serializedProperty;
		public GUIContent label;

		public ElementHeightCallbackDelegate elementHeightCallback;
		public DrawElementCallbackDelegate drawElementCallback;

		public int count => serializedProperty == null ? 0 : serializedProperty.arraySize;

		public NonEditableList(SerializedObject serializedObject, SerializedProperty serializedProperty, GUIContent label = null)
		{
			this.serializedObject = serializedObject;
			this.serializedProperty = serializedProperty;
			this.label = label ?? new GUIContent(serializedProperty.displayName);
		}

		public void DoLayoutList()
		{
			var totalHeight = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing + GetListElementHeight();
			var rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, totalHeight, GUILayout.ExpandWidth(true));

			DoList(rect);
		}

		public void DoList(Rect rect)
		{
			serializedObject.Update();
			var foldoutHeaderRect = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);

			rect = rect.RemoveUsedRects(foldoutHeaderRect);

			serializedProperty.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(foldoutHeaderRect, serializedProperty.isExpanded, label);

			GUI.enabled = false;
			if (serializedProperty.isExpanded)
			{
				EditorGUI.indentLevel++;
				var elementRect = new Rect(rect.x, rect.y, rect.width, 0f);
				for (int i = 0; i < count; i++)
				{
					elementRect.height = elementHeightCallback(i);

					if (drawElementCallback != null)
						drawElementCallback.Invoke(elementRect, i);
					else
						EditorGUI.PropertyField(elementRect, serializedProperty.GetArrayElementAtIndex(i));

					elementRect.y += elementRect.height + EditorGUIUtility.standardVerticalSpacing;
				}
				EditorGUI.indentLevel--;
			}
			GUI.enabled = true;

			EditorGUI.EndFoldoutHeaderGroup();
		}

		private float GetListElementHeight()
		{
			if (elementHeightCallback == null)
				return EditorGUIUtility.singleLineHeight;
			else
			{
				float result = 0f;
				for (int i = 0; i < count; i++)
					result += elementHeightCallback.Invoke(i);

				return result;
			}
		}
	}
}