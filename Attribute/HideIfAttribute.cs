using System;

using UnityEngine;

namespace HUtil.Attribute
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct)]
    public class HideIfAttribute : PropertyAttribute
    {
        public string PropertyName { get; }
        public bool UseValue { get; }
        public int Value { get; }

        public HideIfAttribute(string propertyName)
        {
            PropertyName = propertyName;
            Value = 0;
            UseValue = false;
        }

        public HideIfAttribute(string propertyName, int value)
        {
            PropertyName = propertyName;
            Value = value;
            UseValue = true;
        }
    }
}