using System;

using UnityEngine;

namespace HUtil.Attribute
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct)]
    public class OnValueChagedAttribute : PropertyAttribute
    {
        public string ActionName { get; }

        public OnValueChagedAttribute(string actionName)
        {
            ActionName = actionName;
        }
    }
}