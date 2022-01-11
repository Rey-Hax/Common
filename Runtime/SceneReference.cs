using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#endif

namespace PhantasmicGames.Common
{
	[System.Serializable]
	public class SceneReference
#if UNITY_EDITOR
		: IPreprocessBuildWithReport
#endif
	{
		[HideInInspector][SerializeField] private string m_Path;
		
#if UNITY_EDITOR
		[SerializeField] private SceneAsset m_SceneAsset;

		public string path => AssetDatabase.GetAssetPath(m_SceneAsset);

		public int callbackOrder => 0;

		private static readonly List<SceneReference> s_AllSceneReferences = new List<SceneReference>();

		public SceneReference()
		{
			s_AllSceneReferences.Add(this);
		}

		public void OnPreprocessBuild(BuildReport report)
		{
			foreach (var sceneReference in s_AllSceneReferences)
			{
				sceneReference.m_Path = sceneReference.path;
			}
		}
#else
		public string path => m_Path;
#endif

		public static implicit operator string(SceneReference sceneReference)
		{
			return sceneReference.path;
		}
	}
}