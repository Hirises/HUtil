using System;
using System.Linq;

using HUtil.Editor;
using HUtil.Runtime.Extension;

using UnityEditor;

using UnityEngine;

namespace HUtil.UI.Editor
{
    [CustomPropertyDrawer(typeof(ViewModelResolver))]
    public class ViewModelResolverDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label){
            var bindingMethodProp = property.FindPropertyRelative("_bindingMethod");
            var bindMapProp = property.FindPropertyRelative("_bindMap");

            bool useDynamicBinding = bindingMethodProp.enumValueIndex == (int)ViewModelResolver.BindingMethod.DynamicBinding;
            int numLines = 3 + (useDynamicBinding ? 1 : 0) + bindMapProp.arraySize;

            return ((EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * numLines) - EditorGUIUtility.standardVerticalSpacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){
            var resolver = property.GetActualObject() as ViewModelResolver;
            var fieldInfo = property.GetFieldInfo();
            if(resolver == null || fieldInfo == null){
                EditorGUI.HelpBox(position, $"Internal error: {resolver} {fieldInfo}", MessageType.Error);
                return;
            }

            var viewModelTypeProp = property.FindPropertyRelative("_viewModelType");
            var bindingMethodProp = property.FindPropertyRelative("_bindingMethod");
            var viewModelProp = property.FindPropertyRelative("_viewModelProp");
            var bindMapProp = property.FindPropertyRelative("_bindMap");

            Rect curLine = position.GetStartLine();

            // # ViewModelType
            var options = InspectorHelper.GetAllConcreteTypesDerivedFrom(typeof(IViewModel));
            (var labelRect, var contentRect) = curLine.SliceVertical(EditorGUIUtility.labelWidth);
            EditorGUI.LabelField(labelRect, "ViewModel Type");
            InspectorHelper.DrawSearchableDropdownField(contentRect, viewModelTypeProp, options.Select(type => new DropdownOption(type.FullName)).ToArray());
            curLine = curLine.GetNextLine();

            // # BindingMethod
            (labelRect, contentRect) = curLine.SliceVertical(EditorGUIUtility.labelWidth);
            EditorGUI.LabelField(labelRect, "Binding Method");
            InspectorHelper.DrawEnumField(contentRect, bindingMethodProp);
            curLine = curLine.GetNextLine();

            // # ViewModelProp
            if(bindingMethodProp.enumValueIndex == (int)ViewModelResolver.BindingMethod.DynamicBinding){
                EditorGUI.PropertyField(curLine, viewModelProp, true);
                curLine = curLine.GetNextLine();
            }

            // # BindMap
            EditorGUI.LabelField(curLine, "Binding Map");
            curLine = curLine.GetNextLine();
            var viewModelType = InspectorHelper.GetAllConcreteTypesDerivedFrom(typeof(IViewModel)).FirstOrDefault(type => type.FullName == viewModelTypeProp.stringValue);
            if(viewModelType != null){
                EditorGUI.indentLevel++;
                var bindList = UIRuntimeReflectionHelper.GetAllBindablePropertyNames(viewModelType);
                if(bindList.Count != bindMapProp.arraySize){
                    bindMapProp.ClearArray();
                    bindMapProp.arraySize = bindList.Count;
                    for(int i = 0; i < bindList.Count; i++){
                        var sourceName = bindList[i];
                        var destinationName = bindList[i];
                        var itemProp = bindMapProp.GetArrayElementAtIndex(i);
                        itemProp.FindPropertyRelative("_sourcePropertyPath").stringValue = sourceName;
                        itemProp.FindPropertyRelative("_destinationPropertyPath").stringValue = destinationName;
                    }
                }
                for(int i = 0; i < bindMapProp.arraySize; i++){
                    var itemProp = bindMapProp.GetArrayElementAtIndex(i);
                    var sourceNameProp = itemProp.FindPropertyRelative("_sourcePropertyPath");
                    var destinationNameProp = itemProp.FindPropertyRelative("_destinationPropertyPath");

                    if(sourceNameProp.stringValue != bindList[i]){
                        // 속성 이름이 변경되었으면 초기화
                        sourceNameProp.stringValue = bindList[i];
                        destinationNameProp.stringValue = bindList[i];
                    }

                    var (sourceRect, destinationRect) = curLine.SliceVerticalRatio(0.5f);
                    EditorGUI.LabelField(sourceRect, sourceNameProp.stringValue);
                    destinationNameProp.stringValue = EditorGUI.TextField(destinationRect, destinationNameProp.stringValue);
                    curLine = curLine.GetNextLine();
                }
                EditorGUI.indentLevel--;
            }
        }
    }
}