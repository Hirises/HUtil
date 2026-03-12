using System;

using UnityEngine;

namespace HUtil.Attribute
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct)]
    public class ShowIfAttribute : PropertyAttribute
    {
        public string PropertyName { get; }
        public bool UseValue { get; }
        public int Value { get; }

        public ShowIfAttribute(string propertyName)
        {
            PropertyName = propertyName;
            Value = 0;
            UseValue = false;
        }

        public ShowIfAttribute(string propertyName, int value)
        {
            PropertyName = propertyName;
            Value = value;
            UseValue = true;
        }
    }
}