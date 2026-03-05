using System;
using System.Collections.Generic;
using System.Linq;

using HUtil.UI;
using HUtil.Runtime.Extension;

using UnityEditor;
using UnityEngine;

namespace HUtil.Editor.UI
{
    [CustomEditor(typeof(UIComponent))]
    public class UIComponentDrawer : UnityEditor.Editor
    {
        public override void OnInspectorGUI(){
            var uiComponent = target as UIComponent;

            var viewModelType = serializedObject.FindProperty("_viewModelType");
            var binders = serializedObject.FindProperty("_binders");

            // # ViewModel
            (var labelRect, var fieldRect) = EditorGUILayout.GetControlRect().SliceVertical(EditorGUIUtility.labelWidth);
            EditorGUI.LabelField(labelRect, "ViewModel");
            var options = InspectorHelper.GetAllConcreteTypesDerivedFrom(typeof(IViewModel))
                                    .Select(t => new DropdownOption(t.FullName)).ToArray();
            InspectorHelper.DrawDropdownField(fieldRect, viewModelType, options, "ViewModel");

            // Space
            EditorGUILayout.GetControlRect();

            // # Update Binder List
            if(GUI.Button(EditorGUILayout.GetControlRect(), "Update Binder List")){
                uiComponent.UpdateBinderList();
            }

            // # Binders
            for(int i = 0; i < binders.arraySize; i++){
                var binder = binders.GetArrayElementAtIndex(i);
                EditorGUI.PropertyField(EditorGUILayout.GetControlRect(), binder, new GUIContent($"Binder {i}"));
            }
        }
    }
}