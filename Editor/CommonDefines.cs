using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace PhantasmicGames.CommonEditor
{
	public class CommonDefines : CustomDefines
	{
		public override bool autoEvaluate => true;

		public override DefineInfo[] defines => new DefineInfo[]
		{
			new DefineInfo()
			{
				define = "PHANTASMICGAMES_COMMON",
				condition = () => true,
			}
		};

		//internal class CustomAssetModificationProcessor : UnityEditor.AssetModificationProcessor
		//{
		//	private static readonly DirectoryInfo s_ScriptDirectoryInfo = new DirectoryInfo(GetScriptPath());

		//	static AssetDeleteResult OnWillDeleteAsset(string assetName, RemoveAssetOptions options)
		//	{
		//		var assetDir = new DirectoryInfo(assetName);

		//		Debug.Log("Asset: " + assetDir.FullName);
		//		Debug.Log("Script: " + s_ScriptDirectoryInfo.FullName);

		//		return AssetDeleteResult.DidNotDelete;
		//	}

		//	private static string GetScriptPath([System.Runtime.CompilerServices.CallerFilePath] string fileName = null) => fileName;
		//}

	}
}