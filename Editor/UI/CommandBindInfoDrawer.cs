using System;

using HUtil.UI;
using HUtil.Runtime.Extension;
using HUtil.Editor;

using UnityEditor;
using UnityEngine;
using HUtil.UI.Binder;
using System.Linq;

namespace HUtil.Editor.UI
{
    [CustomPropertyDrawer(typeof(CommandBindingInfo))]
    public class CommandBindInfoDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //필요한 변수들 캐싱
            var instance = InspectorHelper.GetActualObject(property) as CommandBindingInfo;
            var binder = property.serializedObject.targetObject as MonoBinder;
            if(binder == null){
                base.OnGUI(position, property, label);
                return;
            }
            var viewRoot = binder.FindUIComponent();
            var viewModelType = InspectorHelper.GetAllConcreteTypesDerivedFrom(typeof(IViewModel))
                                    .FirstOrDefault(t => t.FullName == viewRoot.ViewModelType);

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
                string[] options = BinderReflectionHelper.GetAllBindablePropertyNames(viewModelType, BindingType.Command, direction).ToArray();
                InspectorHelper.DrawDropdownField(contentRect.SliceRightRatio(0.5f), pathProp, options.Select(o => new DropdownOption(o)).ToArray(), "Property");
            }
        }
    }
    
}