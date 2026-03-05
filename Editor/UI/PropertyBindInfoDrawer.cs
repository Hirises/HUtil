using System;

using HUtil.Runtime.UI;
using HUtil.Runtime.Extension;
using HUtil.Editor;

using UnityEditor;
using UnityEngine;
using HUtil.Runtime.UI.Binder;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

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
            var fieldInfo = InspectorHelper.GetFieldInfo(property);
            var viewModelValueAttribute = fieldInfo.GetCustomAttribute<ViewModelValueAttribute>();
            var binder = property.serializedObject.targetObject as MonoBinder;
            if(binder == null){
                base.OnGUI(position, property, label);
                return;
            }
            var viewRoot = binder.FindUIComponent();
            var viewModelType = BinderReflectionHelper.GetAllViewModelTypes().FirstOrDefault(t => t.FullName == viewRoot.ViewModelType);

            var directionProp = property.FindPropertyRelative("_direction");
            var direction = (BindingMode)directionProp.enumValueIndex;
            var pathProp = property.FindPropertyRelative("_path");

            (var labelRect, var contentRect) = position.SliceVertical(EditorGUIUtility.labelWidth);

            // # Label
            EditorGUI.LabelField(labelRect, label);

            // # Direction
            var directionRect = contentRect;
            if(directionProp.enumValueIndex != (int)BindingMode.None){
                directionRect = directionRect.SliceLeftRatio(0.5f);
            }
            InspectorHelper.DrawFilteredEnumField<BindingMode>(directionRect, directionProp, direction => instance.AllowDirection.IsAllowed(direction));
            
            // # Path
            // Direction이 None이면 Path는 숨김
            if(directionProp.enumValueIndex != (int)BindingMode.None){
                string[] options = BinderReflectionHelper.GetAllBindablePropertyNames(viewModelType, instance.ReceivingType, direction).ToArray();
                InspectorHelper.DrawDropdownField(contentRect.SliceRightRatio(0.5f), pathProp, options.Select(o => new DropdownOption(o)).ToArray(), "Property");
            }
        }
    }
    
}