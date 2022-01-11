using UnityEngine;

[System.Serializable]
public struct Range
{
	[SerializeField] private float m_Min;
	[SerializeField] private float m_Max;

	public float min 
	{ 
		get => m_Min;
		set => m_Min = Mathf.Min(value, m_Max);
	}

	public float max
	{
		get => m_Max;
		set => m_Max = Mathf.Max(value, m_Min);
	}

	public Range(float min, float max)
	{
		m_Min = Mathf.Min(min, max);
		m_Max = Mathf.Max(min, max);
	}

	public bool ValueInRange(float value)
	{
		return value >= m_Min && value <= m_Max;
	}

	public float GetClampedValue(float value)
	{
		if (value >= m_Max)
			return m_Max;
		else if (value <= m_Min)
			return m_Min;
		else
			return value;
	}
}
