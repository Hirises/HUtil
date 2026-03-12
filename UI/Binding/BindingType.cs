using System;

using UnityEngine;

namespace HUtil.UI.Binding
{
    [Serializable]
    public struct BindingType
    {
        [SerializeField] private BindingBaseType _baseType;
        public BindingBaseType BaseType => _baseType;

        public BindingType(BindingBaseType baseType){
            _baseType = baseType;
        }
    }
}