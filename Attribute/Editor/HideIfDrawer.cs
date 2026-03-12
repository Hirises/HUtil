using System;

using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

namespace HUtil.Attribute.Editor
{
    [CustomPropertyDrawer(typeof(HideIfAttribute))]
    public class HideIfDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var hideIfAttribute = attribute as HideIfAttribute;
            if(IsHidden(property, hideIfAttribute))
            {
                return -EditorGUIUtility.standardVerticalSpacing;
            }
            return base.GetPropertyHeight(property, label);
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var hideIfAttribute = attribute as HideIfAttribute;
            if(!IsHidden(property, hideIfAttribute))
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }

        private bool IsHidden(SerializedProperty property, HideIfAttribute hideIfAttribute)
        {
            SerializedProperty targetProperty = AttributeEditortimeReflectionHelper.GetRelativeProperty(property, hideIfAttribute.PropertyName);
            if(hideIfAttribute.UseValue)
            {
                return targetProperty.intValue == hideIfAttribute.Value;
            }
            return targetProperty.boolValue;
        }
    }
}