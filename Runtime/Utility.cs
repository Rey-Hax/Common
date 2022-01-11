using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PhantasmicGames.Common
{
	public class Utility
	{
		/// <summary>
		/// Returns true if <paramref name="type"/> inherits a Type with the generic type definition '<paramref name="genericTypeDefinition"/>'
		/// and sets the out <paramref name="genericType"/> to the generic Type.
		/// </summary>
		public static bool TypeInheritsGenericTypeDefinition(Type type, Type genericTypeDefinition, out Type genericType)
		{
			if(!genericTypeDefinition.IsGenericTypeDefinition)
				throw new Exception("Provided 'genericTypeDefinition' is not a generic type definition! Must be something like 'typeof(List<>)'");

			while (type != null && type != typeof(object))
			{
				if (type.IsGenericType && type.GetGenericTypeDefinition() == genericTypeDefinition)
				{
					genericType = type;
					return true;
				}
				type = type.BaseType;
			}
			genericType = null;
			return false;
		}
	}
}