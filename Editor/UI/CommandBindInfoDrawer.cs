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
                string[] options = ReflectionHelper.GetAllAssignablePropertyNames(viewModelType).ToArray();
                InspectorHelper.DrawDropdownField(contentRect.SliceRightRatio(0.5f), path, options.Select(o => new DropdownOption(o)).ToArray(), "Property");
            }
        }
    }
    
}