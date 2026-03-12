using System;

using UnityEditor;

namespace HUtil.Attribute.Editor
{
    internal static class AttributeEditortimeReflectionHelper
    {
        public static SerializedProperty GetRelativeProperty(this SerializedProperty fromProperty, string relativePath)
        {
            int lastDot = fromProperty.propertyPath.LastIndexOf('.');
            string targetPath = "";
            if(lastDot != -1)
            {
                string parentPath = fromProperty.propertyPath.Substring(0, lastDot);
                targetPath = $"{parentPath}.{relativePath}";
            }
            else
            {
                targetPath = relativePath;
            }
            return fromProperty.serializedObject.FindProperty(targetPath);
        }
    }
}