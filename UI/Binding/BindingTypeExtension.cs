using System;
using System.Collections.Generic;
using System.Linq;

using HUtil.Runtime.Command;
using HUtil.Runtime.Observable;

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
            { typeof(long), BindingBaseType.Long },
            { typeof(double), BindingBaseType.Double },
            { typeof(DateTime), BindingBaseType.DateTime },
            { typeof(Vector2), BindingBaseType.Vector2 },
            { typeof(Vector3), BindingBaseType.Vector3 },
            { typeof(Vector4), BindingBaseType.Vector4 },
            { typeof(Quaternion), BindingBaseType.Quaternion },
            { typeof(Color), BindingBaseType.Color },
            { typeof(Color32), BindingBaseType.Color32 },
            { typeof(GameObject), BindingBaseType.GameObject },
            { typeof(Transform), BindingBaseType.Transform },
        };

        // 역방향 매핑은 정적 생성자에서 자동으로 빌드
        private static readonly Dictionary<BindingBaseType, Type> _bindingToTypeMap = 
            _typeToBindingMap.ToDictionary(x => x.Value, x => x.Key);

        /// <summary>
        /// 주어진 타입을 바인딩 타입 열거형으로 변환합니다
        /// </summary>
        /// <param name="type">변환할 타입</param>
        /// <returns>변환된 바인딩 타입</returns>
        public static BindingBaseType ToBindingType(this Type type)
        {
            if (type == null) return BindingBaseType.None;

            // 직접 매핑 확인
            if (_typeToBindingMap.TryGetValue(type, out var bindingType)) return bindingType;

            // 특수 조건(상속/인터페이스) 확인
            if (type.IsEnum) return BindingBaseType.Enum;
            if (typeof(CommandBase).IsAssignableFrom(type)) return BindingBaseType.Command;
            if (typeof(ObservableTrigger).IsAssignableFrom(type)) return BindingBaseType.Trigger;
            if (typeof(IViewModel).IsAssignableFrom(type)) return BindingBaseType.ViewModel;
            if (typeof(ObservableList<>).IsAssignableFrom(type)) return BindingBaseType.List;

            return BindingBaseType.None;
        }

        /// <summary>
        /// 주어진 바인딩 타입을 실제 타입으로 변환합니다
        /// </summary>
        /// <param name="bindingType">변환할 바인딩 타입</param>
        /// <returns>변환된 실제 타입</returns>
        public static Type ToType(this BindingBaseType bindingType)
        {
            if (bindingType == BindingBaseType.Enum) return typeof(Enum);
            // 인터페이스나 추상 클래스는 기본 대표 타입으로 반환
            if (bindingType == BindingBaseType.Command) return typeof(CommandBase);
            if (bindingType == BindingBaseType.ViewModel) return typeof(IViewModel);
            if (bindingType == BindingBaseType.Trigger) return typeof(ObservableTrigger);
            if (bindingType == BindingBaseType.List) return typeof(ObservableList<>);
            
            return _bindingToTypeMap.TryGetValue(bindingType, out var type) ? type : typeof(void);
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
    }
}