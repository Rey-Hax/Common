using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

//Based off of: https://gist.github.com/yagero/0922654a0645fbfd21b926ca048ed6a7

namespace PhantasmicGames.Common
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ScriptExecutionOrderAttribute : Attribute
    {
        public int order;
        public ScriptExecutionOrderAttribute(int order)
        {
            this.order = order;
        }

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        private static void ApplyScriptExecutionOrder()
        {
            foreach (var monoScript in MonoImporter.GetAllRuntimeMonoScripts())
            {
                var classType = monoScript.GetClass();
                if (classType == null)
                    continue;

                foreach (var attribute in GetCustomAttributes(classType, typeof(ScriptExecutionOrderAttribute)))
                {
                    var newOrder = ((ScriptExecutionOrderAttribute)attribute).order;
                    if (MonoImporter.GetExecutionOrder(monoScript) != newOrder)
                        MonoImporter.SetExecutionOrder(monoScript, newOrder);
                }
            }
        }
#endif
    }
}