using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

//Reference
//https://wiki.unity3d.com/index.php/Custom_Defines_Manager#C.23_Script_-_AssetDefineManager.cs

[InitializeOnLoad]
public class AssetDefines : AssetPostprocessor
{
	public static readonly BuildTargetGroup[] s_SupportedBuildTargetGroups;

	private static List<AssetDefine> s_AssetDefines = new List<AssetDefine>();

	static AssetDefines()
	{
		s_SupportedBuildTargetGroups = GetSupportedBuildTargetGroups();
	}

	protected static void RegisterAssetDefines(List<AssetDefine> assetDefines)
	{
		s_AssetDefines = s_AssetDefines.Concat(assetDefines).ToList();
		ValidateDefines();
	}

	private static void ValidateDefines()
	{
		foreach (var assetDefine in s_AssetDefines)
		{
			if (AssetDatabase.IsValidFolder(assetDefine.assetPath))
				AddCompileDefine(assetDefine);
		}
	}

	private static BuildTargetGroup[] GetSupportedBuildTargetGroups()
	{
		var supportedBuildTargetGroup = new List<BuildTargetGroup>();

		var allBuildTargets = (BuildTarget[])Enum.GetValues(typeof(BuildTarget));
		foreach (var buildTarget in allBuildTargets)
		{
			var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
			if (supportedBuildTargetGroup.Contains(buildTargetGroup))
				continue;
			if (BuildPipeline.IsBuildTargetSupported(buildTargetGroup, buildTarget))
				supportedBuildTargetGroup.Add(buildTargetGroup);
		}

		return supportedBuildTargetGroup.ToArray();
	}

	private static void AddCompileDefine(AssetDefine assetDefine)
	{
		BuildTargetGroup[] targetGroups;
		if (assetDefine.buildTargetGroups == null)
			targetGroups = s_SupportedBuildTargetGroups;
		else
			targetGroups = assetDefine.buildTargetGroups;

		foreach (var targetGroup in targetGroups)
		{
			if (targetGroup == BuildTargetGroup.Unknown)
				continue;

			string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
			if (!defines.Contains(assetDefine.defineSymbol))
			{
				if (!string.IsNullOrEmpty(defines))         //if the list is empty, we don't need to append a semicolon first
					defines += ";";

				defines += assetDefine.defineSymbol;
				PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defines);
			}
		}
	}

	private static void RemoveCompileDefine(AssetDefine assetDefine)
	{
		BuildTargetGroup[] targetGroups;
		if (assetDefine.buildTargetGroups == null)
			targetGroups = s_SupportedBuildTargetGroups;
		else
			targetGroups = assetDefine.buildTargetGroups;

		foreach (var targetGroup in targetGroups)
		{
			string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
			int index = defines.IndexOf(assetDefine.defineSymbol);
			if (index < 0)
				continue;
			else
				index -= 1;

			int lengthToRemove = Mathf.Min(assetDefine.defineSymbol.Length + 1, defines.Length - index);

			defines = defines.Remove(index, lengthToRemove);
			PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defines);
		}
	}

	static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string assetPath in deletedAssets)
		{
			for (int i = 0; i < s_AssetDefines.Count; i++)
			{
				if (assetPath == s_AssetDefines[i].assetPath)
				{
					RemoveCompileDefine(s_AssetDefines[i]);
					s_AssetDefines.RemoveAt(i);
					break;
				}
			}
		}
	}

	public struct AssetDefine
	{
		public readonly string assetPath;
		public readonly string defineSymbol;
		public readonly BuildTargetGroup[] buildTargetGroups;

		public AssetDefine(string defineSymbol, string assetPath, BuildTargetGroup[] buildTargetGroups)
		{
			this.defineSymbol = defineSymbol;
			this.assetPath = assetPath;
			this.buildTargetGroups = buildTargetGroups;
		}

		public bool isValid => AssetDatabase.IsValidFolder(assetPath);
	}
}
