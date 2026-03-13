using System;

using HUtil.Editor;
using HUtil.Runtime.Extension;

using UnityEditor;

using UnityEngine;

namespace HUtil.UI.Editor
{
    [CustomPropertyDrawer(typeof(BindingInfo))]
    public class BindingInfoDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2 + EditorGUIUtility.standardVerticalSpacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect curLine = position.GetStartLine();
            (var labelRect, var contentRect) = curLine.SliceVertical(EditorGUIUtility.labelWidth);
            EditorGUI.LabelField(labelRect, label);
            var propertyPathProp = property.FindPropertyRelative("_propertyPath");
            propertyPathProp.stringValue = EditorGUI.TextField(contentRect, propertyPathProp.stringValue);
            curLine = position.GetNextLine();
            (var left, var right) = curLine.SliceVerticalRatio(0.7f);
            (var IsCollection, var BaseType) = left.SliceVertical(45);
            (var lable, var toggle) = IsCollection.SliceVertical(30);
            EditorGUI.LabelField(lable, "List");
            var isCollectionProp = property.FindPropertyRelative("_type.IsCollection");
            isCollectionProp.boolValue = EditorGUI.Toggle(toggle, isCollectionProp.boolValue);
            var baseTypeProp = property.FindPropertyRelative("_type._baseType");
            baseTypeProp.enumValueIndex = EditorGUI.Popup(BaseType, baseTypeProp.enumValueIndex, baseTypeProp.enumDisplayNames);
            var allowedDirectionProp = property.FindPropertyRelative("_allowedDirection");
            allowedDirectionProp.enumValueIndex = EditorGUI.Popup(right, allowedDirectionProp.enumValueIndex, allowedDirectionProp.enumDisplayNames);
        }
    }
}