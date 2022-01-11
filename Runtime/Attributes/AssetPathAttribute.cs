using System;
using System.IO;

namespace PhantasmicGames.Common
{
	/// <summary>
	/// A generic Attribute for associating classes, fields, properties, etc to the path of an asset.
	/// </summary>
	public class AssetPathAttribute : PathAttribute
	{
		public string relativeAssetsPath { get; protected set; }
		public string packagesPath { get; protected set; }

		/// <summary>
		/// The path where an Asset is located. Null if the provided 'relativeAssetsPath' or 'packagesPath' did not point to an asset.
		/// </summary>
		public override string path 
		{
			get
			{
				if (!string.IsNullOrEmpty(packagesPath) && File.Exists(packagesPath))
					return packagesPath;
				else if (!string.IsNullOrEmpty(relativeAssetsPath))
				{
					if (File.Exists(relativeAssetsPath))
						return relativeAssetsPath;
					else
					{
						if (TryGetAssetsDirectoryPath(out string directory))
						{
							var fullAssetsPath = Path.Combine(directory, Path.GetFileName(relativeAssetsPath));
							if (File.Exists(fullAssetsPath))
								return fullAssetsPath;
						}
					}
				}
				return null;
			}
		}

		public AssetPathAttribute(string relativeAssetsPath = null, string packagesPath = null) : base(string.Empty)
		{
			this.relativeAssetsPath = relativeAssetsPath;
			this.packagesPath = packagesPath;
		}

		public bool TryGetAssetsDirectoryPath(out string directory)
		{
			var relativeDirectory = Path.GetDirectoryName(relativeAssetsPath);
			foreach (var dir in Directory.GetDirectories("Assets", "*.*", SearchOption.AllDirectories))
			{
				if (dir.Contains(relativeDirectory))
				{
					directory = dir;
					return true;
				}
			}
			directory = null;
			return false;
		}
	}
}