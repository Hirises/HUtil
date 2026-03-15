using System;
using System.Collections.Generic;
using System.Linq;

using HUtil.Runtime.Command;
using HUtil.Runtime.Extension;
using HUtil.Runtime.Observable;

using NUnit.Framework.Constraints;

using UnityEngine;

namespace HUtil.UI
{
    /// <summary>
    /// 바인딩 타입 확장 메서드
    /// </summary>
    public static class BindingTypeExtension
    {
        // 1. 중앙 집중식 매핑 테이블 (Source of Truth)
        private static readonly Dictionary<Type, BindingBaseType> _typeToBindingMap = new()
        {
            { typeof(int), BindingBaseType.Int },
            { typeof(float), BindingBaseType.Float },
            { typeof(string), BindingBaseType.String },
            { typeof(bool), BindingBaseType.Bool },
            { typeof(Color), BindingBaseType.Color },
            { typeof(Sprite), BindingBaseType.Sprite },
        };

        // 역방향 매핑은 정적 생성자에서 자동으로 빌드
        private static readonly Dictionary<BindingBaseType, Type> _bindingToTypeMap = 
            _typeToBindingMap.ToDictionary(x => x.Value, x => x.Key);

        /// <summary>
        /// 주어진 타입을 바인딩 타입 열거형으로 변환합니다
        /// </summary>
        /// <param name="type">변환할 타입</param>
        /// <returns>변환된 바인딩 타입</returns>
        public static BindingBaseType ToBindingBaseType(this Type type)
        {
            if (type == null) return BindingBaseType.None;

            // 직접 매핑 확인
            if (_typeToBindingMap.TryGetValue(type, out var bindingType)) return bindingType;

            // 특수 조건(상속/인터페이스) 확인
            if (type.IsEnum) return BindingBaseType.Enum;
            if (typeof(CommandBase).IsAssignableFrom(type)) return BindingBaseType.Command;
            if (typeof(IViewModel).IsAssignableFrom(type)) return BindingBaseType.ViewModel;

            return BindingBaseType.None;
        }

        /// <summary>
        /// 주어진 타입을 바인딩 타입 열거형으로 변환합니다
        /// </summary>
        /// <param name="type">변환할 타입</param>
        /// <returns>변환된 바인딩 타입</returns>
        public static BindingType ToBindingType(this Type type)
        {
            //필드는 ObservableProperty<T>, ObservableTrigger, 또는 CommandBase를 상속하는 타입이어야 합니다.
            if(type.IsSubclassOfGeneric(typeof(ObservableProperty<>)))    //ObservableProperty<T>
            {
                var underlyingType = type.GetGenericArgumentsOfType(typeof(ObservableProperty<>))[0];  //제네릭 안쪽에 들어간 타입을 꺼내옴
                return BindingType.OfType(underlyingType.ToBindingBaseType());
            }
            else if(typeof(CommandBase).IsAssignableFrom(type))    //CommandBase
            {
                return BindingType.Command;    //Command는 데이터로만 동기화 가능
            }
            else if(type.IsSubclassOfGeneric(typeof(ObservableList<>)))    //ObservableList<T>
            {
                var underlyingType = type.GetGenericArgumentsOfType(typeof(ObservableList<>))[0];  //제네릭 안쪽에 들어간 타입을 꺼내옴
                return BindingType.OfCollection(underlyingType.ToBindingBaseType());
            }

            //지원하지 않는 타입
            return BindingType.Invalid;
        }

        /// <summary>
        /// 주어진 바인딩 입력에 출력을 연결할 수 있는지 검사합니다
        /// </summary>
        /// <param name="destination">입력 바인딩 타입</param>
        /// <param name="source">출력 바인딩 타입</param>
        /// <returns>변환 가능하면 true, 아니면 false를 반환</returns>
        public static bool CanAccept(this BindingBaseType destination, BindingBaseType source)
        {
            // 둘 중 하나라도 None인 경우 연결 불가능
            if (source == BindingBaseType.None || destination == BindingBaseType.None) {
                return false;
            }

            // 같은 타입인 경우 연결 가능
            if (source == destination){
                return true;
            }
            
            //이 외에는 불가능
            return false;
        }

        public static object GetField(this BindingType type, string path, object vm){
            if(type.IsCollection){
                switch(type.BaseType){
                    case BindingBaseType.Int:
                        return UIRuntimeReflectionHelper.GetObservableList<int>(vm, path);
                    case BindingBaseType.Float:
                        return UIRuntimeReflectionHelper.GetObservableList<float>(vm, path);
                    case BindingBaseType.String:
                        return UIRuntimeReflectionHelper.GetObservableList<string>(vm, path);
                    case BindingBaseType.Bool:
                        return UIRuntimeReflectionHelper.GetObservableList<bool>(vm, path);
                    case BindingBaseType.Enum:
                        return UIRuntimeReflectionHelper.GetObservableList<Enum>(vm, path);
                    case BindingBaseType.Color:
                        return UIRuntimeReflectionHelper.GetObservableList<Color>(vm, path);
                    case BindingBaseType.Sprite:
                        return UIRuntimeReflectionHelper.GetObservableList<Sprite>(vm, path);
                    case BindingBaseType.Command:
                        return UIRuntimeReflectionHelper.GetObservableList<CommandBase>(vm, path);
                    case BindingBaseType.ViewModel:
                        return UIRuntimeReflectionHelper.GetObservableList<IViewModel>(vm, path);
                    default:
                        return null;
                }
            }else{
                switch(type.BaseType){
                    case BindingBaseType.Int:
                        return UIRuntimeReflectionHelper.GetObservableProperty<int>(vm, path);
                    case BindingBaseType.Float:
                        return UIRuntimeReflectionHelper.GetObservableProperty<float>(vm, path);
                    case BindingBaseType.String:
                        return UIRuntimeReflectionHelper.GetObservableProperty<string>(vm, path);
                    case BindingBaseType.Bool:
                        return UIRuntimeReflectionHelper.GetObservableProperty<bool>(vm, path);
                    case BindingBaseType.Enum:
                        return UIRuntimeReflectionHelper.GetObservableProperty<Enum>(vm, path);
                    case BindingBaseType.Color:
                        return UIRuntimeReflectionHelper.GetObservableProperty<Color>(vm, path);
                    case BindingBaseType.Sprite:
                        return UIRuntimeReflectionHelper.GetObservableProperty<Sprite>(vm, path);
                    case BindingBaseType.Command:
                        return UIRuntimeReflectionHelper.GetCommand(vm, path);
                    case BindingBaseType.ViewModel:
                        return UIRuntimeReflectionHelper.GetObservableProperty<IViewModel>(vm, path);
                    default:
                        return null;
                }
            }
        }
    }
}