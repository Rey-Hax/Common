using System;
using System.IO;
using UnityEngine;

namespace PhantasmicGames.Common
{
	public static class ExtensionMethods
	{
		public static Rect GetRemainingRect(this Rect rect, params Rect[] rects)
		{
			foreach (var r in rects)
			{
				rect.y += r.height;
				rect.height -= r.height;
			}

			rect.height = Mathf.Max(rect.height, 0f);

			return rect;
		}

		public static Rect ShrinkHeightAndOffset(this Rect rect, float amount)
		{
			rect.height -= amount;
			rect.y += amount;
			return rect;
		}

		public static bool IsChildOfPath(this string child, string parent)
		{
			if (string.IsNullOrEmpty(child))
				throw new NullReferenceException();

			if (string.IsNullOrEmpty(parent))
				throw new NullReferenceException();

			//if (!Directory.Exists(parent))
			//	throw new Exception($"'{parent}' is not a valid path.");

			//if(!Directory.Exists(child))
			//	throw new Exception($"'{child}' is not a valid path.");

			return Path.GetFullPath(child).StartsWith(Path.GetFullPath(parent));
		}

		public static bool IsSamePath(this string s, string other)
		{
			//if (!Directory.Exists(s))
			//	throw new Exception($"'{s}' is not a valid path.");

			//if (!Directory.Exists(other))
			//	throw new Exception($"'{other}' is not a valid path.");

			return string.Equals(Path.GetFullPath(s), Path.GetFullPath(other));
		}
	}
}