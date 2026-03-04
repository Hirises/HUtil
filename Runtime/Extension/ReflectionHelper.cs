using UnityEngine;
using System;
using HUtil.Runtime.Observable;
using Unity.Properties;
using HUtil.Runtime.Command;

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
    }
}
