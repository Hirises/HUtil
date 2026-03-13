using System;
using System.Reflection;

using HUtil.Editor;

using UnityEditor;

using UnityEditorInternal;

using UnityEngine;

namespace HUtil.Attribute.Editor
{
    [CustomPropertyDrawer(typeof(ScriptableListAttribute))]
    public class ScriptableListDrawer : PropertyDrawer
    {
        private bool _isInitialized = false;

        private void Initialize(SerializedProperty property){
            var scriptableListAttribute = attribute as ScriptableListAttribute;
            
            object target = property.serializedObject.targetObject;
            var method = target.GetType().GetMethod(scriptableListAttribute._newItemFunc, 
                System.Reflection.BindingFlags.Instance | 
                System.Reflection.BindingFlags.Public | 
                System.Reflection.BindingFlags.NonPublic);

            if (method != null)
            {
                method.Invoke(target, new object[] { property.GetActualObject() });
            }
            _isInitialized = true;
            property.serializedObject.ApplyModifiedProperties();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {            
            if(!_isInitialized){
                Initialize(property);
            }
            return EditorGUI.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if(!_isInitialized){
                Initialize(property);
            }
            EditorGUI.PropertyField(position, property, label, true);
        }
    }
}