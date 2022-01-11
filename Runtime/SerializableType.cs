using System;
using UnityEngine;

namespace PhantasmicGames.Common
{
	[Serializable]
	public class SerializableType : IEquatable<SerializableType>
	{
		[SerializeField] private string m_Name = "Null";
		[SerializeField] private string m_AssemblyQualifiedName;

		private Type m_Type;

		public Type type
		{
			get
			{
				if (m_Type != null)
					return m_Type;

				if (string.IsNullOrEmpty(m_AssemblyQualifiedName))
					return null;

				m_Type = Type.GetType(m_AssemblyQualifiedName);
				return m_Type;
			}
		}

		public SerializableType(Type type)
		{
			m_Type = type;
			m_Name = type.Name;
			m_AssemblyQualifiedName = type.AssemblyQualifiedName;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is SerializableType))
				return false;

			return Equals((SerializableType)obj);
		}

		public bool Equals(SerializableType other)
		{
			return type == other.type;
		}

		public override int GetHashCode()
		{
			return type == null ? 0 : type.GetHashCode();
		}

		public static bool operator ==(SerializableType a, SerializableType b)
		{
			if (ReferenceEquals(a, b))
				return true;

			if ((a is null) || (b is null))
				return false;

			return a.Equals(b);
		}

		public static bool operator !=(SerializableType a, SerializableType b)
		{
			return !(a == b);
		}

		public static implicit operator SerializableType(Type t)
		{
			return new SerializableType(t);
		}

		public static implicit operator Type(SerializableType t)
		{
			return t.type;
		}
	}
}