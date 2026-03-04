using System;
using System.Collections;
using System.Linq;
using System.Reflection;

using UnityEditor;
using UnityEditor.IMGUI.Controls;

using UnityEngine;

namespace HUtil.Editor
{
    public static class InspectorHelper
    {
        /// <summary>
        /// <see cref="SerializedProperty"/>에서 실제 인스턴스를 가져옵니다.
        /// </summary>
        /// <param name="property">속성</param>
        /// <returns>실제 인스턴스</returns>
        public static object GetActualObject(SerializedProperty property)
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
                    
                    if (obj is IList list) obj = list[index];
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

        /// <summary>
        /// 열거형 필드를 그립니다.
        /// </summary>
        /// <param name="rect">그릴 위치</param>
        /// <param name="property">선택된 값</param>
        public static void DrawEnumField(Rect rect, SerializedProperty property)
        {
            property.enumValueIndex = EditorGUI.Popup(rect, property.enumValueIndex, property.enumDisplayNames);
        }

        /// <summary>
        /// 필터링된 열거형 필드를 그립니다.
        /// </summary>
        /// <typeparam name="T">열거형 타입</typeparam>
        /// <param name="rect">그릴 위치</param>
        /// <param name="property">선택된 값</param>
        /// <param name="filter">필터링 함수</param>
        public static void DrawFilteredEnumField<T>(Rect rect, SerializedProperty property, Func<T, bool> filter) where T : Enum
        {
            T[] allValues = Enum.GetValues(typeof(T)).Cast<T>().ToArray();
            T[] values = allValues.Where(filter).ToArray();
            string[] displayNames = values.Select(value => value.ToString()).ToArray();

            //필터링된 인덱스로 변환
            int index = Array.IndexOf(values, allValues[property.enumValueIndex]);
            index = Math.Clamp(index, 0, displayNames.Length - 1);
            
            //다시 원래 인덱스로 변환
            int newIndex = EditorGUI.Popup(rect, index, displayNames);
            property.enumValueIndex = Array.IndexOf(allValues, values[newIndex]);
        }

        /// <summary>
        /// 드롭다운 필드를 그립니다; string값을 사용합니다.
        /// </summary>
        /// <param name="rect">그릴 위치</param>
        /// <param name="property">선택된 값</param>
        /// <param name="options">옵션 리스트</param>
        public static void DrawDropdownField(Rect rect, SerializedProperty property, DropdownOption[] options, string title = "Dropdown")
        {
            if(GUI.Button(rect, property.stringValue, EditorStyles.popup)){
                var state = new AdvancedDropdownState();
                var dropdown = new SearchableDropdown(state, options, property, title);
                dropdown.Show(rect);
            }
        }
    }

    /// <summary>
    /// <see cref="SearchableDropdown"/>에서 사용되는 드롭다운 옵션
    /// </summary>
    public struct DropdownOption
    {
        public string Name;
        public string Value;

        public DropdownOption(string name)
        {
            Name = name;
            Value = name;
        }

        public DropdownOption(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }

    /// <summary>
    /// 검색 가능한 드롭다운
    /// </summary>
    public class SearchableDropdown : AdvancedDropdown
    {
        private DropdownOption[] _options;
        private SerializedProperty _property;
        private string _title;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="state">드롭다운 상태; 단순히 new()로 넣어주면 된다</param>
        /// <param name="options">옵션 리스트</param>
        /// <param name="property">선택된 값</param>
        /// <param name="title">제목</param>
        public SearchableDropdown(AdvancedDropdownState state, DropdownOption[] options, SerializedProperty property, string title = "Dropdown") : base(state)
        {
            _options = options;
            _property = property;
            _title = title;
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem(_title);
            foreach(var option in _options){
                var item = new DropdownOptionItem(option);
                root.AddChild(item);
            }
            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            DropdownOptionItem dropdownOptionItem = item as DropdownOptionItem;
            _property.stringValue = dropdownOptionItem.Option.Value;
            _property.serializedObject.ApplyModifiedProperties();
        }

        private class DropdownOptionItem : AdvancedDropdownItem
        {
            public DropdownOption Option;
            public DropdownOptionItem(DropdownOption option) : base(option.Name)
            {
                Option = option;
            }
        }
    }
}