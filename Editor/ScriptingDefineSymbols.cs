using System.Collections.Generic;
using UnityEditor;
using System;
using System.Linq;

namespace PhantasmicGames.Common
{
	[InitializeOnLoad]
	public class ScriptingDefineSymbols
	{
		public readonly static BuildTargetGroup[] supportedBuildTargetGroups;

		public virtual List<DefineSymbol> defineSymbols => throw new NotImplementedException();

		static ScriptingDefineSymbols()
		{
			supportedBuildTargetGroups = GetSupportedBuildTargetGroups();
			ProcessSymbolConditions();
		}

		private static void ProcessSymbolConditions()
		{
			foreach (var symbolsType in TypeCache.GetTypesDerivedFrom<ScriptingDefineSymbols>())
				EvaluateScriptingDefineSymbols(symbolsType);
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

		#region Public API
		public static void EvaluateScriptingDefineSymbols<T>() where T : ScriptingDefineSymbols
		{
			EvaluateScriptingDefineSymbols(typeof(T));
		}

		public static void EvaluateScriptingDefineSymbols(Type symbolsType)
		{
			if (!typeof(ScriptingDefineSymbols).IsAssignableFrom(symbolsType))
				throw new Exception($"{symbolsType} is not a {nameof(ScriptingDefineSymbols)}!");

			var symbolsInstance = (ScriptingDefineSymbols)Activator.CreateInstance(symbolsType);
			foreach (var symbol in symbolsInstance.defineSymbols)
			{
				if (symbol.condition() == true)
				{
					if (symbol.buildTargetGroups == null)
						AddSymbol(symbol.text);
					else
						AddSymbol(symbol.text, symbol.buildTargetGroups);
				}
				else
				{
					if (symbol.buildTargetGroups == null)
						RemoveSymbol(symbol.text);
					else
						RemoveSymbol(symbol.text, symbol.buildTargetGroups);
				}
			}
		}

		/// <summary>
		/// Set a symbol for script compilation for all supported build target groups.
		/// </summary>
		public static void AddSymbol(string symbol)
		{
			foreach (var buildTargetGroup in supportedBuildTargetGroups)
			{
				AddSymbol(symbol, buildTargetGroup);
			}
		}

		/// <summary>
		/// Set a symbol for script compilation for the given build target group.
		/// </summary>
		public static void AddSymbol(string symbol, BuildTargetGroup targetGroup)
		{
			var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup).Split(';');

			//UNITY 2020 METHOD
			//PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup, out string[] defines);

			if (defines.Contains(symbol))
				return;

			defines = defines.Concat(new string[] { symbol }).ToArray();
			var joinedDefines = string.Join(";", defines);
			PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, joinedDefines);
		}

		/// <summary>
		/// Set a symbol for script compilation for the given build target groups.
		/// </summary>
		public static void AddSymbol(string symbol, BuildTargetGroup[] targetGroups)
		{
			foreach (var targetGroup in targetGroups)
			{
				AddSymbol(symbol, targetGroup);
			}
		}

		/// <summary>
		/// Remove a symbol for script compilation for all supported build target groups.
		/// </summary>
		public static void RemoveSymbol(string symbol)
		{
			foreach (var buildTargetGroup in supportedBuildTargetGroups)
			{
				RemoveSymbol(symbol, buildTargetGroup);
			}
		}

		/// <summary>
		/// Remove a symbol for script compilation for the given build target group.
		/// </summary>
		public static void RemoveSymbol(string symbol, BuildTargetGroup targetGroup)
		{
			var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup).Split(';');
			//UNITY 2020 METHOD
			//PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup, out string[] defines);

			if (!defines.Contains(symbol))
				return;

			defines = defines.Where(def => def != symbol).ToArray();
			var joinedDefines = string.Join(";", defines);
			PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, joinedDefines);
		}

		/// <summary>
		/// Removes a symbol for script compilation for the given build target groups.
		/// </summary>
		public static void RemoveSymbol(string symbol, BuildTargetGroup[] targetGroups)
		{
			foreach (var targetGroup in targetGroups)
			{
				RemoveSymbol(symbol, targetGroup);
			}
		}
		#endregion

		public class DefineSymbol
		{
			public string text;
			public Func<bool> condition;
			public BuildTargetGroup[] buildTargetGroups;
		}
	}
}