using System;
using System.Collections.Generic;
using System.Linq;

using HUtil.Runtime.UI;
using HUtil.Runtime.Extension;

using UnityEditor;
using UnityEngine;

namespace HUtil.Editor.UI
{
    [CustomEditor(typeof(ViewRoot))]
    public class ViewRootDrawer : UnityEditor.Editor
    {
        public override void OnInspectorGUI(){
            var viewRoot = target as ViewRoot;

            var viewModelType = serializedObject.FindProperty("_viewModelType");
            var binders = serializedObject.FindProperty("_binders");

            // # ViewModel
            (var labelRect, var fieldRect) = EditorGUILayout.GetControlRect().SliceVertical(EditorGUIUtility.labelWidth);
            EditorGUI.LabelField(labelRect, "ViewModel");
            var options = ReflectionHelper.GetAllViewModelTypes().Select(t => new DropdownOption(t.FullName)).ToArray();
            InspectorHelper.DrawDropdownField(fieldRect, viewModelType, options, "ViewModel");

            // # Update Binder List
            if(GUI.Button(EditorGUILayout.GetControlRect(), "Update Binder List")){
                viewRoot.UpdateBinderList();
            }

            // # Binders
            for(int i = 0; i < binders.arraySize; i++){
                var binder = binders.GetArrayElementAtIndex(i);
                EditorGUI.PropertyField(EditorGUILayout.GetControlRect(), binder, new GUIContent($"Binder {i}"));
            }
        }
    }
}