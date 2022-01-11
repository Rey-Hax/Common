using UnityEditor;
using UnityEngine;
using PhantasmicGames.Common;

namespace PhantasmicGames.CommonEditor
{
	[CustomPropertyDrawer(typeof(ReadOnlyCollectionAttribute))]
	public class ReadOnlyCollectionDrawer : DecoratorDrawer
	{
		public override void OnGUI(Rect position)
		{
			var enabled = (attribute as ReadOnlyAttribute).playModeOnly && !Application.isPlaying;
			if (!enabled)
				EditorGUI.BeginDisabledGroup(true);
		}

		public override float GetHeight()
		{
			return 0f;
		}
	}
}