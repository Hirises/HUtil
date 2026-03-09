using System;
using System.Collections.Generic;
using System.Linq;

using HUtil.UI;
using HUtil.Runtime.Extension;

using UnityEditor;
using UnityEngine;
using HUtil.Editor;

namespace HUtil.UI.Editor
{
    [CustomEditor(typeof(UIComponent))]
    public class UIComponentDrawer : UnityEditor.Editor
    {
        public override void OnInspectorGUI(){
            serializedObject.Update();

            var uiComponent = target as UIComponent;

            var viewModelResolvers = serializedObject.FindProperty("_viewModelResolvers");
            var binders = serializedObject.FindProperty("_binders");

            // # ViewModelResolver
            if(GUI.Button(EditorGUILayout.GetControlRect(), "Add ViewModel")){
                uiComponent.AddNewViewModelResolver();
            }
            for(int i = 0; i < viewModelResolvers.arraySize; i++){
                EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing * 2);
                if(GUI.Button(EditorGUILayout.GetControlRect(), "Remove ViewModel")){
                    uiComponent.RemoveViewModelResolver(i);
                    i--;
                    continue;
                }
                var resolver = viewModelResolvers.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(resolver, true);
            }
            if(viewModelResolvers.arraySize == 0)
            {
                EditorGUILayout.LabelField("No ViewModel");
            }

            // Space
            EditorGUILayout.GetControlRect();

            // # Update Binder List
            if(GUI.Button(EditorGUILayout.GetControlRect(), "Update Binder List")){
                uiComponent.UpdateBinderList();
            }

            // # Binders
            for(int i = 0; i < binders.arraySize; i++){
                var binder = binders.GetArrayElementAtIndex(i);
                EditorGUILayout.PropertyField(binder, new GUIContent($"Binder {i}"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
