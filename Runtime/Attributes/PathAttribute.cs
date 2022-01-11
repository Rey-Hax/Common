using System;

namespace PhantasmicGames.Common
{
	/// <summary>
	/// A generic Attribute for associating classes, fields, properties, etc to a path.
	/// Mostly useful for Editor code.
	/// </summary>
	public class PathAttribute : Attribute
	{
		public virtual string path { get; protected set; }

		public PathAttribute(string path)
		{
			this.path = path;
		}
	}
}
