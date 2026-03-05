using System;

using HUtil.UI;
using HUtil.Runtime.Extension;
using HUtil.Editor;

using UnityEditor;
using UnityEngine;
using HUtil.UI.Binder;
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
            //필요한 변수들 캐싱
            var instance = InspectorHelper.GetActualObject(property) as PropertyBindingInfo;
            var binder = property.serializedObject.targetObject as MonoBinder;
            var fieldInfo = InspectorHelper.GetFieldInfo(property);
            if(instance == null || binder == null || fieldInfo == null){
                EditorGUI.HelpBox(position, $"Internal error: {instance} {binder} {fieldInfo}", MessageType.Error);
                return;
            }
            var uiComponent = binder.FindUIComponent();
            if(uiComponent == null){
                EditorGUI.HelpBox(position, "UIComponent not found", MessageType.Error);
                return;
            }
            var viewModelType = InspectorHelper.GetAllConcreteTypesDerivedFrom(typeof(IViewModel))
                                    .FirstOrDefault(t => t.FullName == uiComponent.ViewModelType);

            //프로퍼티 캐싱
            var directionProp = property.FindPropertyRelative("_direction");
            var direction = (BindingMode)directionProp.enumValueIndex;
            var pathProp = property.FindPropertyRelative("_path");


            // # Label
            (var labelRect, var contentRect) = position.SliceVertical(EditorGUIUtility.labelWidth);
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