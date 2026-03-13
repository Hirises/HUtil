using System;

using UnityEngine;

namespace HUtil.UI
{
    [Serializable]
    public struct BindingType
    {
        public static readonly BindingType Invalid = new BindingType(BindingBaseType.None, false);
        public static readonly BindingType Command = new BindingType(BindingBaseType.Command, false);
        public static readonly BindingType Trigger = new BindingType(BindingBaseType.Trigger, false);
        public static readonly BindingType ViewModel = new BindingType(BindingBaseType.ViewModel, false);

        [SerializeField] private BindingBaseType _baseType;
        public BindingBaseType BaseType => _baseType;
        [SerializeField] public bool IsCollection;
        public bool IsValid => BaseType != BindingBaseType.None;

        private BindingType(BindingBaseType baseType, bool isCollection){
            _baseType = baseType;
            IsCollection = isCollection;
        }

        public static BindingType OfType(BindingBaseType baseType){
            return new BindingType(baseType, false);
        }

        public static BindingType OfCollection(BindingBaseType baseType){
            return new BindingType(baseType, true);
        }

        public bool CanAccept(BindingType destinationType){
            return _baseType.CanAccept(destinationType.BaseType) && IsCollection == destinationType.IsCollection;
        }

        public override bool Equals(object obj){
            if(obj is BindingType other){
                return _baseType == other._baseType && IsCollection == other.IsCollection;
            }
            return false;
        }

        public override int GetHashCode(){
            return _baseType.GetHashCode();
        }

        public override string ToString(){
            if(!IsValid){
                return "Invalid";
            }
            if(IsCollection){
                return $"{_baseType} Collection";
            }
            return _baseType.ToString();
        }
    }
}