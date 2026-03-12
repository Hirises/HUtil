using System;

using HUtil.Editor;

using UnityEditor;

using UnityEngine;

namespace HUtil.Attribute.Editor
{
    [CustomPropertyDrawer(typeof(OnValueChagedAttribute))]
    public class OnValueChagedDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var onValueChagedAttribute = attribute as OnValueChagedAttribute;
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property, label);
            if(EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();

                // 2. 리플렉션으로 메서드 찾기
                // targetObject는 해당 컴포넌트(MonoBehaviour 등)의 인스턴스임
                object target = property.serializedObject.targetObject;
                var method = target.GetType().GetMethod(onValueChagedAttribute.ActionName, 
                    System.Reflection.BindingFlags.Instance | 
                    System.Reflection.BindingFlags.Public | 
                    System.Reflection.BindingFlags.NonPublic);

                if (method != null)
                {
                    method.Invoke(target, null);
                }
            }
        }
    }
}