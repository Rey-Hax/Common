using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace PhantasmicGames.CommonEditor
{
	public class SearchableDrawer : DecoratorDrawer
	{
		#region Reflection FieldInfo and PropertyInfo
		private static readonly PropertyInfo s_PropertyHandlerCache = typeof(DecoratorDrawer).Assembly.GetType("UnityEditor.ScriptAttributeUtility")
		.GetProperty("propertyHandlerCache", BindingFlags.NonPublic | BindingFlags.Static);

		private static readonly FieldInfo s_PropertyHandlers = typeof(DecoratorDrawer).Assembly.GetType("UnityEditor.PropertyHandlerCache")
			.GetField("m_PropertyHandlers", BindingFlags.NonPublic | BindingFlags.Instance);

		private static readonly Type s_PropertyHandler = typeof(DecoratorDrawer).Assembly.GetType("UnityEditor.PropertyHandler");
#if UNITY_2021_1_OR_NEWER
		private static readonly FieldInfo s_PropertyHandler_PropertyDrawers = s_PropertyHandler.GetField("m_PropertyDrawers", BindingFlags.NonPublic | BindingFlags.Instance);
#else
	private static readonly FieldInfo s_PropertyHandler_PropertyDrawer = s_PropertyHandler.GetField("m_PropertyDrawer", BindingFlags.NonPublic | BindingFlags.Instance);
#endif
		private static readonly FieldInfo s_PropertyHandler_DecoratorDrawers = s_PropertyHandler.GetField("m_DecoratorDrawers", BindingFlags.NonPublic | BindingFlags.Instance);

		private static readonly MethodInfo s_EditorGUI_ValidateObjectFieldAssignment = typeof(EditorGUI).GetMethod("ValidateObjectFieldAssignment", BindingFlags.NonPublic | BindingFlags.Static);
		#endregion

		internal bool m_Initialized;

		public sealed override bool CanCacheInspectorGUI()
		{
			EnsureInitialization();
			return false;
		}

		public sealed override void OnGUI(Rect position)
		{
			EnsureInitialization();
		}

		public sealed override float GetHeight()
		{
			EnsureInitialization();
			return 0f;
		}

		private void EnsureInitialization()
		{
			if (!m_Initialized)
				Initialize();
		}

		private void Initialize()
		{
			//var propertyHandler = GetPropertyHandler();
			//var propertyDrawer = GetPropertyDrawer(propertyHandler);

			//if (propertyDrawer == null)
			//{
			//	propertyDrawer = new InternalDrawer(this);
			//	SetPropertyDrawer(propertyHandler, propertyDrawer);
			//}

			m_Initialized = true;
		}

		internal object GetPropertyHandler()
		{
			var propertyHandlerCache = s_PropertyHandlerCache.GetValue(null, null);
			var propertyHandlerDictionary = (IDictionary)s_PropertyHandlers.GetValue(propertyHandlerCache);

			var propertyHandlers = propertyHandlerDictionary.Values;
			foreach (var propertyHandler in propertyHandlers)
			{
				var decoratorDrawers = (List<DecoratorDrawer>)s_PropertyHandler_DecoratorDrawers.GetValue(propertyHandler);

				if (decoratorDrawers == null)
					continue;

				var index = decoratorDrawers.IndexOf(this);
				if (index < 0)
					continue;

				return propertyHandler;
			}
			return null;
		}

		internal static PropertyDrawer GetPropertyDrawer(object propertyHandler)
		{
#if UNITY_2021_1_OR_NEWER
			var propertyDrawers = (List<PropertyDrawer>)s_PropertyHandler_PropertyDrawers.GetValue(propertyHandler);
			return propertyDrawers != null && propertyDrawers.Count != 0 ? propertyDrawers[0] : null;
#else
		return (PropertyDrawer)s_PropertyHandler_PropertyDrawer.GetValue(propertyHandler);
#endif
		}

		internal static void SetPropertyDrawer(object propertyHandler, PropertyDrawer propertyDrawer)
		{
#if UNITY_2021_1_OR_NEWER
			List<PropertyDrawer> propertyDrawers = (List<PropertyDrawer>)s_PropertyHandler_PropertyDrawers.GetValue(propertyHandler);
			if (propertyDrawers == null)
				propertyDrawers = new List<PropertyDrawer>();
			propertyDrawers.Add(propertyDrawer);
			s_PropertyHandler_PropertyDrawers.SetValue(propertyHandler, propertyDrawers);
#else
		s_PropertyHandler_PropertyDrawer.SetValue(propertyHandler, propertyDrawer);
#endif
		}

		//private class InternalDrawer : PropertyDrawer
		//{
		//	private readonly Sea arrayDrawer;

		//	internal InternalDrawer(ArrayDrawer arrayDrawer)
		//	{
		//		this.arrayDrawer = arrayDrawer;
		//	}

		//	public override bool CanCacheInspectorGUI(SerializedProperty property)
		//	{
		//		if(arrayDrawer.

		//		EnsureOnEnableCalled(property);
		//		return arrayDrawer.CanCacheInspectorGUI(property);
		//	}

		//	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		//	{
		//		EnsureOnEnableCalled(property);
		//		arrayDrawer.OnGUI(position, property, label);
		//	}

		//	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		//	{
		//		EnsureOnEnableCalled(property);
		//		return arrayDrawer.GetPropertyHeight(property, label);
		//	}

		//	private void FinishInitialization(SerializedProperty property)
		//	{

		//	}

		//	//private void EnsureOnEnableCalled(SerializedProperty property)
		//	//{
		//	//	if (arrayDrawer.m_OnEnableCalled)
		//	//		return;

		//	//	arrayDrawer.list = new ReorderableList(property.serializedObject, property, true, false, true, true)
		//	//	{
		//	//		headerHeight = 0f,
		//	//		footerHeight = 0f,
		//	//		drawElementCallback = (rect, index, isActive, isFocused) => EditorGUI.PropertyField(rect, property.GetArrayElementAtIndex(index), true),
		//	//		elementHeightCallback = (index) => EditorGUI.GetPropertyHeight(property.GetArrayElementAtIndex(index))
		//	//	};

		//	//	arrayDrawer.OnEnable(property);
		//	//	arrayDrawer.m_OnEnableCalled = true;
		//	//}
		//}
	}
}