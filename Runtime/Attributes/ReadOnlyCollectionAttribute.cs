using System;

namespace PhantasmicGames.Common
{
	/// <summary>
	/// Use for fields like Arrays or Lists to remove the ability to edit the collection in the editor.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class ReadOnlyCollectionAttribute : ReadOnlyAttribute
	{
		/// <param name="playModeOnly">Set true to disallow editing field while editor is in playmode.</param>
		public ReadOnlyCollectionAttribute(bool playModeOnly = false) : base(playModeOnly)
		{
		}
	}
}
