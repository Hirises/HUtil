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
            if(IsVisible(property, property.serializedObject, showIfAttribute))
            {
                return base.GetPropertyHeight(property, label);
            }
            return -EditorGUIUtility.standardVerticalSpacing;
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var showIfAttribute = attribute as ShowIfAttribute;
            if(IsVisible(property, property.serializedObject, showIfAttribute))
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }

        private bool IsVisible(SerializedProperty property, SerializedObject obj, ShowIfAttribute showIfAttribute)
        {
            int lastDot = property.propertyPath.LastIndexOf('.');
            string targetPath = "";
            if(lastDot != -1)
            {
                string parentPath = property.propertyPath.Substring(0, lastDot);
                targetPath = $"{parentPath}.{showIfAttribute.PropertyName}";
            }
            else
            {
                targetPath = showIfAttribute.PropertyName;
            }
            SerializedProperty targetProperty = obj.FindProperty(targetPath);
            if(showIfAttribute.UseValue)
            {
                return targetProperty.intValue == showIfAttribute.Value;
            }
            return targetProperty.boolValue;
        }
    }
}