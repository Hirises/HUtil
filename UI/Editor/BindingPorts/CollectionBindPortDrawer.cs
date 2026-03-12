using System;

using HUtil.UI;
using HUtil.Runtime.Extension;
using HUtil.Editor;

using UnityEditor;
using UnityEngine;
using HUtil.UI.Binder;
using System.Linq;
using System.Collections.Generic;

namespace HUtil.UI.Editor
{
    [CustomPropertyDrawer(typeof(CollectionBindingPort))]
    public class CollectionBindPortDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            //필요한 변수들 캐싱
            var instance = property.GetActualObject() as CollectionBindingPort;
            var binder = property.serializedObject.targetObject as MonoBinder;
            if(instance == null || binder == null){
                EditorGUI.HelpBox(position, $"Internal error: {instance} {binder}", MessageType.Error);
                return;
            }

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
            InspectorHelper.DrawFilteredEnumField<BindingMode>(directionRect, directionProp, direction => instance.AllowDirection.CanAccept(direction));

            // # Path
            // Direction이 None이면 Path는 숨김
            if(directionProp.enumValueIndex != (int)BindingMode.None){
                List<string> options = binder.GetAllBindablePropertyNamesEditor(BindingType.List, direction);
                InspectorHelper.DrawSearchableDropdownField(contentRect.SliceRightRatio(0.5f), pathProp, options.Select(o => new DropdownOption(o)).ToArray(), "List");
            }
        }
    }

}
