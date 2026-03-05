using System;
using System.Linq;

using HUtil.Runtime.Command;
using HUtil.Runtime.Observable;

using UnityEngine;

namespace HUtil.Runtime.UI
{
    public static class BindingTypeExtension
    {
        public static BindingType ToBindingType(this Type type)
        {
            if (type == typeof(int)) return BindingType.Int;
            if (type == typeof(float)) return BindingType.Float;
            if (type == typeof(string)) return BindingType.String;
            if (type == typeof(bool)) return BindingType.Bool;
            if (type == typeof(long)) return BindingType.Long;
            if (type == typeof(double)) return BindingType.Double;
            if (type == typeof(DateTime)) return BindingType.DateTime;
            if (type.IsEnum) return BindingType.Enum;
            if (type == typeof(GameObject)) return BindingType.GameObject;
            if (type == typeof(Transform)) return BindingType.Transform;
            if (type == typeof(CommandBase)) return BindingType.Command;
            if (type == typeof(ObservableTrigger)) return BindingType.Trigger;
            if (type == typeof(IViewModel)) return BindingType.ViewModel;
            if (type == typeof(Vector2)) return BindingType.Vector2;
            if (type == typeof(Vector3)) return BindingType.Vector3;
            if (type == typeof(Vector4)) return BindingType.Vector4;
            if (type == typeof(Quaternion)) return BindingType.Quaternion;
            if (type == typeof(Color)) return BindingType.Color;
            if (type == typeof(Color32)) return BindingType.Color32;
            if (type == typeof(DateTime)) return BindingType.DateTime;
            return BindingType.None;
        }

        public static Type ToType(this BindingType bindingType)
        {
            if (bindingType == BindingType.Int) return typeof(int);
            if (bindingType == BindingType.Float) return typeof(float);
            if (bindingType == BindingType.String) return typeof(string);
            if (bindingType == BindingType.Bool) return typeof(bool);
            if (bindingType == BindingType.Long) return typeof(long);
            if (bindingType == BindingType.Double) return typeof(double);
            if (bindingType == BindingType.DateTime) return typeof(DateTime);
            if (bindingType == BindingType.Enum) return typeof(Enum);
            if (bindingType == BindingType.GameObject) return typeof(GameObject);
            if (bindingType == BindingType.Transform) return typeof(Transform);
            if (bindingType == BindingType.Command) return typeof(CommandBase);
            if (bindingType == BindingType.Trigger) return typeof(ObservableTrigger);
            if (bindingType == BindingType.ViewModel) return typeof(IViewModel);
            if (bindingType == BindingType.Vector2) return typeof(Vector2);
            if (bindingType == BindingType.Vector3) return typeof(Vector3);
            if (bindingType == BindingType.Vector4) return typeof(Vector4);
            if (bindingType == BindingType.Quaternion) return typeof(Quaternion);
            if (bindingType == BindingType.Color) return typeof(Color);
            if (bindingType == BindingType.Color32) return typeof(Color32);
            if (bindingType == BindingType.DateTime) return typeof(DateTime);
            return typeof(void);
        }

        public static bool IsAssignableTo(this BindingType sendingType, BindingType receivingType){
            BindingType[] assignableTypes = new BindingType[] { receivingType };
            switch(receivingType){
                case BindingType.String:
                    assignableTypes = new BindingType[] { BindingType.String, BindingType.Int, BindingType.Float, 
                        BindingType.Bool, BindingType.Long, BindingType.Double, BindingType.DateTime, BindingType.Enum,
                        BindingType.Vector2, BindingType.Vector3, BindingType.Vector4, BindingType.Quaternion, 
                        BindingType.Color, BindingType.Color32
                    };
                    break;
                case BindingType.Long:
                case BindingType.Int:
                    assignableTypes = new BindingType[] { BindingType.Int, BindingType.Long };
                    break;
                case BindingType.Float:
                case BindingType.Double:
                    assignableTypes = new BindingType[] { BindingType.Float, BindingType.Double };
                    break;
            }
            return assignableTypes.Contains(sendingType);
        }
    }
}