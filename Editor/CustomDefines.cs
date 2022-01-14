using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace PhantasmicGames.CommonEditor
{
    public abstract class CustomDefines
    {
        public abstract bool autoEvaluate { get; }
        public abstract DefineInfo[] defines { get; }

        public string[] allDefines => Array.ConvertAll(defines, new Converter<DefineInfo, string>(def => def.define));

        public struct DefineInfo
        {
            public string define;
            public Func<bool> condition;
            public BuildTargetGroup[] buildTargetGroups;
        }

        public void Evaluate()
        {
            CustomDefinesManager.Evaluate(this);
        }
    }

    [InitializeOnLoad]
    internal class CustomDefinesManager
    {
        private const string kAllCustomDefinesTypesKey = "ALL_CUSTOMDEFINES_TYPES";
        private const string kCustomDefinesKeyPrefix = "CUSTOM_DEFINES=";

        private static readonly BuildTargetGroup[] s_AvailableBuildTargetGroups;
        private static readonly List<string> s_RegisteredCustomDefinesTypeAssemblyQualifiedNames;

        static CustomDefinesManager()
        {
			s_AvailableBuildTargetGroups = GetAvailableBuildTargetGroups();
            s_RegisteredCustomDefinesTypeAssemblyQualifiedNames = EditorPrefs.GetString(kAllCustomDefinesTypesKey).Split(';').ToList();

            ClearRemovedCustomDefines();
            ProcessCustomDefines();

            EditorPrefs.SetString(kAllCustomDefinesTypesKey, string.Join(";", s_RegisteredCustomDefinesTypeAssemblyQualifiedNames));
        }

        private static BuildTargetGroup[] GetAvailableBuildTargetGroups()
        {
            var result = new List<BuildTargetGroup>();

            var allBuildTargets = (BuildTarget[])Enum.GetValues(typeof(BuildTarget));
            foreach (var buildTarget in allBuildTargets)
            {
                var buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
                if (result.Contains(buildTargetGroup))
                    continue;
                if (BuildPipeline.IsBuildTargetSupported(buildTargetGroup, buildTarget))
                    result.Add(buildTargetGroup);
            }

            return result.ToArray();
        }

        private static void ClearRemovedCustomDefines()
        {
            for (int i = s_RegisteredCustomDefinesTypeAssemblyQualifiedNames.Count - 1; i >= 0; i--)
            {
                var typeName = s_RegisteredCustomDefinesTypeAssemblyQualifiedNames[i];
                var type = Type.GetType(typeName);
                if (type == null)
                {
                    var key = GetCustomDefinesSaveKey(typeName);
                    var defines = EditorPrefs.GetString(key).Split(';');
                    EditorPrefs.DeleteKey(key);
                    foreach (var define in defines)
                    {
                        foreach (var targetGroup in s_AvailableBuildTargetGroups)
                            RemoveDefineForBuildTargetGroup(define, targetGroup);
                    }
                    s_RegisteredCustomDefinesTypeAssemblyQualifiedNames.RemoveAt(i);
                }
            }
        }

        private static void ProcessCustomDefines()
        {
            foreach (var customDefinesType in TypeCache.GetTypesDerivedFrom<CustomDefines>())
            {
                var instance = Activator.CreateInstance(customDefinesType) as CustomDefines;
                var allDefines = string.Join(";", instance.allDefines);
                var typeName = customDefinesType.AssemblyQualifiedName;
                var key = GetCustomDefinesSaveKey(typeName);
                EditorPrefs.SetString(key, allDefines);

                if (!s_RegisteredCustomDefinesTypeAssemblyQualifiedNames.Contains(typeName))
                    s_RegisteredCustomDefinesTypeAssemblyQualifiedNames.Add(typeName);

                if (instance.autoEvaluate)
                    Evaluate(instance);
            }
        }

        private static string GetCustomDefinesSaveKey(string assemblyQualifiedName) => string.Concat(kCustomDefinesKeyPrefix, assemblyQualifiedName);

        internal static void Evaluate(CustomDefines customDefines)
        {
            foreach (var defineInfo in customDefines.defines)
            {
                if (string.IsNullOrEmpty(defineInfo.define))
                    continue;

                var targetGroups = defineInfo.buildTargetGroups ?? s_AvailableBuildTargetGroups;
                var add = defineInfo.condition?.Invoke() == true;

                foreach (var targetGroup in targetGroups)
                {
                    if (add)
                        AddDefineForBuildTargetGroup(defineInfo.define, targetGroup);
                    else
                        RemoveDefineForBuildTargetGroup(defineInfo.define, targetGroup);
                }
            }
        }

        private static void AddDefineForBuildTargetGroup(string define, BuildTargetGroup targetGroup)
        {
            PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup, out string[] defines);
            if (defines.Contains(define))
                return;

            defines = defines.Append(define).ToArray();
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defines);
        }

        private static void RemoveDefineForBuildTargetGroup(string define, BuildTargetGroup targetGroup)
        {
            PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup, out string[] defines);
            if (!defines.Contains(define))
                return;

            defines = defines.Where(def => def != define).ToArray();
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defines);
        }

        //Note: Only works when deleting script from the Editor and not from File Explorer.
		private class CustomDefinesScriptDeletionListener : UnityEditor.AssetModificationProcessor
		{
            private static readonly string s_ScriptPath = GetScriptPath();

			static AssetDeleteResult OnWillDeleteAsset(string assetName, RemoveAssetOptions options)
			{
                var fullAssetPath = Path.GetFullPath(assetName);
                var deletingThisScript = s_ScriptPath.Contains(fullAssetPath);

                if (deletingThisScript)
                    ClearAllCustomDefines();

				return AssetDeleteResult.DidNotDelete;
			}

            private static void ClearAllCustomDefines()
            {
                foreach (var typeName in s_RegisteredCustomDefinesTypeAssemblyQualifiedNames)
                {
                    var key = GetCustomDefinesSaveKey(typeName);
                    var definesFromType = EditorPrefs.GetString(key).Split(';');
                    foreach (var targetGroup in s_AvailableBuildTargetGroups)
                    {
                        foreach (var define in definesFromType)
                            RemoveDefineForBuildTargetGroup(define, targetGroup);
                    }
                    EditorPrefs.DeleteKey(key);
                }
                EditorPrefs.DeleteKey(kAllCustomDefinesTypesKey);
            }

			private static string GetScriptPath([System.Runtime.CompilerServices.CallerFilePath] string fileName = null) => fileName;
		}
	}
}