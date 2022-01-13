using System;
using System.Collections.Generic;
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

        static CustomDefinesManager()
        {
			s_AvailableBuildTargetGroups = GetAvailableBuildTargetGroups();

            var registeredCustomDefinesTypeNames = EditorPrefs.GetString(kAllCustomDefinesTypesKey).Split(';').ToList();
            ClearRemovedCustomDefines();
            ProcessCustomDefines();

            EditorPrefs.SetString(kAllCustomDefinesTypesKey, string.Join(";", registeredCustomDefinesTypeNames));

            BuildTargetGroup[] GetAvailableBuildTargetGroups()
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
            void ClearRemovedCustomDefines()
            {
                for (int i = registeredCustomDefinesTypeNames.Count - 1; i >= 0; i--)
                {
                    var typeName = registeredCustomDefinesTypeNames[i];
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
                        registeredCustomDefinesTypeNames.RemoveAt(i);
                    }
                }
            }
            void ProcessCustomDefines()
            {
                foreach (var customDefinesType in TypeCache.GetTypesDerivedFrom<CustomDefines>())
                {
                    var instance = Activator.CreateInstance(customDefinesType) as CustomDefines;
                    var allDefines = string.Join(";", instance.allDefines);
                    var typeName = customDefinesType.AssemblyQualifiedName;
                    var key = GetCustomDefinesSaveKey(typeName);
                    EditorPrefs.SetString(key, allDefines);

                    if (!registeredCustomDefinesTypeNames.Contains(typeName))
                        registeredCustomDefinesTypeNames.Add(typeName);

                    if (instance.autoEvaluate)
                        Evaluate(instance);
                }
            }
		    string GetCustomDefinesSaveKey(string assemblyQualifiedName) => string.Concat(kCustomDefinesKeyPrefix, assemblyQualifiedName);
        }

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

        public static void AddDefineForBuildTargetGroup(string define, BuildTargetGroup targetGroup)
        {
            PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup, out string[] defines);
            if (defines.Contains(define))
                return;

            defines = defines.Append(define).ToArray();
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defines);
        }

        public static void RemoveDefineForBuildTargetGroup(string define, BuildTargetGroup targetGroup)
        {
            PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup, out string[] defines);
            if (!defines.Contains(define))
                return;

            defines = defines.Where(def => def != define).ToArray();
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defines);
        }
	}
}