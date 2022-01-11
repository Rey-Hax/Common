using UnityEditor;
using UnityEngine;
using PhantasmicGames.Common;
using System.Linq;

namespace PhantasmicGames.CommonEditor
{
    [CustomPropertyDrawer(typeof(SceneReference))]
    public class SceneReferenceDrawer : PropertyDrawer
    {
		private static readonly float s_AddToBuildButtonWidth = 85f;
		private static readonly GUIContent s_AddToBuildContent = new GUIContent("Add To Build");

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var sceneAssetProp = property.FindPropertyRelative("m_SceneAsset");
			var pathProp = property.FindPropertyRelative("m_Path");

			if (sceneAssetProp.objectReferenceValue == null)
				EditorGUI.PropertyField(position, sceneAssetProp, label, true);
			else
			{
				pathProp.stringValue = AssetDatabase.GetAssetPath(sceneAssetProp.objectReferenceValue);

				bool isInBuild = EditorBuildSettings.scenes.Any(scene => scene.path == pathProp.stringValue);
				float width = isInBuild ? position.width : position.width - s_AddToBuildButtonWidth;

				var sceneAssetRect = new Rect(position.x, position.y, width, position.height);
				EditorGUI.PropertyField(sceneAssetRect, sceneAssetProp, label, true);

				if (!isInBuild)
				{
					var buttonRect = new Rect(sceneAssetRect.max.x + 4f, position.y, s_AddToBuildButtonWidth - 4f, position.height);
					if (GUI.Button(buttonRect, s_AddToBuildContent))
						EditorBuildSettings.scenes = EditorBuildSettings.scenes.Append(new EditorBuildSettingsScene(pathProp.stringValue, true)).ToArray();
				}
			}
		}
	}
}