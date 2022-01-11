using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class TypePickerAttribute : PropertyAttribute
{
	public Type type;
	public bool includeProvidedType;

	public TypePickerAttribute(Type type, bool includeProvidedType = false)
	{
		this.type = type;
		this.includeProvidedType = includeProvidedType;
	}
}
