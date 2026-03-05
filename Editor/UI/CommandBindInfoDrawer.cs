using System;

using HUtil.Runtime.UI;
using HUtil.Runtime.Extension;
using HUtil.Editor;

using UnityEditor;
using UnityEngine;
using HUtil.Runtime.UI.Binder;
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
            var instance = InspectorHelper.GetActualObject(property) as CommandBindingInfo;
            var binder = property.serializedObject.targetObject as MonoBinder;
            if(binder == null){
                base.OnGUI(position, property, label);
                return;
            }
            var viewRoot = binder.FindViewRoot();
            var viewModelType = ReflectionHelper.GetAllViewModelTypes().FirstOrDefault(t => t.FullName == viewRoot.ViewModelType);

            var directionProp = property.FindPropertyRelative("_direction");
            var direction = (SyncronizeDirection)directionProp.enumValueIndex;
            var pathProp = property.FindPropertyRelative("_path");

            (var labelRect, var contentRect) = position.SliceVertical(EditorGUIUtility.labelWidth);

            // # Label
            EditorGUI.LabelField(labelRect, label);

            // # Direction
            var directionRect = contentRect;
            if(directionProp.enumValueIndex != (int)SyncronizeDirection.None){
                directionRect = directionRect.SliceLeftRatio(0.5f);
            }
            InspectorHelper.DrawFilteredEnumField<SyncronizeDirection>(directionRect, directionProp, direction => instance.AllowDirection.IsAllowed(direction));
            
            // # Path
            // Direction이 None이면 Path는 숨김
            if(directionProp.enumValueIndex != (int)SyncronizeDirection.None){
                string[] options = ReflectionHelper.GetAllAssignablePropertyNames(viewModelType, BindingType.Command, direction).ToArray();
                InspectorHelper.DrawDropdownField(contentRect.SliceRightRatio(0.5f), pathProp, options.Select(o => new DropdownOption(o)).ToArray(), "Property");
            }
        }
    }
    
}