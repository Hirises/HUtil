using System;

using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

namespace HUtil.Attribute.Editor
{
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public class ShowIfDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var showIfAttribute = attribute as ShowIfAttribute;
            if(IsVisible(property, showIfAttribute))
            {
                return EditorGUI.GetPropertyHeight(property, label);
            }
            return -EditorGUIUtility.standardVerticalSpacing;
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var showIfAttribute = attribute as ShowIfAttribute;
            if(IsVisible(property, showIfAttribute))
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }

        private bool IsVisible(SerializedProperty property, ShowIfAttribute showIfAttribute)
        {
            SerializedProperty targetProperty = AttributeEditortimeReflectionHelper.GetRelativeProperty(property, showIfAttribute.PropertyName);
            if(showIfAttribute.UseValue)
            {
                return targetProperty.intValue == showIfAttribute.Value;
            }
            return targetProperty.boolValue;
        }
    }
}