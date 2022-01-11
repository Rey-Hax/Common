using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIUtility
{
	private static readonly List<RaycastResult> s_RaycastResults = new List<RaycastResult>(8);
	
	public static bool IsScreenPointOverUI(Vector2 position, List<RaycastResult> raycastResults)
	{
		PointerEventData eventData = new PointerEventData(EventSystem.current) { position = position };
		EventSystem.current.RaycastAll(eventData, raycastResults);
		return raycastResults.Count > 0;
	}

	public static bool IsScreenPointOverUI(Vector2 position)
	{
		return IsScreenPointOverUI(position, s_RaycastResults);
	}

	public static bool IsTouchOverUI(Touch touch)
	{
		return EventSystem.current && EventSystem.current.IsPointerOverGameObject(touch.fingerId);
	}

	public static bool IsTouchOverUI(Touch touch, Transform ignore)
	{
		if (!IsTouchOverUI(touch))
			return false;

		if (IsScreenPointOverUI(touch.position, s_RaycastResults))
		{
			if (ignore == null)
				return false;
			else
			{
				for (int i = 0; i < s_RaycastResults.Count; i++)
				{
					if (!s_RaycastResults[i].gameObject.transform.IsChildOf(ignore))
						return true;
				}
				return false;
			}
		}
		else
			return false;
	}
}
