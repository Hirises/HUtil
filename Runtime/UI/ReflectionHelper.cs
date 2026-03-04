using UnityEngine;
using System;
using HUtil.Runtime.Observable;
using Unity.Properties;
using HUtil.Runtime.Command;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using HUtil.Runtime.UI;
using System.Linq;

namespace HUtil.Runtime.Extension
{
    /// <summary>
    /// 리플렉션 관련 확장 메서드
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// 주어진 객체 내부의 <see cref="ObservableProperty{T}"/>를 가져옵니다.
        /// </summary>
        /// <typeparam name="T">프로퍼티 타입</typeparam>
        /// <param name="obj">객체</param>
        /// <param name="propertyName">프로퍼티 이름</param>
        /// <returns><see cref="ObservableProperty{T}"/></returns>
        public static ObservableProperty<T> GetObservableProperty<T>(object obj, string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException(nameof(propertyName), "PropertyName is null or empty");
            }
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "Object is null");
            }

            object _obj = obj;
            if (PropertyContainer.TryGetValue(ref _obj, propertyName, out ObservableProperty<T> value))
            {
                return value;
            }
            else
            {
                throw new ArgumentException($"Property {propertyName} not found on object {obj.GetType().Name}");
            }
        }

        /// <summary>
        /// 주어진 객체 내부의 <see cref="CommandBase"/>를 가져옵니다.
        /// </summary>
        /// <param name="obj">객체</param>
        /// <param name="commandName">명령 이름</param>
        /// <returns><see cref="CommandBase"/></returns>
        public static CommandBase GetCommand(object obj, string commandName)
        {
            if (string.IsNullOrEmpty(commandName))
            {
                throw new ArgumentNullException(nameof(commandName), "CommandName is null or empty");
            }
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "Object is null");
            }

            object _obj = obj;
            if (PropertyContainer.TryGetValue(ref _obj, commandName, out CommandBase value))
            {
                return value;
            }
            else
            {
                throw new ArgumentException($"Command {commandName} not found on object {obj.GetType().Name}");
            }
        }

        /// <summary>
        /// 모든 <see cref="IViewModel"/> 타입 구현체를 가져옵니다.
        /// </summary>
        /// <returns><see cref="IViewModel"/> 타입 구현체 리스트</returns>
        public static List<Type> GetAllViewModelTypes()
        {
            // 1. TypeCache를 통해 IViewModel을 상속/구현한 모든 타입을 즉시 가져옵니다.
            var typeCollection = TypeCache.GetTypesDerivedFrom<IViewModel>(); 
            // 클래스인 경우: TypeCache.GetTypesDerivedFrom(typeof(ViewModelBase));

            // 2. 추상 클래스(Abstract)나 인터페이스 자체는 인스턴스화할 수 없으므로 제외합니다.
            var concreteTypes = typeCollection
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .ToList();

            return concreteTypes;
        }

        /// <summary>
        /// 주어진 객체 내부의 보든 <see cref="ObservableProperty{T}"/> 및 <see cref="CommandBase"/>의 이름을 가져옵니다
        /// </summary>
        /// <param name="type">객체 타입</param>
        /// <returns>프로퍼티 이름 리스트</returns>
        public static List<string> GetAllAssignablePropertyNames(Type type)
        {
            var propertyNames = new List<string>();
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach(var field in fields){
                if(field.FieldType.IsSubclassOfRawGeneric(typeof(ObservableProperty<>)) || field.FieldType.IsAssignableFrom(typeof(CommandBase))){
                    propertyNames.Add(field.Name);
                }
            }
            return propertyNames;
        }

        /// <summary>
        /// 이 타입이 주어진 제네릭 타입의 서브클래스인지 확인합니다.
        /// </summary>
        /// <param name="toCheck">확인할 타입</param>
        /// <param name="generic">제네릭 타입</param>
        /// <returns>서브클래스 여부</returns>
        public static bool IsSubclassOfRawGeneric(this Type toCheck, Type generic)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                // 현재 타입이 제네릭인지 확인하고, 원본 정의(Definition)를 가져옴
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                // 부모 클래스로 올라가며 다시 체크 (상속 관계 대응)
                toCheck = toCheck.BaseType;
            }
            return false;
        }
    }
}
