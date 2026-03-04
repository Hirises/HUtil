using System;

using HUtil.Runtime.UI;

using UnityEditor;

namespace HUtil.Editor.UI
{
    [CustomEditor(typeof(ViewRoot))]
    public class ViewRootDrawer : UnityEditor.Editor
    {
        public override void OnInspectorGUI(){
            var viewRoot = target as ViewRoot;

            var viewModelType = serializedObject.FindProperty("_viewModelType");

            
            InspectorHelper.DrawDropdownField(EditorGUILayout.GetControlRect(), viewModelType, new string[] { "UseViewModel", "Custom" });
        }
    }
}