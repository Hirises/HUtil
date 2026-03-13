using System;

using UnityEngine;

namespace HUtil.Attribute
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct)]
    public class ScriptableListAttribute : PropertyAttribute
    {
        public string _newItemFunc { get; }

        public ScriptableListAttribute(string newItemFuncPath)
        {
            this._newItemFunc = newItemFuncPath;
        }
    }
}