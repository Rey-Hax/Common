using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class TypePickerAttribute : PropertyAttribute
{
	public Type baseType;
	public bool includeProvidedType;

	public TypePickerAttribute(Type baseType, bool includeProvidedType = false)
	{
		this.baseType = baseType;
		this.includeProvidedType = includeProvidedType;
	}
}
