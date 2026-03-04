using System;

using HUtil.Runtime.UI;
using HUtil.Runtime.Extension;
using HUtil.Editor;

using UnityEditor;
using UnityEngine;

namespace HUtil.Editor.UI
{
    [CustomPropertyDrawer(typeof(PropertyBindingInfo))]
    public class PropertyBindInfoDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var instance = InspectorHelper.GetActualObject(property) as PropertyBindingInfo;
            var direction = property.FindPropertyRelative("_direction");
            var path = property.FindPropertyRelative("_path");

            (var labelRect, var contentRect) = position.SliceVertical(EditorGUIUtility.labelWidth);

            // # Label
            EditorGUI.LabelField(labelRect, label);

            // # Direction
            var directionRect = contentRect;
            if(direction.enumValueIndex != (int)SyncronizeDirection.None){
                directionRect = directionRect.SliceLeftRatio(0.5f);
            }
            InspectorHelper.DrawFilteredEnumField<SyncronizeDirection>(directionRect, direction, direction => instance.AllowDirection.IsAllowed(direction));
            
            // # Path
            // Direction이 None이면 Path는 숨김
            if(direction.enumValueIndex != (int)SyncronizeDirection.None){
                string[] options = new string[] { "None", "Property", "Command" };
                InspectorHelper.DrawDropdownField(contentRect.SliceRightRatio(0.5f), path, options);
            }
        }
    }
}