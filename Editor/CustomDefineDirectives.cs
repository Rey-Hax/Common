using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace PhantasmicGames.CommonEditor
{
    public abstract class CustomDefineDirectives
    {
        private readonly static BuildTargetGroup[] availableBuildTargetGroups = GetAvailableBuildTargetGroups();

        public abstract bool autoEvaluate { get; }
        public abstract DefineDirective[] defineDirectives { get; }

        public struct DefineDirective
        {
            public string symbol;
            public Func<bool> condition;
            public BuildTargetGroup[] buildTargetGroups;
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

        public void Evaluate()
        {
            foreach (var directive in defineDirectives)
            {
                if (directive.condition?.Invoke() == true)
                    AddDefineDirective(directive);
                else
                    RemoveDefineDirective(directive);
            }
        }

        private void AddDefineDirective(DefineDirective directive)
        {
            var targetGroups = directive.buildTargetGroups ?? availableBuildTargetGroups;
            foreach (var targetGroup in targetGroups)
            {
                PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup, out string[] defines);

                if (defines.Contains(directive.symbol))
                    continue;

                defines = defines.Concat(new string[] { directive.symbol }).ToArray();
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defines);
            }
        }

        private void RemoveDefineDirective(DefineDirective directive)
        {
            var targetGroups = directive.buildTargetGroups ?? availableBuildTargetGroups;
            foreach (var targetGroup in targetGroups)
            {
                PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup, out string[] defines);

                if (!defines.Contains(directive.symbol))
                    continue;

                defines = defines.Where(def => def != directive.symbol).ToArray();
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, defines);
            }
        }
    }

    [InitializeOnLoad]
    internal class DirectivesAutoEvaluator
    {
        static DirectivesAutoEvaluator()
        {
            foreach (var customDirectivesType in TypeCache.GetTypesDerivedFrom<CustomDefineDirectives>())
			{
				var instance = (CustomDefineDirectives)Activator.CreateInstance(customDirectivesType);
				if (instance.autoEvaluate)
                    instance.Evaluate();
			}
		}
    }
}