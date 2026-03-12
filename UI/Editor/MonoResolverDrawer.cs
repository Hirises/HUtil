using System;

using HUtil.Editor;

using UnityEditor;

using UnityEditorInternal;

namespace HUtil.UI.Editor
{
    [CustomEditor(typeof(MonoResolver))]
    public class MonoResolverDrawer : UnityEditor.Editor
    {
        private ReorderableList _viewModelResolversList;
        private SerializedProperty _viewModelResolversProp;
        
        private void OnEnable()
        {
            var resolver = target as MonoResolver;
            _viewModelResolversProp = serializedObject.FindProperty("_viewModelResolvers");
            _viewModelResolversList = new ReorderableList(serializedObject, _viewModelResolversProp);
            _viewModelResolversList.drawHeaderCallback = (rect) => {
                EditorGUI.LabelField(rect, "ViewModel Resolvers");
            };
            _viewModelResolversList.onAddCallback = (list) => {// 리스트의 현재 크기 늘리기
                var resolver = target as MonoResolver;
                resolver.ViewModelResolvers.Add(new ViewModelResolver());
                serializedObject.ApplyModifiedProperties();
            };
            _viewModelResolversList.elementHeightCallback = (int index) => {
                // 특정 조건이나 프로퍼티의 높이에 따라 동적으로 계산
                var element = _viewModelResolversProp.GetArrayElementAtIndex(index);
                return EditorGUI.GetPropertyHeight(element) + 5; // 프로퍼티 높이 + 여백
            };
            _viewModelResolversList.drawElementCallback = (rect, index, isActive, isFocused) => {
                EditorGUI.PropertyField(rect, _viewModelResolversProp.GetArrayElementAtIndex(index), true);
            };
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var prop = serializedObject.GetIterator();
            prop.NextVisible(true);
            while(prop.NextVisible(false))
            {
                if(prop.name == _viewModelResolversProp.name){
                    _viewModelResolversList.DoLayoutList();
                    continue;
                }
                EditorGUILayout.PropertyField(prop);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}