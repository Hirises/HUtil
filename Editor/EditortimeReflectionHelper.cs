using System;
using System.Collections;
using System.Reflection;

using UnityEditor;

namespace HUtil.Editor
{
    /// <summary>
    /// 에디터 타임에서 동작하는 리플렉션 관련 확장 메서드
    /// </summary>
    public static class EditortimeReflectionHelper
    {

        /// <summary>
        /// <see cref="SerializedProperty"/>에서 실제 필드 정보를 가져옵니다.
        /// </summary>
        /// <param name="property">속성</param>
        /// <returns>필드 정보</returns>
        public static FieldInfo GetFieldInfo(this SerializedProperty property){
            // 1. 최상위 부모 오브젝트 (MonoBehaviour 등) 가져오기
            object obj = property.serializedObject.targetObject;
            
            // 2. propertyPath (예: "bindingList.Array.data[0]._info")를 따라가며 실제 인스턴스 탐색
            string[] path = property.propertyPath.Replace(".Array.data[", "[").Split('.');
            
            FieldInfo fieldInfo = null;
            foreach (var name in path)
            {
                if (name.Contains("["))
                {
                    // 리스트나 배열 처리
                    var fieldName = name.Substring(0, name.IndexOf("["));
                    var index = int.Parse(name.Substring(name.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    
                    fieldInfo = obj.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    obj = fieldInfo.GetValue(obj);
                    
                    if (obj is IList list) {
                        if(index < list.Count){
                            obj = list[index];
                        }else{
                            return null;
                        }
                    }
                }
                else
                {
                    fieldInfo = obj.GetType().GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (fieldInfo == null) return null;
                    obj = fieldInfo.GetValue(obj);
                }
            }
            return fieldInfo;
        }
        
        /// <summary>
        /// <see cref="SerializedProperty"/>에서 실제 인스턴스를 가져옵니다.
        /// </summary>
        /// <param name="property">속성</param>
        /// <returns>실제 인스턴스</returns>
        public static object GetActualObject(this SerializedProperty property)
        {
            // 1. 최상위 부모 오브젝트 (MonoBehaviour 등) 가져오기
            object obj = property.serializedObject.targetObject;
            
            // 2. propertyPath (예: "bindingList.Array.data[0]._info")를 따라가며 실제 인스턴스 탐색
            string[] path = property.propertyPath.Replace(".Array.data[", "[").Split('.');
            
            foreach (var name in path)
            {
                if (name.Contains("["))
                {
                    // 리스트나 배열 처리
                    var fieldName = name.Substring(0, name.IndexOf("["));
                    var index = int.Parse(name.Substring(name.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    
                    var field = obj.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    obj = field.GetValue(obj);
                    
                    if (obj is IList list){
                        if(index < list.Count){
                            obj = list[index];
                        }else{
                            return null;
                        }
                    }
                }
                else
                {
                    var field = obj.GetType().GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    if (field == null) return null;
                    obj = field.GetValue(obj);
                }
            }
            return obj;
        }
    }
}