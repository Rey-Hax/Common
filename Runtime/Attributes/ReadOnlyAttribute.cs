using System;
using UnityEngine;

namespace PhantasmicGames.Common
{
	/// <summary>
	/// Make the field unable to be changed from the editor.
	/// Does not work as expected with variable types that have a custom PropertyDrawer.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class ReadOnlyAttribute : PropertyAttribute
	{
		public bool playModeOnly;

		/// <param name="playModeOnly">Set true to disallow editing field while editor is in playmode.</param>
		public ReadOnlyAttribute(bool playModeOnly = false)
		{
			this.playModeOnly = playModeOnly;
		}
	}
}
